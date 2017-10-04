using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* Makes the relevant calls to generate terrain */
public class TerrainGenerator : MonoBehaviour {

	const float viewerMoveThresholdForChunkUpdate = 25f; //How much viewer must move to update active chunks
	const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate; //Square for easier calcs


	public int colliderLODIndex; //LOD for collider mesh
	public LODInfo[] detailLevels; //Array containing LOD distances and quality

	public MeshSettings meshSettings; //Reference to used MeshSettings
	public HeightMapSettings heightMapSettings; //Reference to height map settings
	public TextureData textureSettings; //reference to texture data

	public Transform viewer; //Specified viewer (usually user)
	public Material mapMaterial; //Material being used

	Vector2 viewerPosition; //These two used for movement checks
	Vector2 viewerPositionOld;

	float meshTileSize;
	int chunksVisibleInViewDst;

	Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
	List<TerrainChunk> visibleTerrainChunks = new List<TerrainChunk>();

	void Start() {
		textureSettings.ApplyToMaterial (mapMaterial); //Provides applies texture to material
		textureSettings.UpdateMeshHeights (mapMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight); //Gets min and max heights using height multiplier * height curve at min and max

		float maxViewDst = detailLevels [detailLevels.Length - 1].visibleDstThreshold;
		meshTileSize = meshSettings.meshWorldSize;
		chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / meshTileSize);

		UpdateVisibleChunks ();
	}

	void Update() {
		viewerPosition = new Vector2 (viewer.position.x, viewer.position.z);

		if (viewerPosition != viewerPositionOld) {
			foreach (TerrainChunk chunk in visibleTerrainChunks) {
				chunk.UpdateCollisionMesh ();
			}
		}

		if ((viewerPositionOld - viewerPosition).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate) {
			viewerPositionOld = viewerPosition;
			UpdateVisibleChunks ();
		}
	}
		
	void UpdateVisibleChunks() {
		HashSet<Vector2> alreadyUpdatedChunkCoords = new HashSet<Vector2> ();
		for (int i = visibleTerrainChunks.Count-1; i >= 0; i--) { //Go through list of known visible chunks
			alreadyUpdatedChunkCoords.Add (visibleTerrainChunks [i].coord); //Add to temp list of already 'updated' chunks, as part of visibleTerrainChunks
			visibleTerrainChunks [i].UpdateTerrainChunk (); //Actually update them
		}
			
		int currentChunkCoordX = Mathf.RoundToInt (viewerPosition.x / meshTileSize); //Which chunk you're currently in
		int currentChunkCoordY = Mathf.RoundToInt (viewerPosition.y / meshTileSize);
		for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++) { //For all chunks in view
			for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++) {
				Vector2 viewedChunkCoord = new Vector2 (currentChunkCoordX + xOffset, currentChunkCoordY + yOffset); //Chunk position, where each chunk increments by 1 in each direction
                if (!alreadyUpdatedChunkCoords.Contains (viewedChunkCoord)) { //If not updated already in first for loop...
					if (terrainChunkDictionary.ContainsKey (viewedChunkCoord)) { //If chunk has been created previously, but not in visible array...
						terrainChunkDictionary [viewedChunkCoord].UpdateTerrainChunk (); //Simply update it
					} else {
						TerrainChunk newChunk = new TerrainChunk (viewedChunkCoord, heightMapSettings, meshSettings, detailLevels, colliderLODIndex, transform, viewer, mapMaterial, false);
						terrainChunkDictionary.Add (viewedChunkCoord, newChunk); //Add to set of chunks that've been made
						newChunk.onVisibilityChanged += OnTerrainChunkVisibilityChanged; //Add the OnTerrainChunk... func to the newChunk's onVisibilityChanged call. Now when chunk calls onVis..., it will execute OnTerrain...
						newChunk.Load ();
					}
				}

			}
		}
	}

	void OnTerrainChunkVisibilityChanged(TerrainChunk chunk, bool isVisible) {
		if (isVisible) {
			visibleTerrainChunks.Add (chunk);
		} else {
			visibleTerrainChunks.Remove (chunk);
		}
	}

}

[System.Serializable]
public struct LODInfo {
	[Range(0,MeshSettings.numSupportedLODs-1)]
	public int lod;
	public float visibleDstThreshold;


	public float sqrVisibleDstThreshold {
		get {
			return visibleDstThreshold * visibleDstThreshold;
		}
	}
}
