using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SimpleTrigger : MonoBehaviour {
    
    [SerializeField] private UnityEvent OnTrigger;
    
    private void OnTriggerEnter(Collider other){
        if(!other.gameObject.CompareTag("Player"))
            return;
        
        if(OnTrigger != null)
            OnTrigger.Invoke();
    }
}
