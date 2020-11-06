using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UnifiedUIAppearanceController))]
public class UnifiedUIAppearanceEditor : Editor {

	public override void OnInspectorGUI() {
		this.DrawDefaultInspector();

		UnifiedUIAppearanceController controller = (UnifiedUIAppearanceController)this.target;

		if (GUILayout.Button("Apply")) {
			Transform[] dirtyObjects = controller.applyChanges();

			// Redraw affected objects
			foreach (Object dirtyObject in dirtyObjects) {
				EditorUtility.SetDirty(dirtyObject);
			}

			SceneView.RepaintAll();
		}
	}
}
