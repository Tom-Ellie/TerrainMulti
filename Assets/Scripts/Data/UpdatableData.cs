using UnityEngine;
using System.Collections;

public class UpdatableData : ScriptableObject {

	public event System.Action OnValuesUpdated;
	public bool autoUpdate;

	#if UNITY_EDITOR

	protected virtual void OnValidate() {
		if (autoUpdate) { //If autoupdate is on, add allow updates when the editor updates itself
            UnityEditor.EditorApplication.update += NotifyOfUpdatedValues; //Need to go through update, as can't perform actual action in an OnValidate
        }
	}

	public void NotifyOfUpdatedValues() { //Called either with auto-update (above OnValidate), or when the 'Update' button is pressed (UpdatableDataEditor)
		UnityEditor.EditorApplication.update -= NotifyOfUpdatedValues;
		if (OnValuesUpdated != null) { //If list isn't empty, perform the action
			OnValuesUpdated ();
		}
	}

	#endif

}
