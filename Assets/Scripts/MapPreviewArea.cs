using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPreviewArea : MonoBehaviour {

    public RegionHolder region;

    public Material mapMaterial; //Material being used

    public int widthOfRegion;

    public int colliderLODIndex; //LOD for collider mesh
    public LODInfo[] detailLevels; //Array containing LOD distances and quality

	public void DrawMapInEditor() {
        region.textureData.ApplyToMaterial(region.terrainMaterial);
        region.textureData.UpdateMeshHeights(region.terrainMaterial, region.heightMapSettings.minHeight, region.heightMapSettings.maxHeight);
        UpdateChunks();
    }

    void UpdateChunks() {
        int currentChunkCoordX = 0;
        int currentChunkCoordY = 0;

        for (int yOffset = -widthOfRegion; yOffset <= widthOfRegion; yOffset++) { //For all chunks in view
            for (int xOffset = -widthOfRegion; xOffset <= widthOfRegion; xOffset++) {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset); //Define centre of chunk in current iter
                TerrainChunk newChunk = new TerrainChunk(viewedChunkCoord, region.heightMapSettings, region.meshSettings, detailLevels, colliderLODIndex, transform, transform, mapMaterial, true);
                newChunk.Load();
            }
        }
    }

}
