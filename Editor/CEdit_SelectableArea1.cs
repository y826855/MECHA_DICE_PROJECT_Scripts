#if UNITY_EDITOR


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Linq;

//[CustomEditor(typeof(CSelectableArea_New), true)]
//public class CEdit_SelectableArea1 : Editor
//{
//    CSelectableArea_New select = null;

//    private void OnEnable()
//    {
//        select = target as CSelectableArea_New;
//    }

//    public override void OnInspectorGUI()
//    {
//        base.OnInspectorGUI();
//        if (GUILayout.Button("GET CHILDS")) 
//        {
//            EditorUtility.SetDirty(select);

//            if (select.m_CanvasGroup == null)
//                select.m_CanvasGroup = select.GetComponent<CanvasGroup>();

//            select.m_ChildSelectables.Clear();
//            var selectables = select.GetComponentsInChildren<Selectable>(true);
//            select.m_ChildSelectables.AddRange(selectables);
//        }
//    }
//}

#endif
