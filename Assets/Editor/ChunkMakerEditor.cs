using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CreateChunks))]
public class ChunkMakerEditor : Editor {

    public override void OnInspectorGUI() {
        CreateChunks createChunks = (CreateChunks)target;

        if (DrawDefaultInspector()) {
        }

        if (GUILayout.Button("Generate")) {
            createChunks.DrawChunksInEditor();
        }
    }
}