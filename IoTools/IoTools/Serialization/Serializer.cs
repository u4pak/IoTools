using CUE4Parse.UE4.Assets;
using IoTools.Providers;
using IoTools.Readers;
using IoTools.StructData;
using IoTools.Writers;
using PropertyEditor.Core;
using static IoTools.Providers.Provider;

namespace IoTools.Serialization;

public class Serializer
{
    private static FNameMapData SerializeFNameMap(List<string> FNameEntrysSerialized, int LastIndex, byte[] originalAssetBytes) // this is only ever going to be used by this so lets just make this private. :shrug:
    {
        FNameMapData FNameMapData = new(); // probably a trash way of doing this but it's most likely gonna be changed in the future.
        FNameMapData.lengths = new();
        FNameMapData.hashes = new();
        uint bytesTheNameMapTakesUp = 0;
        foreach (var Name in FNameEntrysSerialized)
            bytesTheNameMapTakesUp += (uint)Name.Length;
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
            FNameMapData.hashes.Add(To32BitFnv1aHash(FNameEntrysSerialized[i].ToLower()));
            FNameMapData.hashes.Add(0);
            FNameMapData.lengths.Add(new byte[] { 0x0, Convert.ToByte(FNameEntrysSerialized[i].Length)});
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
    
    public static byte[] SerializeAsset(string assetPath, AssetData assetData)
    {
        StructWriter SW = new();
        IoPackage package = null;
        Task.Run(async () =>
        {
            package = (IoPackage)Provider.provider.LoadPackage(assetPath);
        }).Wait();

        assetData.NameMapData = SerializeFNameMap(assetData.NameMap, assetData.NameMap.Count, new byte[0]);
        
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
        byte[] ExportBundleEntries = new byte[ogSummary.GraphDataOffset - ogSummary.ExportBundleEntriesOffset];
        byte[] GraphData = new byte[ogSummary.HeaderSize - ogSummary.GraphDataOffset];
        Buffer.BlockCopy(ogBytes, ogSummary.ExportBundleEntriesOffset, ExportBundleEntries, 0, ExportBundleEntries.Length); // this is temporary.
        Buffer.BlockCopy(ogBytes, ogSummary.GraphDataOffset, GraphData, 0, GraphData.Length); // this is temporary.
        
        /*byte[] properties = new byte[ogBytes.Length - ogSummary.HeaderSize];
        Buffer.BlockCopy(ogBytes, (int)ogSummary.HeaderSize, properties, 0, properties.Length);*/

        assetData.Summary = ogSummary; // not done with serialization so we're not doing to much like recreating the summary yet, atleast not until all header data is being written.
    
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

        int ImportedPublicExportHashesOffset = SW.WrittenBytes.Count + 44;
        SW.Write(new byte[] { 0x0 }); // hashes here are not needed so we don't have to write them.

        int ExportMapOffset = SW.WrittenBytes.Count + 44;
        for(int i = 0; i < package.ExportMap.Length; i++)
        {
            int offset = ogSummary.ExportMapOffset;
            offset += 72 * i;
            FExportMapEntry ogEntry = Reader.ReadStruct<FExportMapEntry>(ogBytes, offset);

            ulong CookedSerialOffset = package.ExportMap[i].CookedSerialOffset;
            if (ExportMapOffset > ogSummary.ExportMapOffset)
                CookedSerialOffset += (ulong)ExportMapOffset - (ulong)ogSummary.ExportMapOffset;
            else
                CookedSerialOffset -= (ulong)ogSummary.ExportMapOffset - (ulong)ExportMapOffset;
            
            FExportMapEntry Entry = new FExportMapEntry()
            {
                CookedSerialOffset = CookedSerialOffset,
                CookedSerialSize = package.ExportMap[i].CookedSerialSize,
                ObjectName = package.ExportMap[i].ObjectName,
                OuterIndex = package.ExportMap[i].OuterIndex,
                ClassIndex = package.ExportMap[i].ClassIndex,
                SuperIndex = package.ExportMap[i].SuperIndex,
                TemplateIndex = package.ExportMap[i].TemplateIndex,
                PublicExportHash = package.ExportMap[i].PublicExportHash,
                ObjectFlags = package.ExportMap[i].ObjectFlags,
                FilterFlags = package.ExportMap[i].FilterFlags
            };
            SW.WriteStruct(Entry);
        }
            

        int ExportBundleEntriesOffset = SW.WrittenBytes.Count + 44;
        SW.Write(ExportBundleEntries);
        
        int GraphDataOffset = SW.WrittenBytes.Count + 44;
        SW.Write(GraphData);
        
        uint HeaderSize = (uint)SW.WrittenBytes.Count + 44;

        uint CookedHeaderSize = ogSummary.CookedHeaderSize;
        if (HeaderSize > ogSummary.HeaderSize)
            CookedHeaderSize += HeaderSize - ogSummary.HeaderSize;
        else
            CookedHeaderSize -= ogSummary.HeaderSize - HeaderSize;

        // summary part
        FZenPackageSummary Summary = new FZenPackageSummary()
        {
            bHasVersioningInfo = 0,
            HeaderSize = HeaderSize,
            Name = ogSummary.Name,
            PackageFlags = ogSummary.PackageFlags,
            CookedHeaderSize = CookedHeaderSize,
            ImportedPublicExportHashesOffset = ImportedPublicExportHashesOffset,
            ImportMapOffset = ImportedPublicExportHashesOffset,
            ExportMapOffset = ExportMapOffset,
            ExportBundleEntriesOffset = ExportBundleEntriesOffset,
            GraphDataOffset = GraphDataOffset
        };
        SW.InsertStruct(Summary,0);

        // properties

        for (int i = 0; i < package.ExportMap.Length; i++)
        {
            List<byte> buffer = new();
            SW.Write(new PropertySerializer(package.ExportsLazy[i].Value.ExportType, provider.MappingsForGame,
                package.ExportsLazy[i].Value.Properties).Serialize(Names));
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