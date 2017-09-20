using UnityEngine;
using System.Collections;

public class TestButton : MonoBehaviour {

    void OnGUI() {
        if (GUI.Button(new Rect(10, 100, 100, 30), "Save")) {
            GameControl.control.Save();
        }
        if (GUI.Button(new Rect(10, 140, 100, 30), "Load")) {
            GameControl.control.Load();
        }
    }
}
