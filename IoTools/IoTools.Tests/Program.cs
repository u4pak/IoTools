using IoTools.Serialization;
using IoTools.StructData;

File.WriteAllBytes(@"C:\Users\anker\OneDrive\Documents\IoTools\test.txt", Serializer.SerializeAsset(new AssetData()
{
    
}, new byte[0]));