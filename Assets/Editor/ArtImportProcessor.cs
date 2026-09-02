#if UNITY_EDITOR
using UnityEditor;
namespace VuonVietKyThu.Editor {
    public sealed class ArtImportProcessor : AssetPostprocessor {
        void OnPreprocessTexture(){
            if(!assetPath.Contains("/Resources/Art/"))return;var ti=(TextureImporter)assetImporter;ti.mipmapEnabled=false;ti.textureCompression=TextureImporterCompression.Compressed;ti.maxTextureSize=1024;ti.alphaIsTransparency=assetPath.Contains("/Fruits/")||assetPath.Contains("/Magic/")||assetPath.Contains("/Characters/");
            var android=ti.GetPlatformTextureSettings("Android");android.overridden=true;android.maxTextureSize=1024;android.format=TextureImporterFormat.ASTC_6x6;android.compressionQuality=70;ti.SetPlatformTextureSettings(android);
        }
    }
}
#endif
