using CUE4Parse.UE4.Assets.Objects;
using CUE4Parse.UE4.Objects.UObject;
using FNameEntrySerialized = IoTools.StructData.FNameEntrySerialized;

namespace PropertyEditor.Core.UE;

using static IoTools.Providers.Provider;

public class MapSerializer
{
    private List<byte> _bytes = new();
    
    public MapSerializer(UScriptMap map, List<FNameEntrySerialized> names)
    {
        var mappings = provider.MappingsForGame;
        _bytes.AddRange(BitConverter.GetBytes(0));
        _bytes.AddRange(BitConverter.GetBytes(map.Properties.Count));
        foreach (var entry in map.Properties)
        {
            var key = entry.Key;
            EnumProperty? innerEnumName = null;
            if (map.KeyType == "EnumProperty" && !((FName)key.GenericValue).Text.Contains("::"))
                innerEnumName = new EnumProperty(map.InnerType + "::" + (FName)key.GenericValue);
            EnumProperty? outerEnumName = null;
            if (map.ValueType == "EnumProperty" && !((FName)key.GenericValue).Text.Contains("::"))
                outerEnumName = new EnumProperty(map.OuterType + "::" + (FName)key.GenericValue);
            _bytes.AddRange(PropertySerializer.SerializeProperty(mappings, map.KeyType,
                entry.Key, innerEnumName?.Value.Text, names));
            _bytes.AddRange(PropertySerializer.SerializeProperty(mappings, map.ValueType,
                entry.Value, outerEnumName?.Value.Text, names));
        }
    }

    public byte[] GetBytes() => _bytes.ToArray();
}