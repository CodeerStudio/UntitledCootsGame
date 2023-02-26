using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class JumpEffect : MonoBehaviour{

    [SerializeField] private float impact = 0.13f;
    [SerializeField] private float duration = 0.3f;
    [SerializeField] private bool punchScale;

    [SerializeField] private float sinEffect;
    [SerializeField] private float sinSpeed;

    [SerializeField] private AudioSource jumpEffectAudio;
    [SerializeField] private AudioClip[] jumpEffectAudioClips;
    [SerializeField] private float jumpEffectMinPitch;
    [SerializeField] private float jumpEffectMaxPitch;

    [SerializeField] private UnityEvent effectEvent;

    [SerializeField] private Transform targetTransform;
    
    private float sinTimer;
    private float timer = 0;

    private void Start(){
        if(targetTransform == null)
            targetTransform = transform;
    }

    private void Update(){
        if(sinEffect != 0 && sinSpeed != 0 && timer <= 0){
            sinTimer += Time.deltaTime;
            
            targetTransform.Translate(Vector3.up * sinEffect * Time.deltaTime * Mathf.Sin(sinTimer * sinSpeed), Space.World);
        }

        if(timer > 0)
            timer -= Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other){
        if(!other.gameObject.CompareTag("Player") || timer > 0)
            return;

        timer = duration * 1.2f;

        if(jumpEffectAudio != null && jumpEffectAudioClips.Length > 0){
            jumpEffectAudio.clip = jumpEffectAudioClips[Random.Range(0, jumpEffectAudioClips.Length)];
            jumpEffectAudio.pitch = Random.Range(jumpEffectMinPitch, jumpEffectMaxPitch);
            jumpEffectAudio.Play();
        }

        Vector3 pos = targetTransform.position;
        
        if(punchScale)
            targetTransform.DOPunchScale(new Vector3(impact * 0.3f, 0, impact * 0.3f), duration, 2, 1);
        
        targetTransform.DOPunchPosition(-Vector3.up * impact, duration, 2, 1).OnComplete(() => {
            targetTransform.position = pos;
        });
        
        effectEvent.Invoke();
    }
}
