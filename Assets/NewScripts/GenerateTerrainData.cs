using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GenerateTerrainData {
    static float[,] falloffMap;
    public static float[,] GenerateTerrain(HeightMapSettings2 heightMapSettings, MeshSettings2 meshSettings, Vector2 sampleCentre) {

        float[,] heights = new float[meshSettings.resolution, meshSettings.resolution];
        heights = Noise2.GenerateNoiseMap(meshSettings.resolution, heightMapSettings.noiseSettings, sampleCentre);

        AnimationCurve heightCurve_threadsafe = new AnimationCurve(heightMapSettings.heightCurve.keys);


        if (heightMapSettings.useFalloff) {
            if (falloffMap == null) {
                falloffMap = FalloffGenerator.GenerateFalloffMap(meshSettings.resolution);
            }
        }

        for (int i = 0; i < meshSettings.resolution; i++) {
            for (int j = 0; j < meshSettings.resolution; j++) {
                heights[i, j] *= heightCurve_threadsafe.Evaluate(heights[i, j] - (heightMapSettings.useFalloff ? falloffMap[i, j] : 0)) * heightMapSettings.heightMultiplier;
            }
        }

        return heights;
    }


    public static TerrainData GenerateTerrainDataStuff(MeshSettings2 meshSettings, float[,] heights) {
        TerrainData terrainData = new TerrainData {
            heightmapResolution = meshSettings.resolution,
            alphamapResolution = meshSettings.resolution,
            size = new Vector3(meshSettings.width, meshSettings.depth, meshSettings.width)
        };

        terrainData.SetHeights(0, 0, heights);

        ApplyTextures(terrainData, meshSettings);

        return terrainData;
    }


    private static void ApplyTextures(TerrainData terrainData, MeshSettings2 meshSettings) {
        SplatPrototype flatSplat = new SplatPrototype();
        SplatPrototype steepSplat = new SplatPrototype();

        flatSplat.texture = meshSettings.flatTexture;
        steepSplat.texture = meshSettings.steepTexture;

        terrainData.splatPrototypes = new SplatPrototype[]
        {
        flatSplat,
        steepSplat
        };

        terrainData.RefreshPrototypes();

        var splatMap = new float[terrainData.alphamapResolution, terrainData.alphamapResolution, 2];

        for (int zRes = 0; zRes < terrainData.alphamapHeight; zRes++) {
            for (int xRes = 0; xRes < terrainData.alphamapWidth; xRes++) {
                float normalizedX = (float)xRes / (terrainData.alphamapWidth - 1);
                float normalizedZ = (float)zRes / (terrainData.alphamapHeight - 1);

                float steepness = terrainData.GetSteepness(normalizedX, normalizedZ);
                float steepnessNormalized = Mathf.Clamp(steepness / 1.5f, 0, 1f);

                splatMap[zRes, xRes, 0] = 1f - steepnessNormalized;
                splatMap[zRes, xRes, 1] = steepnessNormalized;
            }
        }

        terrainData.SetAlphamaps(0, 0, splatMap);
    }

}
