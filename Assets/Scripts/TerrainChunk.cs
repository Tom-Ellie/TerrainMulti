using UnityEngine;

public class TerrainChunk {
	
	const float colliderGenerationDistanceThreshold = 5;
	public event System.Action<TerrainChunk, bool> onVisibilityChanged;
	public Vector2 coord;
	 
	GameObject meshObject;
	Vector2 sampleCentre;
	Bounds bounds;

	MeshRenderer meshRenderer;
	MeshFilter meshFilter;
	MeshCollider meshCollider;

	LODInfo[] detailLevels;
	LODMesh[] lodMeshes;
	int colliderLODIndex;

	HeightMap heightMap;
	bool heightMapReceived;
	int previousLODIndex = -1;
	bool hasSetCollider;
	float maxViewDst;

	HeightMapSettings heightMapSettings;
	MeshSettings meshSettings;
	Transform viewer;

    //Parent is the TerrainGenerator
	public TerrainChunk(Vector2 coord, HeightMapSettings heightMapSettings, MeshSettings meshSettings, LODInfo[] detailLevels, int colliderLODIndex, Transform parent, Transform viewer, Material material, bool isVis) {
		this.coord = coord;
		this.detailLevels = detailLevels;
		this.colliderLODIndex = colliderLODIndex;
		this.heightMapSettings = heightMapSettings;
		this.meshSettings = meshSettings;
		this.viewer = viewer;

        //sampleCentre is the centre of this chunk without scaling
		sampleCentre = coord * meshSettings.meshWorldSize / meshSettings.meshScale; //Effectively same as coord * (numVertsPerLine - 3)
        Vector2 position = coord * meshSettings.meshWorldSize; //position is coord ((0,1) etc) * tileSize. so if tile is 400, scale is 2, and coord is (1,2), pos is (800, 1600)
		bounds = new Bounds(position,Vector2.one * meshSettings.meshWorldSize ); //Bounds contains centre of position, and size of worldSize (if as above, 800)


		meshObject = new GameObject("Terrain Chunk");
		meshRenderer = meshObject.AddComponent<MeshRenderer>();
		meshFilter = meshObject.AddComponent<MeshFilter>();
		meshCollider = meshObject.AddComponent<MeshCollider>();
		meshRenderer.material = material;

		meshObject.transform.position = new Vector3(position.x,0,position.y); //Set actual position to real position
		meshObject.transform.parent = parent; //Set parent
		SetVisible(isVis); //Make visible

		lodMeshes = new LODMesh[detailLevels.Length]; //Array of LOD meshes.
		for (int i = 0; i < detailLevels.Length; i++) {
			lodMeshes[i] = new LODMesh(detailLevels[i].lod); //Create new lod mesh with lod lod
			lodMeshes[i].updateCallback += UpdateTerrainChunk; //Within the lodMesh, when updateCallback called, execute UpdateTerrain
			if (i == colliderLODIndex) {
				lodMeshes[i].updateCallback += UpdateCollisionMesh; //One of the LODs also have collisionmesh added to callback
			}
		}

		maxViewDst = detailLevels [detailLevels.Length - 1].visibleDstThreshold;

	}

    //Get height map then update terrain
    public void Load() {
        //Pass function to generate height map, along with a callback, to multi-threading
        if (Application.isEditor && !Application.isPlaying) {
            OnHeightMapReceived(HeightMapGenerator.GenerateHeightMap(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, heightMapSettings, sampleCentre));
        } else {
            ThreadedDataRequester.RequestData(() => HeightMapGenerator.GenerateHeightMap(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, heightMapSettings, sampleCentre), OnHeightMapReceived);
        }
    }


    //Called once load is sucessful for this chunk
	void OnHeightMapReceived(object heightMapObject) {

        this.heightMap = (HeightMap)heightMapObject;
		heightMapReceived = true;

		UpdateTerrainChunk ();
	}

	Vector2 viewerPosition {
		get {
			return new Vector2 (viewer.position.x, viewer.position.z);
		}
	}

    //Called upon recieving height meshes, and when lod updates
	public void UpdateTerrainChunk() {
		if (heightMapReceived) { //If loaded and recieved a height map...
			float viewerDstFromNearestEdge = Mathf.Sqrt (bounds.SqrDistance (viewerPosition));

			bool wasVisible = IsVisible ();
			bool visible = (viewerDstFromNearestEdge <= maxViewDst);

			if (visible) {
				int lodIndex = 0;

				for (int i = 0; i < detailLevels.Length - 1; i++) {
                    //If further than currently considered lod, 
					if (viewerDstFromNearestEdge > detailLevels [i].visibleDstThreshold) {
						lodIndex = i + 1; //Keeps 1 ahead of loop, so when exits contains actual lod
					} else {
						break;
					}
				}

				if (lodIndex != previousLODIndex) { //If need a different lod
					LODMesh lodMesh = lodMeshes [lodIndex]; //Get needed lod
					if (lodMesh.hasMesh) { //If a mesh has been made...
						previousLODIndex = lodIndex; //Update lod and mesh
						meshFilter.mesh = lodMesh.mesh;
					} else if (!lodMesh.hasRequestedMesh) {
						lodMesh.RequestMesh (heightMap, meshSettings); //Request a mesh
					}
				}


			}

            //If changed from visible to invisible, or vice versa
			if (wasVisible != visible) {
				
				SetVisible (visible);
				if (onVisibilityChanged != null) {
					onVisibilityChanged (this, visible);
				}
			}
		}
	}

	public void UpdateCollisionMesh() {
		if (!hasSetCollider) {
			float sqrDstFromViewerToEdge = bounds.SqrDistance (viewerPosition);

			if (sqrDstFromViewerToEdge < detailLevels [colliderLODIndex].sqrVisibleDstThreshold) {
				if (!lodMeshes [colliderLODIndex].hasRequestedMesh) {
					lodMeshes [colliderLODIndex].RequestMesh (heightMap, meshSettings);
				}
			}

			if (sqrDstFromViewerToEdge < colliderGenerationDistanceThreshold * colliderGenerationDistanceThreshold) {
				if (lodMeshes [colliderLODIndex].hasMesh) {
					meshCollider.sharedMesh = lodMeshes [colliderLODIndex].mesh;
					hasSetCollider = true;
				}
			}
		}
	}

	public void SetVisible(bool visible) {
		meshObject.SetActive (visible);
	}

	public bool IsVisible() {
		return meshObject.activeSelf;
	}

}

class LODMesh {

	public Mesh mesh;
	public bool hasRequestedMesh;
	public bool hasMesh;
	int lod;
	public event System.Action updateCallback;

	public LODMesh(int lod) {
		this.lod = lod;
	}

	void OnMeshDataReceived(object meshDataObject) {
		mesh = ((MeshData)meshDataObject).CreateMesh ();
		hasMesh = true;

		updateCallback ();
	}
 
	public void RequestMesh(HeightMap heightMap, MeshSettings meshSettings) {
        //Request mesh be made, generate terrain mesh, then go to OnMeshData, which calls UpdateTerrainChunk/CollisionMesh
		hasRequestedMesh = true;
        if (Application.isEditor && !Application.isPlaying) {
            OnMeshDataReceived(MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, lod));
        } else {
            ThreadedDataRequester.RequestData(() => MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, lod), OnMeshDataReceived);
        }
    }

}