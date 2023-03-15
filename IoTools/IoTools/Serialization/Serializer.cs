using CUE4Parse.UE4.Assets;
using IoTools.Providers;
using IoTools.Readers;
using IoTools.StructData;
using IoTools.Writers;

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
    
    public static byte[] SerializeAsset(string assetPath, AssetData assetData, byte[] originalAssetBytes)
    {
        StructWriter SW = new();
        IoPackage package = null;
        Task.Run(async () =>
        {
            package = (IoPackage)Provider.provider.LoadPackage(assetPath);
        }).Wait();

        assetData.NameMapData = SerializeFNameMap(assetData.NameMap, assetData.NameMap.Count, new byte[0]);

        FZenPackageSummary ogSummary = Reader.ReadStruct<FZenPackageSummary>(originalAssetBytes, 0);
        byte[] properties = new byte[originalAssetBytes.Length - ogSummary.HeaderSize];
        Buffer.BlockCopy(originalAssetBytes, (int)ogSummary.HeaderSize, properties, 0, properties.Length);

        assetData.Summary = ogSummary; // not done with serialization so we're not doing to much like recreating the summary yet, atleast not until all header data is being written.
    
        //SW.WriteStruct(ogSummary);
        FNameBlankData FNameBlankData = new FNameBlankData()
        {
            hash = assetData.NameMapData.hash,
            count = assetData.NameMapData.count,
            bytesToTakeUp = assetData.NameMapData.bytesToTakeUp
        };
        SW.WriteStruct(FNameBlankData);

        SW.WriteList(assetData.NameMapData.hashes);
        foreach(var length in assetData.NameMapData.lengths)
            SW.Write(length);
        SW.WriteList(assetData.NameMap);

        int ImportedPublicExportHashesOffset = SW.WrittenBytes.Count + 44;
        SW.Write(new byte[] { 0x0 }); // hashes here are not needed so we don't have to write them.

        int ExportMapOffset = SW.WrittenBytes.Count + 44;
        foreach (var exportMap in package.ExportMap)
            SW.WriteStruct(new FExportMapEntry()
            {
                CookedSerialOffset = exportMap.CookedSerialOffset,
                CookedSerialSize = exportMap.CookedSerialSize,
                ObjectName = exportMap.ObjectName,
                OuterIndex = exportMap.OuterIndex,
                ClassIndex = exportMap.ClassIndex,
                SuperIndex = exportMap.SuperIndex,
                TemplateIndex = exportMap.TemplateIndex,
                PublicExportHash = exportMap.PublicExportHash,
                ObjectFlags = exportMap.ObjectFlags,
                FilterFlags = exportMap.FilterFlags
            });

        uint HeaderSize = (uint)SW.WrittenBytes.Count + 44;

        uint CookedHeaderSize = ogSummary.CookedHeaderSize;
        if (HeaderSize > ogSummary.HeaderSize)
            CookedHeaderSize += HeaderSize - ogSummary.HeaderSize;
        else
            CookedHeaderSize -= ogSummary.HeaderSize - HeaderSize;

                // summary part
        FZenPackageSummary Summary = new FZenPackageSummary()
        {
            HeaderSize = HeaderSize,
            Name = ogSummary.Name,
            PackageFlags = ogSummary.PackageFlags,
            CookedHeaderSize = CookedHeaderSize,
            ImportedPublicExportHashesOffset = ImportedPublicExportHashesOffset,
            ImportMapOffset = ImportedPublicExportHashesOffset,
            ExportMapOffset = ExportMapOffset,
            
        };
        
        SW.Write(properties); // couldn't be asked serializing properties atm.
        
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