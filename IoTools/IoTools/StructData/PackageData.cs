using CUE4Parse.UE4.IO.Objects;
using CUE4Parse.UE4.Objects.UObject;

namespace IoTools.StructData;

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
