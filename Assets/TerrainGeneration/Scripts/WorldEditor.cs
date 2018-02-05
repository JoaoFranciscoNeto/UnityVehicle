﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(World))]
public class WorldEditor : Editor {

    public override void OnInspectorGUI()
    {
        World world = (World)target;
        if (DrawDefaultInspector() && world.autoUpdate)
        {
            world.GenerateWorld();
        }

        if (GUILayout.Button("Generate"))
        {
            world.GenerateWorld();
        }
    }
}
