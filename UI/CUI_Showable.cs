using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUI_Showable : MonoBehaviour
{

    //TODO : ¾È¾¸
    public virtual T GetScriptableData<T>(T _inst) where T : ScriptableObject
    { return _inst; }
    
}
