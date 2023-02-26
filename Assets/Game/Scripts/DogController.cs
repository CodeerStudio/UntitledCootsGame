using System;
using System.Collections;
using UnityEngine;

public class DogController : MonoBehaviour{

    [SerializeField] private Transform dog;
    [SerializeField] private float dogMoveSpeed;
    [SerializeField] private float dogTurnSpeed;
    [SerializeField] private ParticleSystem dogDistractedParticles;
    
    [SerializeField] private Transform distractedTarget;
    [SerializeField] private Collider attackTrigger;

    [SerializeField] private GameObject attackCamera;

    private float stoppingDistance = 0.1f;

    private Animator dogAnim;
    private RandomSFX dogBark;

    private PlayerController playerController;
    private GameManager gameManager;

    private bool isDistracted;
    private Transform currentAttackTarget;

    private void Awake(){
        attackCamera.SetActive(false);
    }

    private void Start(){
        dogAnim = dog.GetComponent<Animator>();
        dogBark = dog.GetComponent<RandomSFX>();

        playerController = FindObjectOfType<PlayerController>();
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Update(){
        if(currentAttackTarget != null){
            float dist = Vector3.Distance(dog.position, currentAttackTarget.position);
            
            if(dist > stoppingDistance){
                float moveStep = Time.deltaTime * dogMoveSpeed;
                dog.position = Vector3.MoveTowards(dog.position, currentAttackTarget.position, moveStep);

                if(Vector3.Distance(dog.position, currentAttackTarget.position) <= stoppingDistance){
                    dogAnim.SetBool("Barking", true);
                    dogDistractedParticles.Play();
                }
            }

            Vector3 targetDir = currentAttackTarget.forward;

            if(currentAttackTarget == playerController.transform)
                targetDir = (playerController.transform.position - transform.position).normalized;
            
            Quaternion target = Quaternion.LookRotation(targetDir);
            dog.rotation = Quaternion.RotateTowards(dog.rotation, target, Time.deltaTime * dogTurnSpeed);
        }
    }

    public void GetDistracted(){
        isDistracted = true;
        currentAttackTarget = distractedTarget;

        attackTrigger.enabled = false;
        dogAnim.SetBool("Walking", true);

        StartCoroutine(BarkSound());
    }

    private IEnumerator BarkSound(){
        yield return new WaitForSeconds(0.7f);
        dogBark.PlayRandom();
    }
    
    private void OnTriggerEnter(Collider other){
        if(!other.gameObject.CompareTag("Player"))
            return;
        
        StartCoroutine(AttackCoots());
    }
    
    private IEnumerator AttackCoots(){
        attackCamera.SetActive(true);

        StartCoroutine(playerController.AttackedByDog());
        
        currentAttackTarget = playerController.transform;
        
        dogAnim.SetBool("Running", true);
        
        dogMoveSpeed *= 2f;
        dogTurnSpeed *= 3f;
        stoppingDistance = 2f;

        yield return new WaitForSeconds(2f);
        
        gameManager.GameOver("COOTS GOT KILLED BY A DOG.");
    }

    public void EscapedGarden(){
        if(!isDistracted)
            return;
        
        gameManager.ShowSuccessNotification("Distract the dog");
    }
}
