# IoTools

C# Open-source class library for turning deserialized IO Store asset data into Serialized Io Store asset data that can be used for modding.

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

  File.WriteAllBytes({path to write at}, Serializer.SerializeAsset(new AssetData()
  {
    NameMap = NameMap, // new name map of the asset
    ExportMaps = ExportMaps // export maps
  }, {Your original assets bytes here});
```


## Contributing

You're always welcome to make a pull request for any contribution you feel will make the project better.

## Creators

- [Anker#0853](https://github.com/OngAnker)
