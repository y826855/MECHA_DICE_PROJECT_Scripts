#if UNITY_EDITOR


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Linq;

//[CustomEditor(typeof(CSelectableAreaGroup), true)]
//public class CEdit_SelectableAreaGroup : Editor
//{
//    CSelectableAreaGroup select = null;

//    private void OnEnable()
//    {
//        select = target as CSelectableAreaGroup;
//    }

//    public override void OnInspectorGUI()
//    {
//        base.OnInspectorGUI();
//        if (GUILayout.Button("GET CHILDS")) 
//        {
//            select.m_ChildAreas.Clear();
//            var selectables = select.GetComponentsInChildren<CSelectableArea_New>(true);
//            select.m_ChildAreas.AddRange(selectables);
//            foreach (var it in selectables)
//                it.m_Parent = select;
//        }
//    }
//}

#endif