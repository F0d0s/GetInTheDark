using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    [SerializeField] public GameObject levelSelect;
    [SerializeField] public GameObject mainMenu;

    [SerializeField] private TextMeshProUGUI timer1;
    [SerializeField] private TextMeshProUGUI timer2;
    [SerializeField] private TextMeshProUGUI timer3;
    [SerializeField] private TextMeshProUGUI timer4;

    
    private void Start()
    {
        Time.timeScale = 1f;
        Cursor.visible = true;
        Debug.Log(GameManage.TimerL1);
        Debug.Log(GameManage.TimerL2);
        Debug.Log(GameManage.TimerL3);
        Debug.Log(GameManage.TimerL4);
        if (GameManage.end == true)
        {
            LevelSelect();
        }
        TimeSpan time1 = TimeSpan.FromSeconds(GameManage.TimerL1);
        timer1.text = time1.ToString(@"mm\:ss\:ff");
        TimeSpan time2 = TimeSpan.FromSeconds(GameManage.TimerL2);
        timer2.text = time2.ToString(@"mm\:ss\:ff");
        TimeSpan time3 = TimeSpan.FromSeconds(GameManage.TimerL3);
        timer3.text = time3.ToString(@"mm\:ss\:ff");
        TimeSpan time4 = TimeSpan.FromSeconds(GameManage.TimerL4);
        timer4.text = time4.ToString(@"mm\:ss\:ff");
        
        GameManage.currentLevel = 0;
    }

    public void Tutorial()
    {
        GameManage.currentLevel = 5;
        Cursor.visible = false;
        SceneManager.LoadScene("Tutorial");
    }
    public void Level1()
    {
        GameManage.currentLevel = 1;
        Cursor.visible = false;
        SceneManager.LoadScene("Level 1");
    }
    public void Level2()
    {
        GameManage.currentLevel = 2;
        Cursor.visible = false;
        SceneManager.LoadScene("Level 2");
    }
    public void Level3()
    {
        GameManage.currentLevel = 3;
        Cursor.visible = false;
        SceneManager.LoadScene("Level 3");
    }
    public void Level4()
    {
        GameManage.currentLevel = 4;
        Cursor.visible = false;
        SceneManager.LoadScene("Level 4");
    }
    public void LevelSelect()
    {
        levelSelect.SetActive(true);
        mainMenu.SetActive(false);
    }
    public void MainMenuBack()
    {
        levelSelect.SetActive(false);
        mainMenu.SetActive(true);
    }
    public void Quit()
    {
        Application.Quit();
        Debug.Log("Game Quit");
    }
}
