using UnityEngine;
using System.Collections;
using System;

public static class TextureGenerator {

	public static Texture2D TextureFromColourMap(Color[] colourMap, int width, int height) {
		Texture2D texture = new Texture2D (width, height);
		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.SetPixels (colourMap);
		texture.Apply ();
		return texture;
	}


	public static Texture2D TextureFromHeightMap(HeightMap heightMap) {
		int width = heightMap.values.GetLength (0);
		int height = heightMap.values.GetLength (1);

		Color[] colourMap = new Color[width * height];
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				colourMap [y * width + x] = Color.Lerp (Color.black, Color.white, Mathf.InverseLerp(heightMap.minValue,heightMap.maxValue,heightMap.values [x, y]));
			}
		}

		return TextureFromColourMap (colourMap, width, height);
	}

    public static Texture2D MakeTextureOfColour(int size, Color colour) {
        Color[] colourMap = new Color[size * size];
        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                colourMap[y * size + x] = colour;
            }

        }

        return TextureFromColourMap(colourMap, size, size);
    }

    internal static Texture MakeTextureOfColour(object v, Color green) {
        throw new NotImplementedException();
    }
}
