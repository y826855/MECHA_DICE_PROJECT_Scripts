using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CUI_Field : MonoBehaviour
{
    void Start()
    {
        LookAtCamera();
    }


    public void LookAtCamera() 
    {
        //this.transform.LookAt(Camera.main.transform);
        this.transform.forward = -Camera.main.transform.forward;
    }
}
