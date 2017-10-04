using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateChunks : MonoBehaviour {

    public int numChunks = 2;

    public RegionHolder region;

    public Material baseMaterial;

    GameObject[] chunks;


    //Need an if to check if exists
    public void DrawChunksInEditor() {
        float size = region.meshSettings.meshWorldSize;
        for (int i = 0; i < numChunks; i++) {
            for (int j = 0; j < numChunks; j++) {
                GameObject chunk = new GameObject("chunk" + i + "," + j);

                chunk.transform.position = new Vector3(size * i * 2, 0, size * j * 2);
                Mesh mesh = CreateBaseMesh(size, "chunk" + i + "," + j);
                MeshFilter meshFilter = (MeshFilter)chunk.AddComponent(typeof(MeshFilter));
                meshFilter.mesh = mesh;

                MeshRenderer renderer = chunk.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
                renderer.material = baseMaterial;
 //               renderer.sharedMaterial.mainTexture = TextureGenerator.MakeTextureOfColour(4, Color.green);


                Texture2D tex = new Texture2D(1, 1);
                tex.SetPixel(0, 0, Color.green);
                tex.Apply();

                renderer.sharedMaterial.mainTexture = tex;
                renderer.sharedMaterial.color = Color.green;

                chunk.transform.parent = this.transform;
            }
        }
    }

    Mesh CreateBaseMesh(float size, string name) {
        Mesh m = new Mesh();
        m.name = name;
        m.vertices = new Vector3[] {
         new Vector3(-size, 0.01f, size),
         new Vector3( size, 0.01f, size),
         new Vector3(-size, 0.01f, -size),
         new Vector3( size, 0.01f, -size)
     };
        m.uv = new Vector2[] {
         new Vector2 (0, 0),
         new Vector2 (0, 1),
         new Vector2(1, 1),
         new Vector2 (1, 0)
     };
        m.triangles = new int[] { 0, 1, 2, 3, 2, 1 };
        m.RecalculateNormals();

        return m;
    }


}
