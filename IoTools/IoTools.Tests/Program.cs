using CUE4Parse.UE4.IO.Objects;
using CUE4Parse.UE4.Objects.UObject;
using IoTools.Writers;
using FZenPackageSummary = IoTools.StructData.FZenPackageSummary;

StructWriter SW = new();

SW.WriteStruct<FZenPackageSummary>(new FZenPackageSummary()
{
    bHasVersioningInfo = 0,
    HeaderSize = 5,
    Name = new FMappedName()
    {
        _nameIndex = 1,
        ExtraIndex = 0
    },
    PackageFlags = EPackageFlags.PKG_None,
    CookedHeaderSize = 0,
    ImportedPublicExportHashesOffset = 0,
    ImportMapOffset = 0,
    ExportMapOffset = 0,
    ExportBundleEntriesOffset = 0,
    GraphDataOffset = 0
});

File.WriteAllBytes(@"C:\Users\anker\OneDrive\Documents\IoTools\test.txt", SW.WrittenBytes.ToArray());