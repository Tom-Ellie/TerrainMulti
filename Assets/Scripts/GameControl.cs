using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameControl : MonoBehaviour {

    public static GameControl control; //Ensures this is single instance
                                       // Use this for initialization

    public float valueStored;

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

    //Not super secure if serialized. Should encrypt etc.
    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Create);

        PlayerData data = new PlayerData();
        data.valueStored = valueStored;
        Debug.Log(Application.persistentDataPath);
        bf.Serialize(file, data); //Sends data as binary to file
        file.Close();
    }

    public void Load() {
        if (File.Exists(Application.persistentDataPath + "/playerInfo.dat")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();
            valueStored = data.valueStored;
        }
    }
}

[Serializable] //Tells unity this class can be written to a file
class PlayerData {
    public float valueStored;
}