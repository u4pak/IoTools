

using IoTools.Providers;
using IoTools.StructData;

namespace PropertyEditor.Core.UE;

public class NamePropertySerializer
{
    private List<byte> _bytes = new();

    public NamePropertySerializer(List<FNameEntrySerialized> nameMap, string name)
    {
        var index = nameMap.FindIndex(x => x.Name == name);
        var extraIndex = nameMap.FindIndex(x => x.Name == "None");
        
        if (index == -1)
        {
            index = nameMap.Count;
            nameMap.Add(new FNameEntrySerialized()
            {
                Name = name
            });
        }
        
        if (extraIndex == -1)
        {
            extraIndex = nameMap.Count;
            nameMap.Add(new FNameEntrySerialized()
            {
                Name = "None"
            });
        }
        
        _bytes.AddRange(BitConverter.GetBytes(index));
        _bytes.AddRange(BitConverter.GetBytes(extraIndex));
    }
    
    public byte[] GetBytes() => _bytes.ToArray();
}