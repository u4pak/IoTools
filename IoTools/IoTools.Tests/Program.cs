﻿using IoTools.Providers;
using IoTools.Serialization;
using IoTools.StructData;
using IoTools.Tests;

// create a provider

var Output = new Output(@"C:\Users\anker\OneDrive\Desktop\Fortnite\FortniteGame\Content\Paks", @"C:\Users\anker\OneDrive\Desktop\Mappings\++Fortnite+Release-23.50-CL-24376996-Windows.usmap");
Provider.provider = Output.FProvider;

List<string> NameMap = new();
NameMap.Add("/Game/");

File.WriteAllBytes(@"C:\Users\anker\OneDrive\Documents\IoTools\test.txt", Serializer.SerializeAsset("/Game/Athena/Heroes/Meshes/Bodies/CP_028_Athena_Body", new AssetData()
{
    NameMap = NameMap
}, File.ReadAllBytes(@"C:\Users\anker\OneDrive\Desktop\Output\Exports\CP_028_Athena_Body.uasset")));