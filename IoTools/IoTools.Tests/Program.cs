using CUE4Parse.UE4.Assets;
using IoTools.Serialization;
using IoTools.StructData;
using IoTools.Tests;

// create a provider

var Output = new Output(@"C:\Users\anker\OneDrive\Desktop\Fortnite\FortniteGame\Content\Paks", @"C:\Users\anker\OneDrive\Desktop\Mappings\++Fortnite+Release-23.50-CL-24376996-Windows.usmap");

IoPackage Package = (IoPackage)Output.FProvider.LoadPackage("/Game/Athena/Heroes/Meshes/Bodies/CP_028_Athena_Body");


List<string> NameMap = new();
NameMap.Add("/Game/");

List<FExportMapEntry> ExportMaps = new();
foreach (var exportMap in Package.ExportMap)
{
    ExportMaps.Add(new FExportMapEntry()
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
}

File.WriteAllBytes(@"C:\Users\anker\OneDrive\Documents\IoTools\test.txt", Serializer.SerializeAsset(new AssetData()
{
    NameMap = NameMap,
    ExportMaps = ExportMaps
}, File.ReadAllBytes(@"C:\Users\anker\OneDrive\Desktop\Output\Exports\CP_028_Athena_Body.uasset")));

/*byte[] bytes = File.ReadAllBytes(@"C:\Users\anker\OneDrive\Documents\IoTools\test.txt");

AssetData data = Reader.ReadStruct<AssetData>(bytes, 0);

if (bytes.Length != 0)
{
    
}*/