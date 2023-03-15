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
    public string name;
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