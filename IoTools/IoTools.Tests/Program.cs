using CUE4Parse.UE4.Assets;
using CUE4Parse.UE4.Assets.Objects;
using IoTools.Providers;
using IoTools.Serialization;
using IoTools.StructData;
using IoTools.Tests;

// create a provider

var Output = new Output(@"C:\Users\anker\OneDrive\Desktop\Fortnite\FortniteGame\Content\Paks", @"C:\Users\anker\OneDrive\Desktop\Mappings\++Fortnite+Release-24.00-CL-24518431-Android_oo.usmap");
Provider.provider = Output.FProvider;

/*IoPackage package1 =
    (IoPackage)Output.FProvider.LoadPackage("/Game/Athena/Heroes/Meshes/Bodies/CP_Athena_Body_M_BasilStrong");
IoPackage package2 =
    (IoPackage)Output.FProvider.LoadPackage("FortniteGame/Content/Athena/Heroes/Meshes/Bodies/CP_015_Athena_Body");

Dictionary<string, List<FPropertyTag>> Properties = new();
List<FPropertyTag> Tags = new();

int IndexofProperty = package2.ExportsLazy[1].Value.Properties.FindIndex(x => x.Name == "MaterialOverrides");
Tags.Add(package2.ExportsLazy[1].Value.Properties[IndexofProperty]);

Properties.Add(package1.ExportsLazy[1].Value.Name, Tags);*/

List<string> NameMap = new();
NameMap.Add("/Game/");

File.WriteAllBytes(@"C:\Users\anker\OneDrive\Documents\IoTools\test.txt", Serializer.SerializeAsset("/Game/Characters/CharacterParts/Backpacks/CP_Backpack_Bites", new AssetData()
{
    NameMap = NameMap
}));