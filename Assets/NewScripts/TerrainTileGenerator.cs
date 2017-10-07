using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainTileGenerator : MonoBehaviour {

    public RegionHolder2 region;

    // Use this for initialization

    void Start() {
        Terrain terrain = GetComponent <Terrain>();
        float[,] heights = GenerateTerrainData.GenerateTerrain(region.heightMapSettings, region.meshSettings, Vector2.zero);
        terrain.terrainData = GenerateTerrainData.GenerateTerrainDataStuff(region.meshSettings, heights);

        //  AssignSplatMap.SetSplatMap(terrain);
    }

}
