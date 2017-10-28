using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonMaker : MonoBehaviour {

    public DungeonData dungeonData;

    Material material;

    List<DungeonChunk> dungeonChunks = new List<DungeonChunk>();


    public void Start() {
        material = new Material(Shader.Find("Specular"));
        MakeBaseDungeon();
    }

    public void MakeBaseDungeon() {
       for(int i=0;i < dungeonData.numTilesPerRow; i++) {
            for (int j = 0; j < dungeonData.numTilesPerRow; j++) {
                DungeonChunk chunk = new DungeonChunk(new Vector2(i, j), dungeonData, this.transform, material);
                dungeonChunks.Add(chunk);
                chunk.Load();
            }
        }

    }

}
