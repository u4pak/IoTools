using CUE4Parse.UE4.Assets.Objects;

namespace IoTools.StructData;

public struct AssetData
{
    public FZenPackageSummary Summary;
    public FNameMapData NameMapData;
    public List<string> NameMap;
    public List<FPropertyTag> Properties;
}