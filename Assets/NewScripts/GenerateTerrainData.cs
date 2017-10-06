using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GenerateTerrainData  {

    public static TerrainData GenerateTerrain(HeightMapSettings2 heightMapsSettings, int width, int depth, Vector2 sampleCentre) {
        TerrainData terrainData = new TerrainData {
            heightmapResolution = width,
            size = new Vector3(width, depth, width)
        };
        terrainData.SetDetailResolution(1024, 16);

        float[,] heights = new float[width+1, width+1];
        Heights = CalcNoise(xOrg, yOrg);

        Debug.Log(sampleCentre);
        float[,] heights = new float[width, width];
              for (int x = 0; x < width; x++) {
                  for (int y = 0; y < width; y++) {
                      float xPos = (float)(x + sampleCentre.x) / width;
                      float yPos = (float)(y + sampleCentre.y) / width;
                      heights[x,y] = Mathf.PerlinNoise(xPos / (m_MapSize.x) * m_ScaleRED, (yPos / (m_MapSize.y) * m_ScaleRED)
         //       heights = Noise.GenerateNoiseMap(width, heightMapsSettings.noiseSettings, sampleCentre);
            }
        }


        terrainData.SetHeights(0, 0, heights);

        return terrainData;
    }
/*
    private static float[,] GenerateHeights(int width) {

        //   return HeightMapGenerator2.GenerateHeightMap(width, region.heightMapSettings, new Vector2(0, 0)).values;
        float[,] heights = new float[width, width];
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < width; y++) {
                heights[x, y] = GenerateHeight(x, y, width);
            }
        }

        return heights;
    }


    private static float GenerateHeight(int x, int y, int width) {
        float xPos = (float)x / width;
        float yPos = (float)y / width;

        return Mathf.PerlinNoise(xPos, yPos);
    }
    */
}
