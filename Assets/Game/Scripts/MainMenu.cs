using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour{

    [SerializeField] private Transform menuCamera;
    [SerializeField] private Animator transition;
    
    [SerializeField] private GameObject settingsScreen;
    [SerializeField] private GameObject creditsScreen;

    [SerializeField] private Button continueButton;

    private bool inTransition;

    private void Start(){
        settingsScreen.SetActive(false);
        creditsScreen.SetActive(false);

        if(PlayerPrefs.GetInt("Intro") == 0)
            continueButton.interactable = false;
    }

    private void Update(){
        if(inTransition)
            menuCamera.position += menuCamera.forward * Time.deltaTime * 12;
    }
    
    public void NewGame(){
        PlayerPrefs.DeleteAll();
        
        Continue();
    }

    public void Continue(){
        if(inTransition)
            return;

        inTransition = true;
        StartCoroutine(StartGame());
    }

    IEnumerator StartGame(){
        transition.SetTrigger("Transition");

        yield return new WaitForSeconds(0.45f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Settings(bool show){
        if(show){
            settingsScreen.SetActive(true);
        }
        else{
            StartCoroutine(HideSettings());
        }
    }

    private IEnumerator HideSettings(){
        settingsScreen.GetComponent<Animator>().SetBool("Show", false);

        yield return new WaitForSeconds(1f/3f);
        
        settingsScreen.SetActive(false);
    }
    
    public void Credits(bool show){
        if(show){
            creditsScreen.SetActive(true);
        }
        else{
            StartCoroutine(HideCredits());
        }
    }

    private IEnumerator HideCredits(){
        creditsScreen.GetComponent<Animator>().SetBool("Show", false);

        yield return new WaitForSeconds(1f/3f);
        
        creditsScreen.SetActive(false);
    }

    public void Quit(){
        Application.Quit();
    }
}
