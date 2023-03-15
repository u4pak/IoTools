# IoTools

C# Open-source class library for turning deserialized IO Store asset data into Serialized Io Store asset data.

#### Serialization
<details>
<summary>Serialization</summary>
  It's super simple to serialize your an asset Example:
  ```csharp
  File.WriteAllBytes({path to write at}, Serializer.SerializeAsset(new AssetData()
  {
    NameMap = NameMap, // new name map of the asset
    ExportMaps = ExportMaps // export maps
  }, {Your original assets bytes here});
  ```
</details>
