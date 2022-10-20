using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using PerkSystem;

public class PerkWorkspace
{
    Texture2D def_perkAreaTexture;
    Texture2D sel_perkAreaTexture;
    Texture2D connectParentTexture;
    Texture2D connectChildTexture;
    Texture2D workspaceSectionTexture;

    Color workspaceSectionColor = new Color32(70, 100, 150, 255);

    Vector2Int perkSize = new Vector2Int(130, 70);
    Vector2Int connectSize = new Vector2Int(20, 20);

    Rect secton;

    Rect[] perkSections;
    Rect[] perkSectionsParentPoint;
    Rect[] perkSectionsChildPoint;

    public Vector2 offset;
    Vector2 _lMouseTemp;

    #region DragData
    SelType selType;
    #endregion

    public PerkWorkspace()
    {
        IninTexture();
        PerkTreeWindow.OnKeyEvent += Action;
    }

    void IninTexture()
    {
        workspaceSectionTexture = new Texture2D(1, 1);
        workspaceSectionTexture.SetPixel(0, 0, workspaceSectionColor);
        workspaceSectionTexture.Apply();

        def_perkAreaTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Plugins/PerkTree/editor/icons/PerkBG.png");
        sel_perkAreaTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Plugins/PerkTree/editor/icons/PerkBGselected.png");
        connectChildTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Plugins/PerkTree/editor/icons/PerkConnectChild.png");
        connectParentTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Plugins/PerkTree/editor/icons/PerkConnectParent.png");
    }


    #region Draw
    public void Draw(Rect rect)
    {
        secton = rect;
        UpdatePerkSections();

        //Draw background
        GUI.DrawTexture(secton, workspaceSectionTexture);

        //Draw workspace
        GUILayout.BeginArea(secton);

        DrawWorkSpaceSection();

        DrawUIInfo();

        GUILayout.EndArea();
    }

    public void UpdatePerkSections()
    {
        if (perkSections == null || perkSections.Length != PerkTreeWindow.dataPerkTree.PerkList.Length)
        {
            perkSections = new Rect[PerkTreeWindow.dataPerkTree.PerkList.Length];
            perkSectionsChildPoint = new Rect[PerkTreeWindow.dataPerkTree.PerkList.Length];
            perkSectionsParentPoint = new Rect[PerkTreeWindow.dataPerkTree.PerkList.Length];

            //Fill array
            for (int i = 0; i < PerkTreeWindow.dataPerkTree.PerkList.Length; i++)
            {
                perkSections[i].size = perkSize;
                perkSectionsChildPoint[i].size = connectSize;
                perkSectionsParentPoint[i].size = connectSize;
            }
        }

        //Update pos
        for (int i = 0; i < PerkTreeWindow.dataPerkTree.PerkList.Length; i++)
        {
            perkSections[i].position = PerkTreeWindow.dataPerkTree.PerkList[i].Pos + offset;
            perkSectionsChildPoint[i].center = new Vector2(perkSections[i].xMin, perkSections[i].yMin + perkSections[i].height / 2);
            perkSectionsParentPoint[i].center = new Vector2(perkSections[i].xMax, perkSections[i].yMin + perkSections[i].height / 2);
        }
    }

    void DrawUIInfo()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("center", GUILayout.Width(45))) offset = Vector2.zero;
        GUILayout.Label("Total: " + PerkTreeWindow.dataPerkTree.PerkList.Length);
        GUILayout.EndHorizontal();

        GUILayout.Label("LMB - Move view");

        GUILayout.Label("RMB - Move/Select perk");

    }

    void DrawWorkSpaceSection()
    {
        for (int i = 0; i < PerkTreeWindow.dataPerkTree.PerkList.Length; i++)
        {
            DrawPerkBG(i);
            DrawPerkData(i);
            DrawConnectPonts(i);
        }

        for (int i = 0; i < PerkTreeWindow.dataPerkTree.PerkList.Length; i++)
            DrawPerkLinks(i);
    }

    void DrawConnectPonts(int index)
    {
        GUI.DrawTexture(perkSectionsChildPoint[index], connectChildTexture);
        GUI.DrawTexture(perkSectionsParentPoint[index], connectParentTexture);
    }

    void DrawPerkBG(int index)
    {
        if (PerkTreeWindow.SelectedPerkObject == index)
            GUI.DrawTexture(perkSections[index], sel_perkAreaTexture);
        else
            GUI.DrawTexture(perkSections[index], def_perkAreaTexture);
    }

    void DrawPerkLinks(int perkindex)
    {
        var perkPos = new Vector2(perkSections[perkindex].xMax, perkSections[perkindex].yMax - perkSections[perkindex].height / 2);

        Handles.color = Color.green;
        foreach (var childIndex in PerkTreeWindow.dataPerkTree[perkindex].ChildsIndex)
        {
            var childPos = (perkPos + new Vector2(perkSections[childIndex].xMin, perkSections[childIndex].yMax - perkSections[childIndex].height / 2)) * 0.5f;
            Handles.DrawLine(
                perkPos,
                childPos
                );
        }

        perkPos = new Vector2(perkSections[perkindex].xMin, perkSections[perkindex].yMax - perkSections[perkindex].height / 2);

        Handles.color = Color.white;
        foreach (var parentIndex in PerkTreeWindow.dataPerkTree[perkindex].ParentsIndex)
        {
            var parentPos = (perkPos + new Vector2(perkSections[parentIndex].xMax, perkSections[parentIndex].yMax - perkSections[parentIndex].height / 2)) * 0.5f;
            Handles.DrawLine(
               perkPos,
               parentPos
               );
        }
    }

    void DrawPerkData(int index)
    {
        //Draw settings
        GUILayout.BeginArea(perkSections[index]);
        GUILayoutUtility.GetRect(1, 4);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Id: " + index);
        PerkTreeWindow.dataPerkTree[index].Perk = (Perk)EditorGUILayout.ObjectField(PerkTreeWindow.dataPerkTree[index].Perk, typeof(ScriptableObject), false);
        GUILayout.EndHorizontal();

        GUILayout.Label("  Perk:");

        GUILayout.EndArea();
    }
    #endregion


    public void MoveOffset(Vector2 pos)
    {
        offset += pos;
    }
    public void MoveObj(Vector2 pos)
    {
        offset += pos;
    }

    void Action(EventType type, Vector2 pos, int button)
    {
        switch (selType)
        {
            case SelType.isChild:
            case SelType.isParent:
                Handles.DrawLine(_lMouseTemp + secton.position, pos);
                break;
        }

        //normalize pos
        pos -= secton.position;

        if (type == EventType.MouseDown && button == 0)
        {
            Selection(pos);
            _lMouseTemp = pos;
        }


        if (selType == SelType.isContainer && type == EventType.MouseDrag && button == 0)
        {
            PerkTreeWindow.dataPerkTree[PerkTreeWindow.SelectedPerkObject].Pos += Vector2Int.FloorToInt(pos - (Vector2)_lMouseTemp);
            _lMouseTemp = pos;
        }

        if (type == EventType.MouseUp && button == 0)
        {
            int tempPerk = PerkTreeWindow.SelectedPerkObject;
            var tempSel = selType;
            Selection(pos);

            if (tempPerk != PerkTreeWindow.SelectedPerkObject)
                if (tempSel == SelType.isParent)
                {
                    if (PerkTreeWindow.ExistConnection(PerkTreeWindow.SelectedPerkObject, tempPerk))
                        PerkTreeWindow.Disconnect(tempPerk, PerkTreeWindow.SelectedPerkObject);
                    else
                        PerkTreeWindow.Connect(tempPerk, PerkTreeWindow.SelectedPerkObject);
                }
                else if (tempSel == SelType.isChild)
                {
                    if (PerkTreeWindow.ExistConnection(tempPerk, PerkTreeWindow.SelectedPerkObject))
                        PerkTreeWindow.Disconnect(PerkTreeWindow.SelectedPerkObject, tempPerk);
                    else
                        PerkTreeWindow.Connect(PerkTreeWindow.SelectedPerkObject, tempPerk);
                }

            selType = SelType.None;
        }
    }

    void Selection(Vector2 pos)
    {
        for (int i = perkSections.Length - 1; i >= 0; i--)
        {
            if (perkSectionsParentPoint[i].Contains(pos))
            {
                PerkTreeWindow.SelectedPerkObject = i;
                selType = SelType.isParent;
                return;
            }

            if (perkSectionsChildPoint[i].Contains(pos))
            {
                PerkTreeWindow.SelectedPerkObject = i;
                selType = SelType.isChild;
                return;
            }

            if (perkSections[i].Contains(pos))
            {
                PerkTreeWindow.SelectedPerkObject = i;
                selType = SelType.isContainer;
                return;
            }
        }
    }

    enum SelType
    {
        None,
        isContainer,
        isParent,
        isChild
    }
}
