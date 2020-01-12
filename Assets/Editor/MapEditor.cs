using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (MapGenerator))]
public class MapEditor : Editor {

    public override void OnInspectorGUI () {

        MapGenerator map = target as MapGenerator;
        if (DrawDefaultInspector () || GUILayout.Button ("Generate Map")) {
            map.GenerateMap ();
        }
    }

}