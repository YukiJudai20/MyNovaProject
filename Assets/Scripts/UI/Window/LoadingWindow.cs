using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class LoadingWindow : WindowBase
{
    public Text progressText;
    protected override void Awake()
    {
        base.Awake();
        EventCenter.Instance.AddEventListener<float>("加载进度条更新", UpdateLoadingProgress);
    }

    private void UpdateLoadingProgress(float progress)
    {
        progressText.text = (progress*100).ToString() + "%";   
    }

    public void FadeOut(float fadingSpeed = 1.5f)
    {
        //_canvasGroup.alpha = 1;
        _canvasGroup.DOFade(0, fadingSpeed);
        SetVisible(false);
        Debug.Log("淡出");
    }
    
    public void FadeIn(float fadingSpeed = 1.5f)
    {
        //_canvasGroup.alpha = 0;
        _canvasGroup.DOFade(1, fadingSpeed);
        SetVisible(true);
        Debug.Log("淡入");
    }
    

    protected override void OnDestroy()
    {
        base.OnDestroy();
        EventCenter.Instance.RemoveEventListener<float>("加载进度条更新",UpdateLoadingProgress);
    }
}
