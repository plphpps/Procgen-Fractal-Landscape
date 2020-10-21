using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(LandscapeGenerator))]
public class LandscapeGeneratorEditor : Editor {
    public override void OnInspectorGUI() {
        LandscapeGenerator landscapeGenerator = (LandscapeGenerator)target;

        if (DrawDefaultInspector()){
            landscapeGenerator.GenerateLandscape();
        }
    }
}
