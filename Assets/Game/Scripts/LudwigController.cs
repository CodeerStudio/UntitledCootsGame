using System;
using System.Collections;
using cakeslice;
using DG.Tweening;
using UnityEngine;

public class LudwigController : MonoBehaviour{

    [SerializeField] private Transform ludwig;
    [SerializeField] private GameObject ludwigDoorCamera;
    [SerializeField] private float cameraZoomDuration;
    [SerializeField] private FallTrigger shelfFallTrigger;
    [SerializeField] private Transform cleanUpTarget;
    [SerializeField] private Transform outsideTarget;
    [SerializeField] private float ludwigTurnSpeed;
    [SerializeField] private float ludwigMoveSpeed;
    [SerializeField] private GameObject exclamationMark;
    [SerializeField] private ParticleSystem annoyedParticles;
    [SerializeField] private RandomSFX annoyedSFX;
    [SerializeField] private Outline[] activateOutlines;

    private PlayerController playerController;
    private GameManager gameManager;

    private Animator ludwigAnim;
    private bool cleaningUp;

    private Transform moveTarget;

    private void Start(){
        ludwigAnim = ludwig.GetComponent<Animator>();
        ludwigDoorCamera.SetActive(false);

        playerController = FindObjectOfType<PlayerController>();
        gameManager = FindObjectOfType<GameManager>();
        
        for (int i = 0; i < activateOutlines.Length; i++){
            activateOutlines[i].enabled = false;
        }

        if(shelfFallTrigger != null)
            shelfFallTrigger.DetectedFirstItem += CleanUpMess;
    }

    private void Update(){
        if(moveTarget != null){
            Vector3 dir = (moveTarget.position - ludwig.transform.position).normalized;
            Quaternion target = Quaternion.LookRotation(dir);
            ludwig.transform.rotation = Quaternion.RotateTowards(ludwig.transform.rotation, target, Time.deltaTime * ludwigTurnSpeed);

            ludwig.transform.position += ludwig.transform.forward * Time.deltaTime * ludwigMoveSpeed;

            if(Vector3.Distance(ludwig.transform.position, moveTarget.position) < 1f){
                ludwigAnim.SetTrigger("Angry");
                
                annoyedParticles.Play();
                annoyedSFX.PlayRandom();
                
                moveTarget = null;
            }
        }
    }

    private void CleanUpMess(){
        moveTarget = cleanUpTarget;
        ludwigAnim.SetBool("Walking", true);
        
        annoyedSFX.PlayRandom();
    }

    private void ShowDoorCamera(){
        ludwigDoorCamera.SetActive(true);
        Vector3 target = ludwigDoorCamera.transform.position + ludwigDoorCamera.transform.forward * 4;

        ludwigDoorCamera.transform.DOMove(target, cameraZoomDuration).OnComplete(() => {
            Debug.Log("game over");
        });
    }
    
    private void OnTriggerEnter(Collider other){
        if(!other.gameObject.CompareTag("Player"))
            return;

        if(!ludwigAnim.GetBool("Walking")){
            ShowDoorCamera();

            playerController.Caught();

            //Invoke(nameof(ChaseCoots), 1.5f);
            StartCoroutine(ChaseCoots());
        }
        else{
            gameManager.ShowSuccessNotification("Escape the house");

            for (int i = 0; i < activateOutlines.Length; i++){
                activateOutlines[i].enabled = true;
            }
        }
    }

    private IEnumerator ChaseCoots(){
        ludwigTurnSpeed /= 5;
        moveTarget = outsideTarget;
        
        exclamationMark.SetActive(true);
        exclamationMark.transform.DOPunchScale(Vector3.one * 0.5f, 0.7f, 1, 0);
        
        ludwigAnim.SetBool("Walking", true);

        yield return new WaitForSeconds(1f);
        
        gameManager.GameOver("COOTS GOT CAUGHT SNEAKING OUT.");
    }
}
