using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateChunks : MonoBehaviour {

    public int numChunks;

    public RegionHolder region;

    public GameObject[] chunks;


    //Need an if to check if exists
    public void MakeMesh() {
        float size = region.meshSettings.meshWorldSize;
        for (int i = 0; i < numChunks; i++) {
            for (int j = 0; j < numChunks; j++) {
                GameObject chunk = new GameObject();
                chunk.transform.position = new Vector3(size * i, 0, size * j);
                Mesh mesh = new Mesh();
                mesh.name = "mesh" + i + "," + j;
                chunk.AddComponent<MeshFilter>().mesh = new Mesh();

                chunk.transform.parent = this.transform;
            }
        }
    }

}
