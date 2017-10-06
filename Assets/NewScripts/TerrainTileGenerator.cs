using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainTileGenerator : MonoBehaviour {

    public RegionHolder2 region;

    // Use this for initialization

    void Start() {
        Terrain terrain = GetComponent < Terrain>();
        terrain.terrainData = GenerateTerrainData.GenerateTerrain(region.heightMapSettings, region.meshSettings.width, region.meshSettings.depth, Vector2.zero);

        //  AssignSplatMap.SetSplatMap(terrain);
    }

}
