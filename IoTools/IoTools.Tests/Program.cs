using IoTools.Readers;
using IoTools.Serialization;
using IoTools.StructData;

List<FNameEntrySerialized> NameMap = new();

NameMap.Add(new FNameEntrySerialized()
{
    name = "/Game/"
});

File.WriteAllBytes(@"C:\Users\anker\OneDrive\Documents\IoTools\test.txt", Serializer.SerializeAsset(new AssetData()
{
    NameMap = NameMap
}, File.ReadAllBytes(@"C:\Users\anker\OneDrive\Desktop\Output\Exports\CP_028_Athena_Body.uasset")));

/*byte[] bytes = File.ReadAllBytes(@"C:\Users\anker\OneDrive\Documents\IoTools\test.txt");

AssetData data = Reader.ReadStruct<AssetData>(bytes, 0);

if (bytes.Length != 0)
{
    
}*/