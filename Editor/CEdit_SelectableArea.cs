#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Linq;

//[CustomEditor(typeof(CSelectableArea), true)]
//public class CEdit_SelectableArea : Editor
//{
//    CSelectableArea select = null;

//    private void OnEnable()
//    {
//        select = target as CSelectableArea;
//    }

//    public override void OnInspectorGUI()
//    {
//        base.OnInspectorGUI();
//        if (GUILayout.Button("GET CHILDS")) 
//        {
//            select.m_ChildSelectables.Clear();
//            var selectables = select.GetComponentsInChildren<Selectable>(true);
//            select.m_ChildSelectables.AddRange(selectables);
//        }
//    }
//}

#endif