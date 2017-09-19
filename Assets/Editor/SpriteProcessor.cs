using UnityEngine;
using System.Collections;
using UnityEditor;

public class SpriteProcessor : AssetPostprocessor {

    //Any time a texture is imported into project, this is called.
    //Store asset path in string, check if it contains "sprites", get ref to textureimporter, then set textureimporter to sprite
    void OnPostprocessTexture(Texture2D texture) {
        string lowerCaseAssetPath = assetPath.ToLower(); //assetPath is from AssetPostprocessor
        bool isInSpritesDirectory = (lowerCaseAssetPath.IndexOf("/sprites/") != -1); 

        if (isInSpritesDirectory) {
            TextureImporter textureImporter = (TextureImporter)assetImporter; //assetImporter is part of AssetPostprocessor
            textureImporter.textureType = TextureImporterType.Sprite;
        }
    }
}
