using CUE4Parse.UE4;
using CUE4Parse.UE4.Assets.Objects;
using IoTools.StructData;
using static IoTools.Providers.Provider;

namespace PropertyEditor.Core.UE;

public class ArrayPropertySerializer
{
    private List<byte> _bytes = new();

    public ArrayPropertySerializer(UScriptArray array, List<FNameEntrySerialized> names)
    {
        _bytes.AddRange(BitConverter.GetBytes(array.Properties.Count));
        foreach (var prop in array.Properties)
            _bytes.AddRange(PropertySerializer.SerializeProperty(provider.MappingsForGame,
                array.InnerType, prop, array.InnerTagData?.StructType, names));
    }

    public byte[] GetBytes() => _bytes.ToArray();
}