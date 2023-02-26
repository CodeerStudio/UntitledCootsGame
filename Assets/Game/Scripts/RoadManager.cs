using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class Vehicle{
    public GameObject vehicle;

    [HideInInspector] 
    public List<GameObject> storage = new();
}

public class RoadManager : MonoBehaviour{

    [SerializeField] private Vehicle[] vehicles;
    [SerializeField] private Transform[] lanes;

    [SerializeField] private float minSpawnWait;
    [SerializeField] private float maxSpawnWait;

    [SerializeField] private float driveSpeed;
    [SerializeField] private float driveDistance;

    [SerializeField] private int twoCarChance;
    
    private bool spawning = true;

    private void Start(){
        StartCoroutine(SpawnCars());
    }

    private IEnumerator SpawnCars(){
        while (spawning){
            int numberOfCars = Random.Range(0, twoCarChance) == 0 ? 2 : 1;
            int spawnedLane = -1;

            for (int i = 0; i < numberOfCars; i++){
                int randomLane = Random.Range(0, lanes.Length);
                
                if(randomLane == spawnedLane)
                    continue;

                if(i > 0)
                    yield return new WaitForSeconds(Random.Range(0.1f, 0.25f));

                spawnedLane = randomLane;

                int randomVehicleIndex = Random.Range(0, vehicles.Length);
                Vehicle randomVehicle = vehicles[randomVehicleIndex];
                GameObject vehicleToSpawn = null;

                if(randomVehicle.storage.Count > 0){
                    vehicleToSpawn = randomVehicle.storage[0];
                    randomVehicle.storage.RemoveAt(0);
                    vehicleToSpawn.SetActive(true);
                }
                else{
                    vehicleToSpawn = Instantiate(randomVehicle.vehicle);
                }

                vehicleToSpawn.transform.position = lanes[randomLane].position;
                vehicleToSpawn.transform.rotation = lanes[randomLane].rotation;

                float speed = randomLane == 0 ? driveSpeed * 1.1f : driveSpeed * 0.9f;
                vehicleToSpawn.GetComponent<VehicleController>().Initialize(this, randomVehicleIndex, speed, driveDistance);
            }

            float waitTime = Random.Range(minSpawnWait, maxSpawnWait);

            if(Random.Range(0, 8) == 0)
                waitTime += maxSpawnWait/2;
            
            yield return new WaitForSeconds(waitTime);
        }
    }

    public void Deactivate(GameObject vehicle, int vehicleType){
        vehicle.SetActive(false);
        vehicles[vehicleType].storage.Add(vehicle);
    }
    
    
}
