using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(MapPreviewArea))]
public class MapPreviewAreaEditor : Editor {

    public override void OnInspectorGUI() {
        MapPreviewArea mapPreviewArea = (MapPreviewArea)target;

        if (DrawDefaultInspector()) {
 //           if (mapPreview.autoUpdate) {
 //              mapPreview.DrawMapInEditor();
 //           }
        }

                if (GUILayout.Button("Generate")) {
                    mapPreviewArea.DrawMapInEditor();
                }
    }
}