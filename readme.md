# IoTools

Open-source class library for turning deserialized IO Store asset data into Serialized Io Store asset data that can be used for modding.

## Setup

#### Provider Linking

We utilize CUE4Parse so that we can export the IoPackage from the game you're attempting to mod, Since we don't want to create a provider just for IOTools you have to link your provider with IOTools, This is a simple process, Example Below.

```csharp
var Output = new Output(@"C:\Users\anker\OneDrive\Desktop\Fortnite\FortniteGame\Content\Paks", @"C:\Users\anker\OneDrive\Desktop\Mappings\++Fortnite+Release-23.50-CL-24376996-Windows.usmap"); // create our provider
Provider.provider = Output.FProvider; // set the provider IOTools uses to our new provider that our modding program uses.
```

## Usage

#### Serialization

Serialization is super simple below is a preview on how to serialize an asset

```csharp
  AssetData assetData = new AssetData(); // this is so we can control what names we add and what properties we add.
  File.WriteAllBytes({file path to write bytes in}, Serializer.SerializeAsset({asset path}, assetData);
```

#### Adding names to the name map

Adding names is one of the easiest ways to mod an asset example below.

```csharp
  List<string> NameMap = new();
  NameMap.Add("/Game/");

  AssetData assetData = new AssetData
  {
    NameMap = NameMap
  }; // this is so we can control what names we add and what properties we add.
  File.WriteAllBytes({file path to write bytes in}, Serializer.SerializeAsset({asset path}, assetData);
```

#### Adding properties to an asset

Adding properties is pretty simple, Below is a way of copying a property from 1 asset and putting it into ours.

```csharp
  IoPackage package1 =
    (IoPackage)Output.FProvider.LoadPackage("FortniteGame/Content/Athena/Heroes/Meshes/Bodies/CP_028_Athena_Body");
  IoPackage package2 =
    (IoPackage)Output.FProvider.LoadPackage("FortniteGame/Content/Athena/Heroes/Meshes/Bodies/CP_Athena_Body_F_Mochi");
    
  Dictionary<string, List<FPropertyTag>> Properties = new();
  List<FPropertyTag> Tags = new();

  int IndexofProperty = package2.ExportsLazy[1].Value.Properties.FindIndex(x => x.Name == "IdleEffectNiagara");
  Tags.Add(package2.ExportsLazy[1].Value.Properties[IndexofProperty]);
    
  Properties.Add(package1.ExportsLazy[1].Value.Name, Tags);
    
  AssetData assetData = new AssetData
  {
     Properties = Properties
  }; // this is so we can control what names and/or properties we add to the asset.
  File.WriteAllBytes({file path to write bytes in}, Serializer.SerializeAsset({asset path}, assetData);
```

## Contributing

You're always welcome to make a pull request for any contribution you feel will make the project better.

## Creators

- [Anker#0853](https://github.com/OngAnker)
- [owens#5889](https://github.com/owen-developer)
