using UnityEngine;

public class TerrainChunk2 {

    const float colliderGenerationDistanceThreshold = 10;
    public event System.Action<TerrainChunk2, bool> onVisibilityChanged;
    public Vector2 coord;

    GameObject terrain;
    TerrainData terrainData;
    Vector2 sampleCentre;
    Bounds bounds;

    bool terrainDataRecieved;
    bool hasSetCollider;
    float maxViewDst;

    HeightMapSettings2 heightMapSettings;
    MeshSettings2 meshSettings;
    Transform viewer;

    Transform parent;

    //Parent is the TerrainGenerator
    public TerrainChunk2(Vector2 coord, HeightMapSettings2 heightMapSettings, MeshSettings2 meshSettings, Transform parent, Transform viewer) {
        this.coord = coord;
        this.heightMapSettings = heightMapSettings;
        this.meshSettings = meshSettings;
        this.viewer = viewer;
        this.parent = parent;

        //sampleCentre is the centre of this chunk without scaling
        sampleCentre = coord * meshSettings.width; //Effectively same as coord * (numVertsPerLine - 3)
        Vector2 position = coord * meshSettings.meshSize; //position is coord ((0,1) etc) * tileSize. so if tile is 400, scale is 2, and coord is (1,2), pos is (800, 1600)
        bounds = new Bounds(position, Vector2.one * meshSettings.meshSize); //Bounds contains centre of position, and size of worldSize (if as above, 800)

        terrain = new GameObject("Terrain");

        maxViewDst = meshSettings.maxViewDistance; //CHANGE TO CONSTANT DIST SPECIFIABLE IN MESH DATA

    }

    //Get height map then update terrain
    public void Load() {
       // if (Application.isEditor && !Application.isPlaying) {
            OnHeightMapReceived(GenerateTerrainData.GenerateTerrain(heightMapSettings, meshSettings.width, meshSettings.depth, sampleCentre));
      //  } else {
       //     ThreadedDataRequester.RequestData(() => GenerateTerrainData.GenerateTerrain(heightMap, meshSettings.width, meshSettings.depth), OnHeightMapReceived);
       // }
    }


    //Called once load is sucessful for this chunk
    void OnHeightMapReceived(object terrainData) {
        Vector2 position = coord * meshSettings.meshSize; //position is coord ((0,1) etc) * tileSize. so if tile is 400, scale is 2, and coord is (1,2), pos is (800, 1600)
        this.terrainData =  (TerrainData)terrainData;
        terrainDataRecieved = true;
        terrain = Terrain.CreateTerrainGameObject(this.terrainData);

        terrain.transform.position = new Vector3(position.x, 0, position.y); //Set actual position to real position
        terrain.transform.parent = parent; //Set parent

        SetVisible(true);
    }

    Vector2 viewerPosition {
        get {
            return new Vector2(viewer.position.x, viewer.position.z);
        }
    }

    //Called upon recieving height meshes, and when lod updates
    public void UpdateTerrainChunk() {
        if (terrainDataRecieved) { //If loaded and recieved a height map...
            float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));

            bool wasVisible = IsVisible();
            bool visible = (viewerDstFromNearestEdge <= maxViewDst);

            //If changed from visible to invisible, or vice versa
            if (wasVisible != visible) {

                SetVisible(visible);
                if (onVisibilityChanged != null) {
                    onVisibilityChanged(this, visible);
                }
            }
        }
    }

    public void SetVisible(bool visible) {
        terrain.SetActive(visible);
    }

    public bool IsVisible() {
        return terrain.activeSelf;
    }

}