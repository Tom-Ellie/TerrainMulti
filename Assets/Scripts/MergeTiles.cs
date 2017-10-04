using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeTiles : MonoBehaviour {

    public RegionHolder[] regions;

    //TODO: Make limits
    public int xAxisStart = 0;
    public int xAxisEnd = 0;
    public int yAxisStart = 0;
    public int yAxisEnd = 0;

    //Used when making a sub region of map, allows for sections outside of specified region to be considered for region
    public bool considerBorders;

    HeightMap[,] heightMaps;

    public void DrawMapInEditor() {

        regions[0].textureData.ApplyToMaterial(regions[0].terrainMaterial);
        regions[0].textureData.UpdateMeshHeights(regions[0].terrainMaterial, regions[0].heightMapSettings.minHeight, regions[0].heightMapSettings.maxHeight);
        regions[1].textureData.ApplyToMaterial(regions[1].terrainMaterial);
        regions[1].textureData.UpdateMeshHeights(regions[1].terrainMaterial, regions[1].heightMapSettings.minHeight, regions[1].heightMapSettings.maxHeight);
        Debug.Log(regions[1].meshSettings.numVertsPerLine);
        heightMaps = new HeightMap[(xAxisEnd - xAxisStart)+1, (yAxisEnd - yAxisStart)+1];
        for (int x = xAxisStart; x <= xAxisEnd; x++) {
            for (int y = yAxisStart; y <= yAxisEnd; y++) {
                //Look at a tile, make it's height map, generate neighbour's height maps, balance
                RegionHolder region;
                if (x == 0 && y == 0) {
                    region = regions[0];
                } else {
                    region = regions[1];
                }
                int sampleCentreX = x * (region.meshSettings.numVertsPerLine-3);
                int sampleCentreY = y * (region.meshSettings.numVertsPerLine-3); //Why -3 here????
                Vector2 centre = new Vector2(sampleCentreX, sampleCentreY);
                heightMaps[x, y] = HeightMapGenerator.GenerateHeightMap((int)(region.meshSettings.numVertsPerLine), region.heightMapSettings, centre);
                DrawMesh(MeshGenerator.GenerateTerrainMesh(heightMaps[x, y].values, region.meshSettings, 0), x, y);

            }
        }
        /*
for (int x = xAxisStart; x < xAxisEnd; x++) {
for (int y = yAxisStart; y < yAxisEnd; y++) {
HeightMap hm;
MeshData md;

    if (x == xAxisStart && y == yAxisEnd) {
        hm = HeightMapGenerator.AverageHeightMaps(heightMaps[x, y], heightMaps[x, y], heightMaps[x, y], heightMaps[x + 1, y], heightMaps[x, y + 1], (int)(regions[0].meshSettings.numVertsPerLine), x, y, xAxisStart, xAxisEnd, yAxisStart, yAxisEnd);
        md = MeshGenerator.GenerateTerrainMesh(hm.values, regions[0].meshSettings, 0);
    } else if (x == xAxisStart) {
        hm = HeightMapGenerator.AverageHeightMaps(heightMaps[x, y], heightMaps[x, y], heightMaps[x, y - 1], heightMaps[x + 1, y], heightMaps[x, y + 1], (int)(regions[0].meshSettings.numVertsPerLine), x, y,  xAxisStart, xAxisEnd, yAxisStart, yAxisEnd);
        md = MeshGenerator.GenerateTerrainMesh(hm.values, regions[0].meshSettings, 0);
    } else if (y == yAxisStart) {
        hm = HeightMapGenerator.AverageHeightMaps(heightMaps[x, y], heightMaps[x - 1, y], heightMaps[x, y], heightMaps[x + 1, y], heightMaps[x, y + 1], (int)(regions[0].meshSettings.numVertsPerLine), x, y, xAxisStart, xAxisEnd, yAxisStart, yAxisEnd);
        md = MeshGenerator.GenerateTerrainMesh(hm.values, regions[0].meshSettings, 0);
    } else if (x == xAxisEnd) {
        hm = HeightMapGenerator.AverageHeightMaps(heightMaps[x, y], heightMaps[x - 1, y], heightMaps[x, y - 1], heightMaps[x, y], heightMaps[x, y + 1], (int)(regions[0].meshSettings.numVertsPerLine), x, y, xAxisStart, xAxisEnd, yAxisStart, yAxisEnd);
        md = MeshGenerator.GenerateTerrainMesh(hm.values, regions[0].meshSettings, 0);
    } else if (y == yAxisEnd) {
        hm = HeightMapGenerator.AverageHeightMaps(heightMaps[x, y], heightMaps[x - 1, y], heightMaps[x, y - 1], heightMaps[x + 1, y], heightMaps[x, y], (int)(regions[0].meshSettings.numVertsPerLine), x, y, xAxisStart, xAxisEnd, yAxisStart, yAxisEnd);
        md = MeshGenerator.GenerateTerrainMesh(hm.values, regions[0].meshSettings, 0);
    } else {
        hm = HeightMapGenerator.AverageHeightMaps(heightMaps[x, y], heightMaps[x - 1, y], heightMaps[x, y - 1], heightMaps[x + 1, y], heightMaps[x, y + 1], (int)(regions[1].meshSettings.numVertsPerLine), x, y, xAxisStart, xAxisEnd, yAxisStart, yAxisEnd);
        md = MeshGenerator.GenerateTerrainMesh(hm.values, regions[1].meshSettings, 0);
    }
        Transform tmp = transform.Find("plane" + x + "," + y);
                tmp.GetComponent<MeshFilter>().sharedMesh = md.CreateMesh();
            }
        }
        */

//                DrawMeshes(MeshGenerator.GenerateTerrainMesh(heightMap.values, region.meshSettings, 0));
    }


    public void DrawMesh(MeshData meshData, int x, int y) {
        Transform tmp = transform.Find("plane" + x + "," + y);
        Debug.Log("x is:" + x + " and y is: " + y);
        tmp.GetComponent<MeshFilter>().sharedMesh = meshData.CreateMesh();
    }
}
