using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour{

    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private Text gameOverLabel;
    
    [SerializeField] private GameObject gameCompleteScreen;
    [SerializeField] private Text attemptsLabel;

    [SerializeField] private Animator screenTransition;

    [SerializeField] private AudioSource backgroundMusic;
    
    [SerializeField] private GameObject settingsScreen;

    [SerializeField] private AudioSource buttonAudio;
    
    [SerializeField] private AudioSource successAudio;
    [SerializeField] private Animator successNotification;
    [SerializeField] private Text successNotificationLabel;

    [SerializeField] private GameObject introDialogueCamera;
    [SerializeField] private GameObject gameCompletedCamera;

    private PlayerController playerController;
    private CinemachineFreeLook freeLookCamera;

    private bool gameOver;

    private bool gameCompleted;

    private readonly List<string> notificationsShown = new();

    private void Awake(){
        gameCompletedCamera.SetActive(false);
    }

    private void Start(){
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        playerController = FindObjectOfType<PlayerController>();
        freeLookCamera = FindObjectOfType<CinemachineFreeLook>();
        
        gameOverScreen.SetActive(false);
        settingsScreen.SetActive(false);
        
        gameCompleteScreen.SetActive(false);
    }

    private void Update(){
        if(gameOver && backgroundMusic.pitch > 0.7f)
            backgroundMusic.pitch -= Time.deltaTime * 0.5f;

        if(Input.GetButtonDown("Cancel"))
            Settings(!settingsScreen.activeSelf);
    }

    public void GameOver(string gameOverText){
        if(gameOver)
            return;

        PlayerPrefs.SetInt("Game over count", PlayerPrefs.GetInt("Game over count") + 1);
        
        gameOverLabel.text = gameOverText;
        
        gameOverScreen.SetActive(true);
        
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        gameOver = true;
    }

    public void Retry(){
        StartCoroutine(RestartScene());
    }
    
    public void Home(){
        StartCoroutine(RestartScene(true));
    }

    IEnumerator RestartScene(bool home = false){
        screenTransition.SetTrigger("Transition");

        yield return new WaitForSeconds(0.45f);

        int target = home ? 0 : SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(target);
    }
    
    public void Settings(bool show){
        if(show){
            if(!gameOver){
                settingsScreen.SetActive(true);

                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                playerController.enabled = false;
                freeLookCamera.enabled = false;
                
                buttonAudio.Play();
            }
        }
        else{
            StartCoroutine(HideSettings());
            
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            
            if(!introDialogueCamera.activeSelf)
                playerController.enabled = true;
            
            freeLookCamera.enabled = true;
            
            buttonAudio.Play();
        }
    }

    private IEnumerator HideSettings(){
        settingsScreen.GetComponent<Animator>().SetBool("Show", false);

        yield return new WaitForSeconds(1f/3f);
        
        settingsScreen.SetActive(false);
    }

    public void ShowSuccessNotification(string notification){
        if(notificationsShown.Contains(notification))
            return;
        
        notificationsShown.Add(notification);
        
        successAudio.Play();

        successNotificationLabel.text = notification;
        successNotification.SetTrigger("Show");
    }

    public void CompleteGame(){
        gameCompleted = true;
        
        gameCompletedCamera.SetActive(true);

        playerController.GameCompleted();
        
        Invoke(nameof(GameCompletedUI), 1.5f);
    }

    private void GameCompletedUI(){
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        int attempts = PlayerPrefs.GetInt("Game over count") + 1;
        attemptsLabel.text = "It only took " + attempts + " attempts!";
        gameCompleteScreen.SetActive(true);
    }
}
