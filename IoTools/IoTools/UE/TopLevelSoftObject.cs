using IoTools.Providers;
using IoTools.StructData;

namespace PropertyEditor.Core.UE;

public class TopLevelSoftObject
{
    private List<byte> _bytes = new();
    
    public TopLevelSoftObject(List<FNameEntrySerialized> nameMap, string name)
    {
        if (string.IsNullOrEmpty(name))
            name = "None.None";
        
        var strs = name.Split('.');
        var index = nameMap.FindIndex(x => x.Name == strs[0]);
        var extraIndex = strs[1] != "None" ? nameMap.FindIndex(x => x.Name == strs[1]) : 0;
        
        if (index == -1)
        {
            index = nameMap.Count;
            nameMap.Add(new FNameEntrySerialized()
            {
                Name = strs[0]
            });
        }
        
        if (extraIndex == -1)
        {
            extraIndex = nameMap.Count;
            nameMap.Add(new FNameEntrySerialized()
            {
                Name = strs[1]
            });
        }
        
        _bytes.AddRange(BitConverter.GetBytes(index));
        _bytes.AddRange(BitConverter.GetBytes(0));
        _bytes.AddRange(BitConverter.GetBytes(extraIndex));
        _bytes.AddRange(BitConverter.GetBytes(0));
        _bytes.AddRange(BitConverter.GetBytes(0));
    }

    public byte[] GetBytes() => _bytes.ToArray();
}