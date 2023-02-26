using System;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
public class Breakable : MonoBehaviour{

    [SerializeField] private float breakForce;
    [SerializeField] private GameObject broken;
    
    [SerializeField] private UnityEvent OnBreak;
    
    private AudioSource hitAudio;
    
    private Rigidbody rb;
    
    private float lastRbMagnitude;

    private void Start(){
        rb = GetComponent<Rigidbody>();
        hitAudio = GetComponent<AudioSource>();
    }

    private void Update(){
        lastRbMagnitude = rb.velocity.magnitude;
    }

    private void OnCollisionEnter(Collision other){
        if(other.gameObject.CompareTag("Player"))
            return;
        
        if(lastRbMagnitude > breakForce){
            if(broken != null){
                if(OnBreak != null)
                    OnBreak.Invoke();
                
                Instantiate(broken, transform.position, transform.rotation);
                Destroy(gameObject);
            }
            else if(hitAudio != null){
                hitAudio.pitch = Random.Range(1.1f, 1.3f);
                hitAudio.volume *= Random.Range(0.8f, 1.2f);
                
                hitAudio.Play();
            }
        }
    }
}
