# IoTools

C# Open-source class library for turning deserialized IO Store asset data into Serialized Io Store asset data.

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

-- [Anker#0853]
