using System;
using System.Collections;
using System.Collections.Generic;
using PerkSystem;
using UnityEditor;
using UnityEngine;

public class PerkTreeWindow : EditorWindow
{


    Texture2D buttonsSectionTexture;

    Color buttonsSectionColor = new Color32(50, 50, 50, 255);

    Rect buttonsSection;
    Rect workSpaceSection;

    PerkWorkspace workspace;

    Vector2 _lMousePosTemp;

    public static PerkTreeWindow window;
    public static PerkTree dataPerkTree;

    public static int SelectedPerkObject;

    public static event Action<EventType, Vector2, int> OnKeyEvent;


    public static void OpenWindow(PerkTree perkTree)
    {
        dataPerkTree = perkTree;

        window = (PerkTreeWindow)GetWindow(typeof(PerkTreeWindow));
        window.minSize = new Vector2(600, 400);
        window.Show();
    }


    #region  Init
    void OnEnable()
    {
        OnKeyEvent = null;

        InitData();
        InitTexture();
    }

    void InitData()
    {

        if (!dataPerkTree)
            window.Close();

        workspace = new PerkWorkspace();
    }

    void InitTexture()
    {
        buttonsSectionTexture = new Texture2D(1, 1);
        buttonsSectionTexture.SetPixel(0, 0, buttonsSectionColor);
        buttonsSectionTexture.Apply();
    }
    #endregion

    #region Draw
    void OnGUI()
    {
        if (workspace == null) return;

        DrawLayouts();
        workspace.Draw(workSpaceSection);
        DrawButtonsSettings();

        KeysEvents();

        window.Repaint();
    }

    void DrawLayouts()
    {
        buttonsSection.x = 0;
        buttonsSection.y = 0;
        buttonsSection.width = 200;
        buttonsSection.height = Screen.height;

        workSpaceSection.x = 200;
        workSpaceSection.y = 0;
        workSpaceSection.width = Screen.width - 200;
        workSpaceSection.height = Screen.height;

        GUI.DrawTexture(buttonsSection, buttonsSectionTexture);
    }

    void DrawButtonsSettings()
    {
        GUILayout.BeginArea(buttonsSection);

        if (GUILayout.Button("Add", GUILayout.Width(40), GUILayout.Height(40)))
            AddPerk();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Delete")) DeletePerk(SelectedPerkObject);
        SelectedPerkObject = EditorGUILayout.IntSlider(SelectedPerkObject, 0, dataPerkTree.PerkList.Length - 1);
        GUILayout.EndHorizontal();

        GUILayout.EndArea();
    }
    #endregion

    #region  DataActions
    void AddPerk()
    {
        Array.Resize(ref dataPerkTree.PerkList, dataPerkTree.PerkList.Length + 1);
        dataPerkTree.PerkList[dataPerkTree.PerkList.Length - 1] = new PerkObject();
        dataPerkTree.PerkList[dataPerkTree.PerkList.Length - 1].Pos = new Vector2Int(-(int)workspace.offset.x + 50, -(int)workspace.offset.y + 50);

        SelectedPerkObject = dataPerkTree.PerkList.Length - 1;

        EditorUtility.SetDirty(dataPerkTree);
    }

    void DeletePerk(int perkIndex)
    {
        if (perkIndex < 0 || perkIndex >= dataPerkTree.PerkList.Length)
            return;

        //Remove link on Index
        for (int i = 0; i < dataPerkTree.PerkList.Length; i++)
        {
            IntArrayExtention.Remove(ref dataPerkTree.PerkList[i].ChildsIndex, perkIndex);
            IntArrayExtention.Remove(ref dataPerkTree.PerkList[i].ParentsIndex, perkIndex);
        }

        //update links
        for (int i = 0; i < dataPerkTree.PerkList.Length; i++)
        {

            var childLenght = dataPerkTree.PerkList[i].ChildsIndex.Length;
            for (int j = 0; j < childLenght; j++)
            {
                if (dataPerkTree.PerkList[i].ChildsIndex[j] > perkIndex)
                    dataPerkTree.PerkList[i].ChildsIndex[j]--;
            }

            var parentLenght = dataPerkTree.PerkList[i].ParentsIndex.Length;
            for (int j = 0; j < parentLenght; j++)
            {
                if (dataPerkTree.PerkList[i].ParentsIndex[j] > perkIndex)
                    dataPerkTree.PerkList[i].ParentsIndex[j]--;
            }
        }

        //move indexes
        for (int i = perkIndex; i < dataPerkTree.PerkList.Length - 1; i++)
        {
            dataPerkTree.PerkList[i] = dataPerkTree.PerkList[i + 1];
        }

        Array.Resize(ref dataPerkTree.PerkList, dataPerkTree.PerkList.Length - 1);

        //Update Asset
        EditorUtility.SetDirty(dataPerkTree);
    }

    public static void Connect(int parentIndex, int childIndex)
    {
        IntArrayExtention.Add(ref dataPerkTree[parentIndex].ChildsIndex, childIndex);
        IntArrayExtention.Add(ref dataPerkTree[childIndex].ParentsIndex, parentIndex);
    }

    public static void Disconnect(int parentIndex, int childIndex)
    {
        IntArrayExtention.Remove(ref dataPerkTree[parentIndex].ChildsIndex, childIndex);
        IntArrayExtention.Remove(ref dataPerkTree[childIndex].ParentsIndex, parentIndex);
    }

    public static bool ExistConnection(int child, int parent)
    {
        return Array.Exists(dataPerkTree.PerkList[child].ParentsIndex, (x) => x == parent);
    }
    #endregion

    private void KeysEvents()
    {
        var curEvent = Event.current;
        var mousePosition = curEvent.mousePosition;
        switch (curEvent.type)
        {
            case EventType.MouseDrag:
                if (curEvent.button == 1)
                    workspace.MoveOffset(curEvent.delta);
                break;
            // case EventType.MouseDown:
            //     if (curEvent.button == 1)
            //        workspace.MouseSelect(mousePosition);
            //     break;
            case EventType.ScrollWheel:
                //  Debug.Log("EventType.ScrollWheel");
                break;

        }

        if (OnKeyEvent != null)
        {
            OnKeyEvent(curEvent.type, mousePosition, curEvent.button);
        }

    }

    static class IntArrayExtention
    {
        public static void Add(ref int[] array, int value)
        {
            if (!Array.Exists(array, (x) => x == value))
            {
                Array.Resize(ref array, array.Length + 1);
                array[array.Length - 1] = value;
            }
        }

        public static void Remove(ref int[] array, int value)
        {
            int index = Array.FindIndex(array, (x) => x == value);

            if (index != -1)
            {
                for (int i = index; i < array.Length - 1; i++)
                {
                    array[i] = array[i + 1];
                }
                Array.Resize(ref array, array.Length - 1);
            }
        }
    }
}