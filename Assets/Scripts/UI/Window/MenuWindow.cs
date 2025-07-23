using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuWindow : WindowBase
{
    public Button chooseGameMode;
    public Button opentSettings;
    public Button quitGame;

    private void Start()
    {
        chooseGameMode.onClick.AddListener(() => GameManager.Instance.StartGame());
        opentSettings.onClick.AddListener(()=>OpenSettingWindow());
        quitGame.onClick.AddListener(()=>GameManager.Instance.QuitGame());
        
    }
    
    public void OpenSettingWindow()
    {
        UIManager.Instance.LoadWindow("SettingWindow");
    }
}
