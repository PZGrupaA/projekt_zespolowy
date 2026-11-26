using UnityEditor;
using UnityEngine;

namespace Zaliczenie.EditorTools
{
    public class SpriteImport : AssetPostprocessor
    {
        void OnPreprocessTexture()
        {
            if (!assetPath.Replace('\\', '/').Contains("/Resources/Sprites/"))
                return;

            TextureImporter importer = (TextureImporter)assetImporter;
            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsToUnits = 1f;
            importer.filterMode = FilterMode.Point;
            importer.mipmapEnabled = false;
            importer.isReadable = true;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.npotScale = TextureImporterNPOTScale.None;
        }
    }
}
