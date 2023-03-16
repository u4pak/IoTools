using CUE4Parse.UE4.Assets;
using CUE4Parse.UE4.IO.Objects;
using CUE4Parse.UE4.Objects.UObject;
using CUE4Parse.UE4.Versions;
using IoTools.Providers;
using IoTools.Readers;
using IoTools.StructData;
using IoTools.Writers;
using PropertyEditor.Core;
using static IoTools.Providers.Provider;
using FExportMapEntry = IoTools.StructData.FExportMapEntry;
using FNameEntrySerialized = IoTools.StructData.FNameEntrySerialized;
using FZenPackageSummary = IoTools.StructData.FZenPackageSummary;

namespace IoTools.Serialization;

public class Serializer
{
    private static FNameMapData SerializeFNameMap(List<FNameEntrySerialized> FNameEntrysSerialized, int LastIndex, byte[] originalAssetBytes) // this is only ever going to be used by this so lets just make this private. :shrug:
    {
        FNameMapData FNameMapData = new(); // probably a trash way of doing this but it's most likely gonna be changed in the future.
        FNameMapData.lengths = new();
        FNameMapData.hashes = new();
        uint bytesTheNameMapTakesUp = 0;
        foreach (var Name in FNameEntrysSerialized)
            bytesTheNameMapTakesUp += (uint)Name.Name.Length;
        FNameMapData.bytesToTakeUp = bytesTheNameMapTakesUp;

        FNameMapData.hash = 3244556288; // Ig it's kinda just a hash idrk though just guessing.

        for (int i = 0; i < FNameEntrysSerialized.Count; i++)
        {
            //int HashStart = 44;
            //HashStart += originalAssetBytes[44] * 8;
            /*if (i == LastIndex)
            {
                
            }
            else if (i == LastIndex - 1)
            {
                
            }
            else
            {
                
            }*/
            FNameMapData.hashes.Add(To32BitFnv1aHash(FNameEntrysSerialized[i].Name.ToLower()));
            FNameMapData.hashes.Add(0);
            FNameMapData.lengths.Add(new byte[] { 0x0, Convert.ToByte(FNameEntrysSerialized[i].Name.Length)});
        }
        FNameMapData.count = (uint)FNameEntrysSerialized.Count;
        
        return FNameMapData;
    }

    private static List<ulong> getHashes(string assetPath, List<FNameEntrySerialized> names)
    {
        IoPackage package = null;
        Task.Run(async () =>
        {
            package = (IoPackage)Provider.provider.LoadPackage(assetPath);
        }).Wait();
        List<ulong> hashes = new();
        byte[] ogBytes = SaveAssetBytes(assetPath);
        FZenPackageSummary ogSummary = Reader.ReadStruct<FZenPackageSummary>(ogBytes, 0);

        int hashStart = ogBytes[44] * 8 + 44;

        for (int i = 0; i < names.Count; i++)
        {
            if (i == ogSummary.Name.NameIndex)
                hashes.Add(BitConverter.ToUInt64(StructWriter.Read(ogBytes, hashStart + 8, 8)));
            else if (i == ogSummary.Name.NameIndex - 1)
                hashes.Add(BitConverter.ToUInt64(StructWriter.Read(ogBytes, hashStart, 8)));
            else
                hashes.Add(To32BitFnv1aHash(names[i].Name.ToLower())); // hash ig
        }
        

        return hashes;
    }
    
    public static byte[] SerializeExportEntry(FExportMapEntry ExportMap)
    {
        StructWriter SW = new();
        
        SW.WriteStruct(ExportMap.CookedSerialOffset);
        SW.WriteStruct(ExportMap.CookedSerialSize);
        SW.WriteStruct(ExportMap.ObjectName._nameIndex);
        SW.WriteStruct(ExportMap.ObjectName.ExtraIndex);
        SW.WriteStruct(ExportMap.OuterIndex.TypeAndId);
        SW.WriteStruct(ExportMap.ClassIndex.TypeAndId);
        SW.WriteStruct(ExportMap.SuperIndex.TypeAndId);
        SW.WriteStruct(ExportMap.TemplateIndex.TypeAndId);
        SW.WriteStruct(ExportMap.PublicExportHash);
        SW.WriteStruct(ExportMap.ObjectFlags);
        SW.WriteStruct(ExportMap.FilterFlags);
        SW.Write(new byte[] { 0, 0, 0 });

        return SW.WrittenBytes.ToArray();
    }
    
    public static byte[] SerializeAsset(string assetPath, AssetData assetData)
    {
        StructWriter SW = new();
        IoPackage package = null;
        Task.Run(async () =>
        {
            package = (IoPackage)Provider.provider.LoadPackage(assetPath);
        }).Wait();

        List<FNameEntrySerialized> Names = package.NameMap.ToList().ConvertAll(x => new FNameEntrySerialized()
        {
            Name = x.Name
        });
        
        foreach (var name in assetData.NameMap)
        {
            Names.Add(new FNameEntrySerialized()
            {
                Name = name
            });
        }

        byte[] ogBytes = SaveAssetBytes(assetPath);
        FZenPackageSummary ogSummary = Reader.ReadStruct<FZenPackageSummary>(ogBytes, 0);

        /*byte[] properties = new byte[ogBytes.Length - ogSummary.HeaderSize];
        Buffer.BlockCopy(ogBytes, (int)ogSummary.HeaderSize, properties, 0, properties.Length);*/

        assetData.Summary = ogSummary; // not done with serialization so we're not doing to much like recreating the summary yet, atleast not until all header data is being written.
        
        if (assetData.Properties != null)
        {
            for (int i = 0; i < package.ExportMap.Length; i++) // add needed properties to the properties list.
            {
                int indexOfDic = assetData.Properties.Keys.ToList()
                    .FindIndex(x => x == package.ExportsLazy[i].Value.Name);

                if (indexOfDic != -1)
                {
                    package.ExportsLazy[i].Value.Properties.AddRange(assetData.Properties.ElementAt(indexOfDic).Value);
                    foreach (var Value in assetData.Properties.ElementAt(indexOfDic).Value)
                    {
                        string path = ((FSoftObjectPath)Value.Tag.GenericValue).AssetPathName.Text;
                        int index = Names.FindIndex(x => x.Name == path.Split(".")[0]);
                        if (index < 0)
                        {
                            Names.Add(new FNameEntrySerialized()
                            {
                                Name = path.Split(".")[0]
                            });
                            Names.Add(new FNameEntrySerialized()
                            {
                                Name = path.Split(".")[1]
                            });
                        }
                    }
                }
            }
        }
        
        assetData.NameMapData = SerializeFNameMap(Names, assetData.NameMap.Count, new byte[0]);
        
        //SW.WriteStruct(ogSummary);
        FNameBlankData FNameBlankData = new FNameBlankData()
        {
            hash = assetData.NameMapData.hash,
            count = assetData.NameMapData.count,
            bytesToTakeUp = assetData.NameMapData.bytesToTakeUp
        };
        SW.WriteStruct(FNameBlankData);

        SW.WriteList(getHashes(assetPath, Names));
        
        foreach(var name in Names)
            SW.Write(new byte[] { 0x0, Convert.ToByte(name.Name.Length) });
            
        SW.WriteList((Names.Select(x => x.Name).ToList()));

        if (provider.Versions.Game >= EGame.GAME_UE5_2)
            SW.WriteStruct((ulong)0);

        int ImportedPublicExportHashesOffset = SW.WrittenBytes.Count + 44;
        SW.Write(new byte[] { 0 }); // hashes here are not needed so we don't have to write them.

        int ImportMapOffset = SW.WrittenBytes.Count + 44;
        SW.WriteList(package.ImportMap.Select(x => x.TypeAndId).ToList());
        
        int ExportMapOffset = SW.WrittenBytes.Count + 44;
        int ExportMapSize = 72 * package.ExportMap.Length;

        int ExportBundleEntriesOffset = SW.WrittenBytes.Count + 44 + ExportMapSize;
        SW.WriteList(package.ExportBundleEntries.ToList());
        
        int GraphDataOffset = SW.WrittenBytes.Count + 44 + ExportMapSize;
        SW.WriteList(package.ExportBundleHeaders.ToList());
        
        uint HeaderSize = (uint)SW.WrittenBytes.Count + 44 + (uint)ExportMapSize;

        uint CookedHeaderSize = ogSummary.CookedHeaderSize;
        if (HeaderSize > ogSummary.HeaderSize)
            CookedHeaderSize += HeaderSize - ogSummary.HeaderSize;
        else
            CookedHeaderSize -= ogSummary.HeaderSize - HeaderSize;

        
        ulong SerialOffset = 0;
        for(int i = 0; i < package.ExportMap.Length; i++)
        {
            int offset = ogSummary.ExportMapOffset;
            offset += 72 * i;
            FExportMapEntry ogEntry = Reader.ReadStruct<FExportMapEntry>(ogBytes, offset);

            ulong CookedSerialOffset = CookedHeaderSize + SerialOffset;

            byte[] PropertySize = new PropertySerializer(package.ExportsLazy[i].Value.ExportType, provider.MappingsForGame,
                package.ExportsLazy[i].Value.Properties).Serialize(Names);
            
            ulong CookedSerialSize = package.ExportMap[i].CookedSerialSize;
            if (package.ExportMap[i].CookedSerialSize > (ulong)PropertySize.Length)
                CookedSerialSize -= package.ExportMap[i].CookedSerialSize - (ulong)PropertySize.Length;
            else
                CookedSerialSize += (ulong)PropertySize.Length - package.ExportMap[i].CookedSerialSize;

            SerialOffset += CookedSerialSize;
            
            FExportMapEntry Entry = ogEntry with
            {
                CookedSerialOffset = CookedSerialOffset,
                CookedSerialSize = CookedSerialSize
            };
            SW.Insert(SerializeExportEntry(Entry), ExportMapOffset + 72 * i - 44);
        }
        
        // summary part
        FZenPackageSummary Summary = new FZenPackageSummary()
        {
            bHasVersioningInfo = 0,
            HeaderSize = HeaderSize,
            Name = ogSummary.Name,
            PackageFlags = ogSummary.PackageFlags,
            CookedHeaderSize = CookedHeaderSize,
            ImportedPublicExportHashesOffset = ImportedPublicExportHashesOffset,
            ImportMapOffset = ImportMapOffset,
            ExportMapOffset = ExportMapOffset,
            ExportBundleEntriesOffset = ExportBundleEntriesOffset,
            GraphDataOffset = GraphDataOffset
        };
        SW.InsertStruct(Summary,0);

        
        // properties

        for (int i = 0; i < package.ExportMap.Length; i++)
        {
            var index = package.ExportBundleEntries.ToList().FindIndex(x => x.LocalExportIndex == i && x.CommandType == EExportCommandType.ExportCommandType_Create);

            List<byte> buffer = new();
            SW.Write(new PropertySerializer(package.ExportsLazy[index].Value.ExportType, provider.MappingsForGame,
                package.ExportsLazy[index].Value.Properties).Serialize(Names));
        }
        
        return SW.WrittenBytes.ToArray();
    }
    
    private static uint To32BitFnv1aHash(string toHash,
        bool separateUpperByte = false)
    {
        IEnumerable<byte> bytesToHash;

        if (separateUpperByte)
            bytesToHash = toHash.ToCharArray()
                .Select(c => new[] { (byte)((c - (byte)c) >> 8), (byte)c })
                .SelectMany(c => c);
        else
            bytesToHash = toHash.ToCharArray()
                .Select(Convert.ToByte);

        //this is the actual hash function; very simple
        uint hash = FnvConstants.FnvOffset32;

        foreach (var chunk in bytesToHash)
        {
            hash ^= chunk;
            hash *= FnvConstants.FnvPrime32;
        }

        return hash;
    }
    
    private static class FnvConstants
    {
        public static readonly uint FnvPrime32 = 16777619;
        public static readonly ulong FnvPrime64 = 1099511628211;
        public static readonly uint FnvOffset32 = 2166136261;
        public static readonly ulong FnvOffset64 = 14695981039346656037;
    }
}