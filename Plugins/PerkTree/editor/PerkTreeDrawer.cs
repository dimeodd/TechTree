using System.Collections;
using System.Collections.Generic;
using PerkSystem;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PerkSystem.PerkTree))]
public class PerkTreeDrawer : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Edit"))
            PerkTreeWindow.OpenWindow((PerkTree)target);

        base.OnInspectorGUI();
    }
}