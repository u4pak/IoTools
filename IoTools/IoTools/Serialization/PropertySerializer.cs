using CUE4Parse.FileProvider;
using CUE4Parse.MappingsProvider;
using CUE4Parse.UE4.Assets.Objects;
using CUE4Parse.UE4.Objects.Core.i18N;
using CUE4Parse.UE4.Objects.GameplayTags;
using CUE4Parse.UE4.Objects.UObject;
using CUE4Parse.Utils;
using IoTools.Providers;
using IoTools.StructData;
using PropertyEditor.Core.UE;
using FNameEntrySerialized = IoTools.StructData.FNameEntrySerialized;

namespace PropertyEditor.Core;

public class PropertySerializer
{

    public string ExportType;
    public List<FPropertyTag> Properties;
    public TypeMappings Mappings;
    
    public PropertySerializer(string type, TypeMappings mappings, 
        List<FPropertyTag> properties)
    {
        ExportType = type;
        Properties = properties;
        Mappings = mappings;
    }

    public byte[] Serialize(List<FNameEntrySerialized> names)
    {
        if (Mappings.Types.TryGetValue(ExportType, out var ustruct))
        {
            var zeroMask = new List<uint>();
            var frags = new List<FFragment> { new() };

            void IncludeProp(bool isZero)
            {
                if (frags.Last().ValueNum == Byte.MaxValue)
                {
                    trimZeroMask();
                    frags.Add(new FFragment());
                }
                
                var last = frags.Last();
                frags[^1] = last with
                {
                    ValueNum = (byte)(last.ValueNum + 1), IsLast = false, HasAnyZeroes = last.HasAnyZeroes | isZero
                };

                zeroMask.Add((uint)(isZero ? 1 : 0));
            }

            void ExcludeProp()
            {
                if (frags.Last().ValueNum != 0 || frags.Last().SkipNum == Byte.MaxValue)
                {
                    trimZeroMask();
                    frags.Add(new FFragment());
                }

                var last = frags.Last();
                frags[^1] = last with { SkipNum = (byte)(last.SkipNum + 1) };
            }

            var schema = 0;
            for (var i = 0; i < ustruct.Properties.Count; i++)
            {
                var possiblePropName = ustruct.Properties.ElementAt(i).Value.Name;

                if (schema < Properties.Count)
                {
                    if (possiblePropName == Properties[schema].Name.Text)
                    {
                        var isZero = Properties[schema].Size == 0;
                        IncludeProp(isZero);
                        schema++;
                    }
                    else ExcludeProp();
                }
            }

            void trimZeroMask()
            {
                var frag = frags.Last();
                if (!frag.HasAnyZeroes)
                    zeroMask.RemoveRange(zeroMask.Count - frag.ValueNum, frag.ValueNum);
            }

            trimZeroMask();
            
            while (frags.Count > 1 && frags.Last().ValueNum == 0)
            {
                frags.RemoveAt(frags.Count - 1);
            }
            
            frags[^1] = frags.Last() with { IsLast = true };
            
            var header = new List<byte>();
            frags.ForEach(x =>
                header.AddRange(
                    BitConverter.GetBytes(FFragment.Pack(x.SkipNum, x.HasAnyZeroes, x.ValueNum, x.IsLast))));

            if (zeroMask.Any())
            {
                var numBits = zeroMask.Count;
                var lastWordMask = ~0u >> (int)((32u - numBits) % 32u);
                if (numBits <= 8)
                {
                    var word = zeroMask.First() & lastWordMask;
                    for (int i = 1; i < zeroMask.Count; i++)
                        word |= zeroMask[i] & lastWordMask;
                    header.Add((byte)word);
                }
                else if (numBits <= 16)
                {
                    var word = zeroMask.First() & lastWordMask;
                    for (int i = 1; i < zeroMask.Count; i++)
                        word |= zeroMask[i] & lastWordMask;
                    header.AddRange(BitConverter.GetBytes((ushort)word));
                }
                else
                {
                    var numWords = numBits.DivideAndRoundUp(32);

                    for (int wordIdx = 0; wordIdx < numWords - 1; wordIdx++)
                    {
                        var word = zeroMask[wordIdx];
                        header.AddRange(BitConverter.GetBytes(word));
                    }

                    var lastWord = zeroMask[numWords - 1] & lastWordMask;
                    header.AddRange(BitConverter.GetBytes(lastWord));
                }
            }
            
            foreach (var tag in Properties)
            {
                var prop = SerializeProperty(Mappings, tag.PropertyType.Text, tag.Tag, tag.TagData.StructType, names,
                    tag.Size != 0 ? ReadType.NORMAL : ReadType.ZERO);
                if (prop != null)
                    header.AddRange(prop);
            }
            
            return header.ToArray();
        }

        return Array.Empty<byte>();
    }

    public static byte[] SerializeProperty(TypeMappings mappings, string propType, 
        FPropertyTagType? tag, string? structType, 
        List<FNameEntrySerialized> names, ReadType type = ReadType.NORMAL)
    {
        if (type == ReadType.ZERO)
            return Array.Empty<byte>();

        if (structType == "GameplayTagContainer")
        {
            var buffer = new List<byte>();
            foreach (var prop in ((FGameplayTagContainer)((UScriptStruct)tag.GenericValue).StructType).GameplayTags)
                buffer.AddRange(new NamePropertySerializer(names, prop.Text).GetBytes());
            return buffer.ToArray();
        }

        return propType switch
        {
            "ArrayProperty" => new ArrayPropertySerializer((UScriptArray)tag.GenericValue, names)
                .GetBytes(),
            "BoolProperty" => new[] { (byte)((bool)tag.GenericValue ? 1 : 0) },
            "ByteProperty" => new[] { (byte)tag.GenericValue },
            "EnumProperty" => new[]
            {
                (byte)mappings.Enums.GetValueOrDefault((structType ?? (FName)tag.GenericValue).Text.SubstringBefore("::"))
                    .First(x => x.Value == (structType ?? (FName)tag.GenericValue).Text.SubstringAfter("::")).Key
            },
            "FloatProperty" => BitConverter.GetBytes((float)tag.GenericValue),
            "IntProperty" => BitConverter.GetBytes((int)tag.GenericValue),
            "ObjectProperty" => BitConverter.GetBytes(((FPackageIndex)tag.GenericValue).Index),
            "SoftObjectProperty" => new TopLevelSoftObject(names,
                ((FSoftObjectPath)tag.GenericValue).AssetPathName.Text).GetBytes(),
            "StructProperty" => new PropertySerializer(structType!, mappings,
                ((dynamic)((UScriptStruct)tag.GenericValue).StructType).Properties).Serialize(names),
            "MapProperty" => new MapSerializer((UScriptMap)tag.GenericValue, names).GetBytes(),
            "NameProperty" => new NamePropertySerializer(names, ((FName)tag.GenericValue).Text).GetBytes(),
            "TextProperty" => new TextPropertySerializer((FText)tag.GenericValue).GetBytes(),
            _ => null
        };
    }
}