#if UNITY_EDITOR
using UnityEditor;

namespace VuonVietKyThu.Editor {
    public sealed class ArtImportProcessor : AssetPostprocessor {
        void OnPreprocessTexture(){
            if(!assetPath.Contains("/Resources/Art/")) return;

            var ti=(TextureImporter)assetImporter;
            bool transparent=assetPath.Contains("/Fruits/") || assetPath.Contains("/Magic/") || assetPath.Contains("/Characters/");
            bool smallPiece=assetPath.Contains("/Fruits/") || assetPath.Contains("/Magic/");
            int maxSize=smallPiece ? 512 : 2048;

            ti.mipmapEnabled=false;
            ti.npotScale=TextureImporterNPOTScale.None;
            ti.textureCompression=TextureImporterCompression.CompressedHQ;
            ti.maxTextureSize=maxSize;
            ti.alphaIsTransparency=transparent;

            var android=ti.GetPlatformTextureSettings("Android");
            android.overridden=true;
            android.maxTextureSize=maxSize;
            android.format=transparent ? TextureImporterFormat.ETC2_RGBA8 : TextureImporterFormat.ETC2_RGB4;
            android.compressionQuality=85;
            ti.SetPlatformTextureSettings(android);
        }
    }
}
#endif
