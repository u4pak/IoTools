using IoTools.StructData;
using IoTools.Writers;

namespace IoTools.Serialization;

public class Serializer
{
    private static FNameMapData SerializeFNameMap(List<FNameEntrySerialized> FNameEntrysSerialized, int LastIndex, byte[] originalAssetBytes) // this is only ever going to be used by this so lets just make this private. :shrug:
    {
        FNameMapData FNameMapData = new(); // probably a trash way of doing this but it's most likely gonna be changed in the future.
        
        uint bytesTheNameMapTakesUp = 0;
        foreach (var Name in FNameEntrysSerialized)
            bytesTheNameMapTakesUp += (uint)Name.name.Length;
        FNameMapData.bytesToTakeUp = bytesTheNameMapTakesUp;

        FNameMapData.hash = 3244556288; // Ig it's kinda just a hash idrk though just guessing.

        for (int i = 0; i < FNameEntrysSerialized.Count; i++)
        {
            int HashStart = 44;
            HashStart += originalAssetBytes[44] * 8;
            /*if (i == LastIndex)
            {
                
            }
            else if (i == LastIndex - 1)
            {
                
            }
            else
            {
                
            }*/
            FNameMapData.hashes.Add(To32BitFnv1aHash(FNameEntrysSerialized[i].name.ToLower()));
            FNameMapData.hashes.Add(0);
            FNameMapData.lengths.Add(new byte[] { 0x0, Convert.ToByte(FNameEntrysSerialized[i].name.Length)});
        }
        FNameMapData.count = (uint)FNameEntrysSerialized.Count;
        
        return FNameMapData;
    }
    
    public static byte[] SerializeAsset(AssetData assetData, byte[] originalAssetBytes)
    {
        StructWriter SW = new();
        assetData.NameMapData = SerializeFNameMap(assetData.NameMap, assetData.NameMap.Count, new byte[0]);

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