using System;
using UnityEngine;

public class FallTrigger : MonoBehaviour{

    public Action DetectedFirstItem;
    
    private PlayerController playerController;
    private Vector3 startPos;

    private bool detectedItems;

    private void Awake(){
        playerController = FindObjectOfType<PlayerController>();

        if(playerController != null)
            startPos = playerController.transform.position;
    }

    private void OnTriggerEnter(Collider other){
        if(!other.gameObject.CompareTag("Push item"))
            return;
        
        //check if player moved, just in case books fall down instantly
        if(playerController == null || Vector3.Distance(playerController.transform.position, startPos) > 0.05f){
            if(!detectedItems && DetectedFirstItem != null)
                DetectedFirstItem.Invoke();
            
            detectedItems = true;
        }
    }

    public bool DetectedFallenItems(){
        return detectedItems;
    }
}
