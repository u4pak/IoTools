using IoTools.StructData;
using IoTools.Writers;

StructWriter SW = new();

SW.WriteStruct<AssetData>(new AssetData()
{
    
});

File.WriteAllBytes(@"C:\Users\anker\OneDrive\Documents\IoTools\test.txt", SW.WrittenBytes.ToArray());