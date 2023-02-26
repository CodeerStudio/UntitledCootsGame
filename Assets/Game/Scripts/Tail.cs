using System;
using UnityEngine;

public class Tail : MonoBehaviour{

    [SerializeField] private float movementImpact;

    [SerializeField] private Transform root;
    [SerializeField] private Transform[] bones;
    
    [Tooltip("Single link rotate time")]
    public float delay = 0.1f;
    
    private Vector3[] forwards;
    private Vector3[] velocities;

    private Vector3 rootForward;
    private Vector3 rootVelocity;

    private Vector3 lastPos;

    private void Start(){
        forwards = new Vector3[bones.Length];
        
        for (int i = 0; i < bones.Length; i++)
            forwards[i] = bones[i].up;
            
        velocities = new Vector3[bones.Length];
        lastPos = root.position;
    }

    private void LateUpdate(){
        for (int i = 0; i < bones.Length; i++){
            Vector3 yMovement = (root.position - lastPos).y * Vector3.up;
            Vector3 target = bones[i].parent.up + (yMovement * movementImpact * (1f/(1f + i)));
            bones[i].up = forwards[i] = Vector3.SmoothDamp(forwards[i], target, ref velocities[i], delay);
        }
        lastPos = root.position;
    }
}
