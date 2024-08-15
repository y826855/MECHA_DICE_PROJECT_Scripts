using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[ExecuteInEditMode]
[CustomEditor(typeof(CUI_Dynamic_Group))]
public class CUI_Edit_Dynamic_Group : Editor
{
    // Start is called before the first frame update

    CUI_Dynamic_Group _select = null;

    private void OnEnable()
    {
        _select = target as CUI_Dynamic_Group;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("GET ALL CHILDS")) 
        {
            _select.m_Childs.Clear();
            _select.m_Childs.AddRange(_select.GetComponentsInChildren<CUI_Dynamic>());
        }
    }

}
