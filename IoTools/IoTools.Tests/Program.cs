using CUE4Parse.UE4.Assets;
using CUE4Parse.UE4.Assets.Objects;
using IoTools.Providers;
using IoTools.Serialization;
using IoTools.StructData;
using IoTools.Tests;

// create a provider

var Output = new Output(@"C:\Users\anker\OneDrive\Desktop\Fortnite\FortniteGame\Content\Paks", @"C:\Users\anker\OneDrive\Desktop\Mappings\++Fortnite+Release-24.00-CL-24518431-Android_oo.usmap");
Provider.provider = Output.FProvider;

IoPackage package1 =
    (IoPackage)Output.FProvider.LoadPackage("FortniteGame/Content/Athena/Heroes/Meshes/Bodies/CP_028_Athena_Body");
IoPackage package2 =
    (IoPackage)Output.FProvider.LoadPackage("FortniteGame/Content/Athena/Heroes/Meshes/Bodies/CP_Athena_Body_F_Mochi");

List<string> NameMap = new();
NameMap.Add("/Game/");

Dictionary<string, List<FPropertyTag>> Properties = new();
List<FPropertyTag> Tags = new();

int IndexofProperty = package2.ExportsLazy[1].Value.Properties.FindIndex(x => x.Name == "IdleEffectNiagara");
Tags.Add(package2.ExportsLazy[1].Value.Properties[IndexofProperty]);

Properties.Add(package1.ExportsLazy[1].Value.Name, Tags);

AssetData assetData = new AssetData
{
    Properties = Properties
}; // this is so we can control what names we add and what properties we add.

File.WriteAllBytes(@"C:\Users\anker\OneDrive\Documents\IoTools\test.txt", Serializer.SerializeAsset("/Game/Athena/Heroes/Meshes/Bodies/CP_028_Athena_Body", new AssetData()
{
    NameMap = NameMap,
    Properties = Properties
}));

