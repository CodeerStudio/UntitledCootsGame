using System;
using UnityEngine;

public class Bird : MonoBehaviour{

    [SerializeField] private float flySpeed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float destroyDelay;
    [SerializeField] private RandomSFX takeoffSound;

    private Animator anim;
    
    private Vector3 flyDirection;
    private Quaternion targetRotation;

    private bool flying;

    private void Start(){
        anim = GetComponent<Animator>();
        
        flyDirection = -transform.forward + transform.up;
        flyDirection.Normalize();
        
        targetRotation = Quaternion.LookRotation(flyDirection);
    }

    private void Update(){
        if(!flying)
            return;

        transform.position += flyDirection * Time.deltaTime * flySpeed;

        float rotationStep = Time.deltaTime * turnSpeed;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationStep);
    }

    private void OnTriggerEnter(Collider other){
        if(!other.gameObject.CompareTag("Player") || flying)
            return;

        flying = true;
        anim.SetBool("Flying", true);
        
        takeoffSound.PlayRandom();
        
        Destroy(gameObject, destroyDelay);
    }
}
