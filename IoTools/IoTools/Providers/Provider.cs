using CUE4Parse.FileProvider;

namespace IoTools.Providers;

public class Provider
{
    public static DefaultFileProvider provider = null;
    
    public static byte[] SaveAssetBytes(String UassetPath)
    {
        return provider.SaveAsset(UassetPath);
    }
}