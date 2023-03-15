# IoTools

C# Open-source class library for turning deserialized IO Store asset data into Serialized Io Store asset data.

#### Serialization
<Serialization>
  It's super simple to serialize your an asset Example:
```csharp
  File.WriteAllBytes(@"C:\Users\anker\OneDrive\Documents\IoTools\test.txt", Serializer.SerializeAsset(new AssetData()
  {
    NameMap = NameMap,
    ExportMaps = ExportMaps
  }, File.ReadAllBytes(@"C:\Users\anker\OneDrive\Desktop\Output\Exports\CP_028_Athena_Body.uasset")));
 ```
</Serialization>
