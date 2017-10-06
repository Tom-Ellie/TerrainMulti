using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* Makes the relevant calls to generate terrain */
public class TerrainGenerator2 : MonoBehaviour {

    const float viewerMoveThresholdForChunkUpdate = 25f; //How much viewer must move to update active chunks
    const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate; //Square for easier calcs


    //   public LODInfo[] detailLevels; //Array containing LOD distances and quality

    public RegionHolder2 region;
    public Transform viewer; //Specified viewer (usually user)

    Vector2 viewerPosition; //These two used for movement checks
    Vector2 viewerPositionOld;

    float meshTileSize;
    int chunksVisibleInViewDst;

    Dictionary<Vector2, TerrainChunk2> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk2>();
    List<TerrainChunk2> visibleTerrainChunks = new List<TerrainChunk2>();

    void Start() {
        float maxViewDst = region.meshSettings.maxViewDistance;
        meshTileSize = region.meshSettings.meshSize;
        chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / meshTileSize);
        
    }

    void Update() {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);

        if ((viewerPositionOld - viewerPosition).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate) {
            viewerPositionOld = viewerPosition;
            UpdateVisibleChunks();
        }
    }

    void UpdateVisibleChunks() {
        HashSet<Vector2> alreadyUpdatedChunkCoords = new HashSet<Vector2>();
        for (int i = visibleTerrainChunks.Count - 1; i >= 0; i--) { //Go through list of known visible chunks
            alreadyUpdatedChunkCoords.Add(visibleTerrainChunks[i].coord); //Add to temp list of already 'updated' chunks, as part of visibleTerrainChunks
            visibleTerrainChunks[i].UpdateTerrainChunk(); //Actually update them
        }

        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / meshTileSize); //Which chunk you're currently in
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / meshTileSize);
        for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++) { //For all chunks in view
            for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++) {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset); //Chunk position, where each chunk increments by 1 in each direction
                if (!alreadyUpdatedChunkCoords.Contains(viewedChunkCoord)) { //If not updated already in first for loop...
                    if (terrainChunkDictionary.ContainsKey(viewedChunkCoord)) { //If chunk has been created previously, but not in visible array...
                        terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk(); //Simply update it
                    } else {
                        TerrainChunk2 newChunk = new TerrainChunk2(viewedChunkCoord, region.heightMapSettings, region.meshSettings, transform, viewer);
                        terrainChunkDictionary.Add(viewedChunkCoord, newChunk); //Add to set of chunks that've been made
                        newChunk.onVisibilityChanged += OnTerrainChunkVisibilityChanged; //Add the OnTerrainChunk... func to the newChunk's onVisibilityChanged call. Now when chunk calls onVis..., it will execute OnTerrain...
                        OnTerrainChunkVisibilityChanged(newChunk, true);
                        newChunk.Load();
                    }
                }

            }
        }
    }

    void OnTerrainChunkVisibilityChanged(TerrainChunk2 chunk, bool isVisible) {
        if (isVisible) {
            visibleTerrainChunks.Add(chunk);
        } else {
            visibleTerrainChunks.Remove(chunk);
        }
    }

}
