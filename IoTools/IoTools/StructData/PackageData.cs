using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.IO.Objects;
using CUE4Parse.UE4.Objects.UObject;

namespace IoTools.StructData;

// put all structs for assets/packages here, This is to make sure that we know where all structs are and stuff doesn't get unorganized.

public struct FZenPackageSummary
{
    public uint bHasVersioningInfo;
    public uint HeaderSize;
    public FMappedName Name;
    public EPackageFlags PackageFlags;
    public uint CookedHeaderSize;
    public int ImportedPublicExportHashesOffset;
    public int ImportMapOffset;
    public int ExportMapOffset;
    public int ExportBundleEntriesOffset;
    public int GraphDataOffset;
}

public struct FNameEntrySerialized
{
    public string Name;
#if NAME_HASHES
        public readonly ushort NonCasePreservingHash;
        public readonly ushort CasePreservingHash;
#endif
}

public struct FNameMapData
{
    public uint count;
    public uint bytesToTakeUp;
    public ulong hash;
    public List<uint> hashes;
    public List<byte[]> lengths;
}

public struct FNameBlankData
{
    public uint count;
    public uint bytesToTakeUp;
    public ulong hash;
}

public struct FExportMapEntry
{
    public ulong CookedSerialOffset;
    public ulong CookedSerialSize;
    public FMappedName ObjectName;
    public FPackageObjectIndex OuterIndex;
    public FPackageObjectIndex ClassIndex;
    public FPackageObjectIndex SuperIndex;
    public FPackageObjectIndex TemplateIndex;
    public ulong PublicExportHash;
    public EObjectFlags ObjectFlags;
    public byte FilterFlags; // EExportFilterFlags: client/server flags
}

public struct FExportBundleEntry
{
    public uint LocalExportIndex;
    public EExportCommandType CommandType;
}


public struct FFragment {
    public const uint SkipMax = 127;
    public const uint ValueMax = 127;

    public const uint SkipNumMask = 0x007fu;
    public const uint HasZeroMask = 0x0080u;
    public const int ValueNumShift = 9;
    public const uint IsLastMask  = 0x0100u;
        
    public byte SkipNum; // Number of properties to skip before values
    public bool HasAnyZeroes;
    public byte ValueNum;  // Number of subsequent property values stored
    public bool IsLast; // Is this the last fragment of the header?
    
    public static ushort Pack(byte SkipNum, bool bHasAnyZeroes, byte ValueNum, bool bIsLast)
    {
        return (ushort)(SkipNum | (bHasAnyZeroes ? HasZeroMask : 0) | ValueNum << ValueNumShift |
                        (bIsLast ? IsLastMask : 0));
    }
}