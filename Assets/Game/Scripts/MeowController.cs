using System;
using UnityEngine;

[RequireComponent(typeof(RandomSFX))]
public class MeowController : MonoBehaviour{

    private RandomSFX randomSFX;

    private float meowTimer;

    private void Start(){
        randomSFX = GetComponent<RandomSFX>();
    }

    private void Update(){
        meowTimer += Time.deltaTime;
    }

    public void Meow(){
        if(meowTimer < 2f)
            return;
        
        randomSFX.PlayRandom();
    }

    /*private void Update(){
        if(Input.GetButton("Fire1"))
            randomSFX.PlayRandom();
    }*/
}
