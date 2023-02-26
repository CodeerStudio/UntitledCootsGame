using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassGroundColor : MonoBehaviour{

    [SerializeField] private Color groundColor;

    private void OnValidate(){
        Shader.SetGlobalVector("_GroundColor", groundColor);
    }

    private void Start(){
        Shader.SetGlobalVector("_GroundColor", groundColor);
    }
}
