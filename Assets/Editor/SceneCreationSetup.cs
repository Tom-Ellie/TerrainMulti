using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

public class SceneCreationSetup : MonoBehaviour {

    [PostProcessSceneAttribute(2)]
    public static void OnPostprocessScene(){
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(0.0f, 0.5f, 0.0f);
    }
}
