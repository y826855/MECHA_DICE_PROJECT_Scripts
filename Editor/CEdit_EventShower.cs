#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(CEventShower), true)]
public class CEdit_EventShower : Editor
{
    CEventShower select = null;

    private void OnEnable()
    {
        select = target as CEventShower;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorUtility.SetDirty(select);

        if (GUILayout.Button("GET CHILD EVENT UNITS"))
        {
            select.m_EventUnits.Clear();

            foreach (Transform it in select.transform) 
            {
                var unit = it.GetComponent<CEvent_Unit>();
                select.m_EventUnits.Add(unit.m_ID, unit);
            }
            
        }
    }
}

#endif