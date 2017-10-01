using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(MergeTiles))]
public class WorldGenEditor : Editor {

    public override void OnInspectorGUI() {
        MergeTiles mergeTiles = (MergeTiles)target;

        if (DrawDefaultInspector()) {
        }

        if (GUILayout.Button("Generate")) {
            mergeTiles.DrawMapInEditor();
        }
    }
}
