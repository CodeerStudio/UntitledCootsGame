using System;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class FootstepSurface{
    public string surfaceTag;
    public AudioClip[] footstepSounds;
    public float volume;
    public float minWaitTime;
}

public class Footsteps : MonoBehaviour{

    [SerializeField] private AudioSource footstepSource;

    [Header("Surface based sounds")] 
    [SerializeField] private FootstepSurface[] surfaces;
    
    [Header("Default sounds")]
    [SerializeField] private AudioClip[] footstepSounds;
    [SerializeField] private float defaultVolume;
    
    [Space]
    [SerializeField] private float minPitch;
    [SerializeField] private float maxPitch;

    private PlayerController playerController;

    private float timer;

    private void Start(){
        playerController = transform.root.GetComponent<PlayerController>();

        if(playerController == null)
            playerController = FindObjectOfType<PlayerController>();

        if(playerController != null)
            playerController.JustLanded += PlayFootstep;
    }

    private void Update(){
        if(timer < 15){
            timer += Time.deltaTime;
        }
    }

    public void PlayFootstepSound(){
        if(!playerController.HasMoveInput())
            return;
        
        PlayFootstep(false);
    }
    
    private void PlayFootstep(){
        PlayFootstep(true);
    }

    private void PlayFootstep(bool loudStep){
        AudioClip randomFootstep = null;
        float vol = defaultVolume;
        
        if(Physics.SphereCast(transform.position + Vector3.up, 0.5f, -Vector3.up, out RaycastHit hit, 1.3f, playerController.walkable)){
            foreach(FootstepSurface s in surfaces){
                if(hit.collider.gameObject.CompareTag(s.surfaceTag)){
                    randomFootstep = s.footstepSounds[Random.Range(0, s.footstepSounds.Length)];
                    vol = s.volume;

                    if(s.minWaitTime != 0 && s.minWaitTime > timer)
                        return;

                    break;
                }
            }
        }

        if(randomFootstep == null && footstepSounds.Length > 0)
            randomFootstep = footstepSounds[Random.Range(0, footstepSounds.Length)];

        if(loudStep)
            vol *= 1.6f;

        timer = 0;
        
        footstepSource.clip = randomFootstep;
        footstepSource.volume = vol;

        footstepSource.pitch = Random.Range(minPitch, maxPitch);
        footstepSource.Play();
    }
}
