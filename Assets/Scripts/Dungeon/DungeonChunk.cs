using UnityEngine;

public class DungeonChunk {

    const float colliderGenerationDistanceThreshold = 5;
  //  public event System.Action<DungeonChunk, bool> onVisibilityChanged;
    public Vector2 coord;

    GameObject meshObject;
    Vector2 sampleCentre;
  //  Bounds bounds;

    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
 //   MeshCollider meshCollider;

    MeshDungeon dungeonMesh;
    int colliderLODIndex;

    float[,] heightMap;
    bool heightMapReceived;

 //   bool hasSetCollider;
 //   float maxViewDst;

    DungeonData dungeonData;
 //   Transform viewer;

    public DungeonChunk(Vector2 coord, DungeonData dungeonData, Transform parent, Material material) {
        this.coord = coord;

   //     this.viewer = viewer;

        this.dungeonData = dungeonData;

        sampleCentre = coord * (dungeonData.meshTileSize - 1);

        //Bounds from centre sampleCentre, for it's width
  //      bounds = new Bounds(sampleCentre, Vector2.one * dungeonData.meshTileSize);


        meshObject = new GameObject("Dungeon Chunk");
        meshRenderer = meshObject.AddComponent<MeshRenderer>();
        meshFilter = meshObject.AddComponent<MeshFilter>();
 //       meshCollider = meshObject.AddComponent<MeshCollider>();
        meshRenderer.material = material;

        meshObject.transform.position = new Vector3(sampleCentre.x, 0, sampleCentre.y);
        meshObject.transform.parent = parent;
   //     SetVisible(isVis);

        dungeonMesh = new MeshDungeon();
        dungeonMesh.updateCallback += UpdateDungeonChunk;
        dungeonMesh.updateCallback += UpdateCollisionMesh;


  //      maxViewDst = 20f;

    }

    public void Load() {
        //Pass function to generate height map, along with a callback, to multi-threading
//        if (Application.isEditor && !Application.isPlaying) {
            OnHeightMapReceived(DungeonHeightMap.GenerateFlatMap(dungeonData.meshTileSize));
//        } else {
//            ThreadedDataRequester.RequestData(() => DungeonHeightMap.GenerateFlatMap(dungeonData.meshTileSize, dungeonData), OnHeightMapReceived);
//        }
    }


    //Called once load is sucessful for this chunk
    void OnHeightMapReceived(object heightMapObject) {
        this.heightMap = (float[,])heightMapObject;
        heightMapReceived = true;

        UpdateDungeonChunk();
    }
/*
    Vector2 viewerPosition {
        get {
            return new Vector2(viewer.position.x, viewer.position.z);
        }
    }
    */

    public void UpdateDungeonChunk() {
        if (heightMapReceived) { //If loaded and recieved a height map...
//            float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));

     //       bool visible = viewerDstFromNearestEdge <= maxViewDst;

    //        if (visible) {
                if (dungeonMesh.hasMesh) {
                    meshFilter.mesh = dungeonMesh.mesh;
                } else if (!dungeonMesh.hasRequestedMesh) {
                    dungeonMesh.RequestMesh(heightMap, dungeonData);
      //          }


            }
/*
            if (wasVisible != visible) {

                SetVisible(visible);
                if (onVisibilityChanged != null) {
                    onVisibilityChanged(this, visible);
                }
            }
            */
        }
    }

    public void UpdateCollisionMesh() {
        /*
        if (!hasSetCollider) {
            float sqrDstFromViewerToEdge = bounds.SqrDistance(viewerPosition);
            if (sqrDstFromViewerToEdge < maxViewDst) {
                if (!dungeonMesh.hasRequestedMesh) {
                    dungeonMesh.RequestMesh(heightMap, dungeonData);
                }
            }

            if (sqrDstFromViewerToEdge < colliderGenerationDistanceThreshold * colliderGenerationDistanceThreshold) {
                if (dungeonMesh.hasMesh) {
                    meshCollider.sharedMesh = dungeonMesh.mesh;
                    hasSetCollider = true;
                }
            }
        } 
        */
    }

    /*
    public void SetVisible(bool visible) {
        meshObject.SetActive(visible);
    }

    public bool IsVisible() {
        return meshObject.activeSelf;
    }
    */

}

class MeshDungeon {

    public Mesh mesh;
    public bool hasRequestedMesh;
    public bool hasMesh;
    public event System.Action updateCallback;

    public MeshDungeon() {
    }

    void OnMeshDataReceived(object meshDataObject) {
        mesh = ((DungeonMeshData)meshDataObject).CreateMesh();
        hasMesh = true;

        updateCallback();
    }

    public void RequestMesh(float[,] heightMap, DungeonData dungeonData) {
        hasRequestedMesh = true;
 //       if (Application.isEditor && !Application.isPlaying) {
            OnMeshDataReceived(DungeonMeshGenerator.GenerateDungeonMesh(heightMap));
 //       } else {
 //           ThreadedDataRequester.RequestData(() => DungeonMeshGenerator.GenerateDungeonMesh(heightMap.values), OnMeshDataReceived);
//        }

    }
}