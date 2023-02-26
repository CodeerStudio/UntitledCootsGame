using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomSFX : MonoBehaviour {
    
    [SerializeField] private AudioSource effectAudio;
    [SerializeField] private AudioClip[] effectAudioClips;
    [SerializeField] private float effectMinPitch;
    [SerializeField] private float effectMaxPitch;

    [SerializeField] private float minVolume;
    [SerializeField] private float maxVolume;

    [SerializeField] private bool playOnStart;
    [SerializeField] private bool playOnCollision;

    private int sourceIndex;

    private void Start(){
        if(playOnStart)
            PlayRandom();
    }

    public void PlayRandom(){
        PlayRandom(1f);
    }

    public void PlayRandom(float pitchMultiplier, AudioSource target){
        effectAudio = target;
        
        PlayRandom(pitchMultiplier);
    }

    public void PlayRandom(float pitchMultiplier){
        AudioSource targetSource = effectAudio;
        
        if(minVolume != 0 || maxVolume != 0)
            targetSource.volume = Random.Range(minVolume, maxVolume);
        
        targetSource.clip = effectAudioClips[Random.Range(0, effectAudioClips.Length)];
        targetSource.pitch = Random.Range(effectMinPitch, effectMaxPitch) * pitchMultiplier;
        targetSource.Play();
    }

    private void OnCollisionEnter(Collision other){
        if(!playOnCollision || !other.gameObject.CompareTag("Player"))
            return;
        
        PlayRandom();
    }
}
