using UnityEditor;
using UnityEngine;

// Todo PNG em Resources/Sprites vira Sprite automaticamente (pixel art/neon sem filtro borrado)
public class SpriteImporter : AssetPostprocessor
{
    // PCM evita depender do ffmpeg para converter o audio no build WebGL
    void OnPreprocessAudio()
    {
        if (!assetPath.Contains("Resources/Audio/"))
        {
            return;
        }
        AudioImporter importer = (AudioImporter)assetImporter;
        AudioImporterSampleSettings config = importer.defaultSampleSettings;
        config.compressionFormat = AudioCompressionFormat.PCM;
        config.loadType = AudioClipLoadType.DecompressOnLoad;
        importer.defaultSampleSettings = config;
        importer.SetOverrideSampleSettings("WebGL", config);
        importer.forceToMono = true;
    }

    void OnPreprocessTexture()
    {
        if (!assetPath.Contains("Resources/Sprites/"))
        {
            return;
        }

        TextureImporter importer = (TextureImporter)assetImporter;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.mipmapEnabled = false;
        importer.filterMode = FilterMode.Bilinear;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.alphaIsTransparency = true;
        importer.maxTextureSize = 2048;

        string nome = System.IO.Path.GetFileNameWithoutExtension(assetPath);
        float ppu = 64f;
        if (nome == "pixel")
        {
            ppu = 4f;
        }
        else if (nome == "background")
        {
            ppu = 32f;
        }
        else if (nome == "menu_bg")
        {
            ppu = 100f;
        }
        importer.spritePixelsPerUnit = ppu;
    }
}
