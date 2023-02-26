using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class VehicleController : MonoBehaviour{

    [SerializeField] private Transform[] wheels;

    private float wheelRotationSpeed = 10;
    private float startingPointZ;

    private RoadManager roadManager;

    private float drivingSpeed;
    private float drivingDistance;

    private int vehicleType;

    public void Initialize(RoadManager roadManager, int vehicleType, float drivingSpeed, float drivingDistance){
        this.roadManager = roadManager;
        this.vehicleType = vehicleType;
        this.drivingSpeed = drivingSpeed * Random.Range(0.96f, 1.04f);
        this.drivingDistance = drivingDistance;
        
        startingPointZ = transform.position.z;
    }

    private void Update(){
        for (int i = 0; i < wheels.Length; i++){
            wheels[i].Rotate(transform.right * wheelRotationSpeed);
        }

        transform.position += transform.forward * Time.deltaTime * drivingSpeed;

        if(Mathf.Abs(transform.position.z - startingPointZ) > drivingDistance)
            roadManager.Deactivate(gameObject, vehicleType);
    }

    private void OnCollisionEnter(Collision other){
        if(!other.gameObject.CompareTag("Player"))
            return;

        other.gameObject.GetComponent<PlayerController>().HitByCar(transform.forward);
    }
}
