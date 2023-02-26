using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour{

    public Action JustLanded;

    [SerializeField] private MeowController meowController;
    
    public LayerMask walkable;

    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;

    [SerializeField] private float baseRotationSpeed;
    
    [SerializeField] private float angleRotationMultiplier;
    [SerializeField] private float angleRotationMultiplierSprint;

    [SerializeField] private float minSitWait;
    [SerializeField] private float maxSitWait;

    [SerializeField] private float idleBlendSpeed;

    [SerializeField] private float jumpForce;

    [SerializeField] private AudioSource jumpAudio;
    
    [SerializeField] private AudioSource screamAudio;
    [SerializeField] private ParticleSystem bloodParticles;

    [SerializeField] private GameObject carsCamera;

    private GameManager gameManager;

    private Rigidbody rb;

    private float h;
    private float v;
    private Vector3 input;
    private bool sprinting;
    private bool rawInput;

    private Transform mainCam;
    private bool grounded;

    private float sitWait;
    private float idleTimer;

    private Animator anim;
    private ConstantForce constantForce;

    private float idleBlend;

    private bool doJump;

    private bool wasGrounded;

    private bool wasCaught;

    private void Awake(){
        carsCamera.SetActive(false);
    }

    private void Start(){
        rb = GetComponent<Rigidbody>();
        anim = transform.GetChild(0).GetComponent<Animator>();
        constantForce = GetComponent<ConstantForce>();

        gameManager = FindObjectOfType<GameManager>();

        mainCam = Camera.main.transform;

        sitWait = Random.Range(minSitWait, maxSitWait);
    }

    private void OnEnable(){
        meowController.Meow();
    }

    private void Update(){
        grounded = Physics.CheckSphere(transform.position, 0.25f, walkable, QueryTriggerInteraction.Ignore);
        constantForce.enabled = !grounded;
        
        Vector3 camForward = mainCam.transform.forward;
        camForward.y = 0;
        camForward.Normalize();

        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        input = (h * mainCam.transform.right) + (v * camForward);

        bool rawInputNew = Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;

        if(rawInputNew != rawInput){
            if(rawInputNew && Random.Range(0, 2) == 0)
                meowController.Meow();
            
            if(!rawInputNew)
                idleTimer = 0;
        }

        rawInput = rawInputNew;

        sprinting = Input.GetButton("Sprint") && rawInput;

        if(rawInput){
            Quaternion target = Quaternion.LookRotation(input);
            float angle = Quaternion.Angle(target, transform.rotation);

            float angleMultiplier = sprinting || !grounded ? angleRotationMultiplierSprint : angleRotationMultiplier;
            float rotationSpeed = baseRotationSpeed + (angleMultiplier * angle);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, target, Time.deltaTime * rotationSpeed);
        }

        if(!rawInput){
            idleTimer += Time.deltaTime;

            if(idleTimer > sitWait && idleBlend < 1){
                idleBlend += Time.deltaTime * idleBlendSpeed;
                
                if(idleBlend >= 1)
                    meowController.Meow();
            }
        }
        else{
            idleBlend = 0;
        }

        if(Input.GetButtonDown("Jump") && grounded){
            doJump = true;

            jumpAudio.pitch = Random.Range(0.9f, 1.1f);
            jumpAudio.Play();
        }

        UpdateAnimations();
        
        if(grounded && !wasGrounded && JustLanded != null)
            JustLanded.Invoke();

        wasGrounded = grounded;
    }

    private void UpdateAnimations(){
        anim.SetBool("Walking", input != Vector3.zero && rawInput);
        anim.SetBool("Running", sprinting);
        
        anim.SetFloat("IdleBlend", idleBlend);
        anim.SetBool("Jumping", !grounded);
        
        anim.SetFloat("Horizontal", v > 0 ? h * v : 0);
    }

    private void FixedUpdate(){
        if(rawInput){
            float speed = sprinting ? sprintSpeed : walkSpeed;

            if(!grounded)
                speed = Mathf.Max(speed, walkSpeed * 2.5f);
            
            rb.MovePosition(transform.position + transform.forward * Time.deltaTime * speed);
        }

        if(doJump){
            rb.AddForce(Vector3.up * jumpForce);

            doJump = false;
        }
    }
    
    private void ResetAnimations(bool walking){
        anim.SetBool("Walking", walking);
        anim.SetBool("Running", false);
        anim.SetBool("Jumping", false);
        anim.SetFloat("Horizontal", 0);
    }

    public bool HasMoveInput(){
        return input != Vector3.zero;
    }

    public void Caught(){
        wasCaught = true;

        GetComponent<Collider>().enabled = false;
        
        float walkDistance = 15;
        Vector3 target = transform.position + Vector3.forward * walkDistance;
        transform.DOMove(target, walkDistance / walkSpeed);

        ResetAnimations(true);

        this.enabled = false;
    }

    public IEnumerator AttackedByDog(){
        yield return new WaitForSeconds(0.6f);
        
        screamAudio.Play();

        bloodParticles.transform.parent = null;
        bloodParticles.Play();

        ResetAnimations(false);
        
        transform.Rotate(transform.right * 90);
        
        this.enabled = false;
    }

    public void HitByCar(Vector3 driveDirection){
        if(!this.enabled)
            return;
        
        rb.AddForce(driveDirection * 2000);

        ResetAnimations(false);
        
        screamAudio.Play();
        bloodParticles.Play();
        
        this.enabled = false;
        
        gameManager.GameOver("COOTS GOT HIT BY A CAR.");
        
        carsCamera.SetActive(true);
    }

    public void GameCompleted(){
        ResetAnimations(false);
        anim.SetFloat("IdleBlend", 1);
        meowController.Meow();
        this.enabled = false;
    }

    public bool WasCaught(){
        return wasCaught;
    }
}
