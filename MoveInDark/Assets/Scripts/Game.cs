using System;
using System.Collections;
using Mono.CompilerServices.SymbolWriter;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    [SerializeField] private LayerMask finish;
    [SerializeField] private Transform start;
    [SerializeField] private Transform PlayerTransform;
    [SerializeField] private Rigidbody2D player;

    [SerializeField] private ParticleSystem finishBoom;
    [SerializeField] private ParticleSystem finishBoom2;

    [SerializeField] private LayerMask cheeky;
    
    private bool stopwatchActive;
    private float currentTime;
    public TextMeshProUGUI currentTimeText;

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject timer;
    [SerializeField] bool isPaused;

    [SerializeField] AudioSource music;
    
    private bool touchedCheeky;
    
    private MenuManager menuManager;
    void Awake()
    {
        GameManage.end = false;
        PlayerTransform.position = start.position;
    }

    private void Start()
    {
        if (GameManage.currentLevel == 5)
            timer.SetActive(false);
        stopwatchActive = true;
    }

    void Update()
    {
        if (player.IsTouchingLayers(finish))
        {
            StartCoroutine(Finish());
        }

        
        if (stopwatchActive == true)
        {
            currentTime = currentTime + Time.deltaTime;
            TimeSpan time = TimeSpan.FromSeconds(currentTime);
            currentTimeText.text = time.ToString(@"mm\:ss\:ff");
        }
        
        if (player.IsTouchingLayers(cheeky))
        {
            touchedCheeky = true;
            currentTime = 0;
            timer.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused == true)
            {
                Resume();
            }
            else if (isPaused == false)
            {
                PauseM();
            }
        }
        
        
        //Debug.Log(currentTime);
    }

    IEnumerator Finish()
    {
        stopwatchActive = false;
        switch (GameManage.currentLevel)
        {
            case 1:
            {
                GameManage.TimerL1 = currentTime;
                break;
            }
            case 2:
            {
                GameManage.TimerL2 = currentTime;
                break;
            }
            case 3:
            {
                GameManage.TimerL3 = currentTime;
                break;
            }
            case 4:
            {
                GameManage.TimerL4 = currentTime;
                break;
            }
            
        }
        finishBoom2.Play();
        finishBoom.Play();
        yield return new WaitForSeconds(1f);
        GameManage.end = true;
        SceneManager.LoadScene("Menu");
    }

    public void Resume()
    {
        Cursor.visible = false;
        music.UnPause();
        pauseMenu.SetActive(false);
        if (GameManage.currentLevel != 5)
        {
            timer.SetActive(true);
        }
        else if (GameManage.currentLevel == 5 && touchedCheeky == true)
        {
            timer.SetActive(true);
        }
        isPaused = false;
        Time.timeScale = 1f;
    }
    public void PauseM()
    {
        Cursor.visible = true;
        music.Pause();
        pauseMenu.SetActive(true);
        timer.SetActive(false);
        isPaused = true;
        Time.timeScale = 0f;
    }

    public void Menu()
    {
        //GameManage.end = true;
        currentTime = 0;
        SceneManager.LoadScene("Menu");
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene($"Level {GameManage.currentLevel}");
    }
}
