using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SetFieldBaseBuilder : MonoBehaviour {

    CaptureBase thisBase;
    
    void setFields()
    {
        thisBase = this.GetComponentInParent<CaptureBase>();
        thisBase.BaseBuilder = this.transform.Find("BaseBuilder").gameObject;
    }

    void Awake()
    {
        Debug.Log("Awake");
        setFields();
    }
}
