using CUE4Parse.Encryption.Aes;
using CUE4Parse.FileProvider;
using CUE4Parse.MappingsProvider;
using CUE4Parse.UE4.Objects.Core.Misc;
using CUE4Parse.UE4.Versions;

namespace IoTools.Tests;

public class Output
{
    public static string FPaks;
    public DefaultFileProvider FProvider;
    public FAesKey FDefaultAES = new FAesKey("0x0000000000000000000000000000000000000000000000000000000000000000");
    
    public Output(string Fpaks, string FmanifestFile)
    {
        FPaks = Fpaks;
        FProvider = new DefaultFileProvider(Fpaks, SearchOption.TopDirectoryOnly, false, new VersionContainer(EGame.GAME_UE5_LATEST));
        FProvider.Initialize();
        FProvider.SubmitKey(new FGuid(), FDefaultAES);
        var UMappings = new FileUsmapTypeMappingsProvider(FmanifestFile);
        FProvider.MappingsContainer = UMappings;
    }
}