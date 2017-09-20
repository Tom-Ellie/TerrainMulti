using UnityEngine;
using System.Collections;

public class GameControl : MonoBehaviour {

    public static GameControl control; //Ensures this is single instance
                                       // Use this for initialization

    public int valueStored;

	void Awake () {
        if (control == null) { //If no control exists...
            DontDestroyOnLoad(gameObject);
            control = this;
        } else if (control != this) { //If this isn't the control...
            Destroy(gameObject);
        }
	}
	
	// Testing using gui
	void OnGUI () {
        GUI.Label(new Rect(10,10,100,30), "Val stored: " + valueStored);
	}
}
