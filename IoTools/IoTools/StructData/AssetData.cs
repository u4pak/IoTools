﻿using CUE4Parse.UE4.IO.Objects;

namespace IoTools.StructData;

public struct AssetData
{
    public FZenPackageSummary Summary;
    public FNameMapData NameMapData;
    public List<FNameEntrySerialized> NameMap;
    public List<FExportMapEntry> ExportMaps;
}