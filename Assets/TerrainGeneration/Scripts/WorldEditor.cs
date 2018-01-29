using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(World))]
public class WorldEditor : Editor {

    public override void OnInspectorGUI()
    {
        World world = (World)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Generate"))
        {
            world.GenerateWorld();
        }
    }
}
