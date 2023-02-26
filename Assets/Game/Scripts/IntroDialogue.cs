using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class IntroDialogue : MonoBehaviour {
    
    [SerializeField] private RectTransform dialogueBox;
    [SerializeField] private Text dialogueText;

    [SerializeField] private GameObject dialogueCamera;
    [SerializeField] private Animator blackLines;

    [SerializeField] private string[] lines;

    [SerializeField] private AudioSource musicAudioSource;

    private AudioSource dialogueBoxAudio;
    
    private PlayerController playerController;

    private int currentLine;
    private float originalMusicVolume;

    private void Awake(){
        if(PlayerPrefs.GetInt("Intro") == 1){
            dialogueCamera.SetActive(false);
            gameObject.SetActive(false);
        }
        else{
            PlayerPrefs.SetInt("Intro", 1);
        }
    }

    private void Start(){
        playerController = FindObjectOfType<PlayerController>();
        playerController.enabled = false;
        
        dialogueText.text = lines[currentLine];

        dialogueBoxAudio = GetComponent<AudioSource>();

        originalMusicVolume = musicAudioSource.volume;
        musicAudioSource.volume = 0.12f;
    }

    private void Update(){
        if(Input.GetButtonDown("Jump") && currentLine < lines.Length)
            NextDialogue();
    }

    private void NextDialogue(){
        currentLine++;

        if(currentLine >= lines.Length){
            StartCoroutine(EndDialogue());
        }
        else{
            DialogueBoxEffect();

            dialogueText.text = lines[currentLine];
            dialogueBoxAudio.Play();
        }
    }

    private void DialogueBoxEffect(){
        dialogueBox.localScale = Vector3.one;
        dialogueBox.transform.DOPunchScale(Vector3.one * 0.14f, 0.2f, 3, 0.3f);
    }

    private IEnumerator EndDialogue(){
        playerController.enabled = true;
        blackLines.SetTrigger("Hide");
        
        dialogueCamera.SetActive(false);

        dialogueBox.GetComponent<CanvasGroup>().DOFade(0, 0.3f);
        musicAudioSource.DOFade(originalMusicVolume, 0.5f);

        yield return new WaitForSeconds(0.5f);
        
        gameObject.SetActive(false);
    }
}
