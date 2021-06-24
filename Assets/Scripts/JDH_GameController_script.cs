using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class JDH_GameController_script : MonoBehaviour
{
    
    [System.Serializable]
    public class Components
    {
        public GameObject pausemenu;
        public AudioSource audiosource;
        public AudioClip buttonaccept;
        public AudioClip buttonback;

        public string crowURL = "https://twitter.com/Charlottezxz";
        public string sherbertURL = "https://twitter.com/JDSherbert_";

        public bool pausingfreezestime = true;
    }

    public Components component = new Components();

    public void Start()
    {
        Get();
    }

    public void Get()
    {
        if(GetComponent<AudioSource>())
            component.audiosource = GetComponent<AudioSource>();
    }

    public void Update()
    {
        OpenPauseMenu();
    }

    public void OpenPauseMenu()
    {
        if (Input.GetButtonDown("Cancel") && component.pausemenu.activeSelf == false)
        {
            component.audiosource.PlayOneShot(component.buttonaccept);
            component.pausemenu.SetActive(true);
            
        }
        else if(Input.GetButtonDown("Cancel") && component.pausemenu.activeSelf == false)
        {
            component.audiosource.PlayOneShot(component.buttonback);
            component.pausemenu.SetActive(false);

        }
        else
        {
            //Should never be here
        }
        if(component.pausingfreezestime == true) 
        {
            if (component.pausemenu.activeSelf == true)
                Time.timeScale = 0;

            if (component.pausemenu.activeSelf == false)
                Time.timeScale = 1;
        }

    }

    public void RestartGame()
    {
        int scene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }

    public void NextLevel()
    {
        int scene = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }

    public void LoadSpecificLevel(int level)
    {
        int scene = level;
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }

    public void LoadURL(string URL)
    {
        Debug.Log(URL);
        Application.OpenURL(URL);
    }

    public void QuitGame()
    {
        Debug.Log("Quit game");
        Application.Quit();
    }
}
