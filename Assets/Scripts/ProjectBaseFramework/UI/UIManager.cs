using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoSingleton<UIManager>
{
    public GameObject UIRoot;
    private Dictionary<string, GameObject> mAllWindowDic = new Dictionary<string, GameObject>();
    private GameObject curWindow;
    private LoadingWindow loadingWindow;

    private void Awake()
    {
        if (UIRoot == null)
        {
            UIRoot = GameObject.Find("UIRoot");
            if (UIRoot == null)
            {
                UIRoot = new GameObject("UIRoot");
            }
        }
        
        //因为加载窗口经常要用，所以在一开始就加载出来，之后也不会删除
        ResManager.Instance.LoadGameObjectAsync("UI/Window/LoadingWindow", UIRoot, (obj) =>
        {
            loadingWindow = obj.GetComponent<LoadingWindow>();
            loadingWindow.SetVisible(false);
        });
        DontDestroyOnLoad(UIRoot);
    }

    public void LoadWindow(string windowName)
    {
        if (UIRoot != null)
        {
            if (mAllWindowDic.ContainsKey(windowName))
            {
                SetWindowVisible(windowName,true);
                curWindow = mAllWindowDic[windowName];
            }
            else
            {
                ResManager.Instance.LoadGameObjectAsync("UI/Window/"+windowName,UIRoot,(windowObj)=>
                {
                    mAllWindowDic[windowName] = windowObj;
                    curWindow = windowObj;
                });            
            }
        }
    }

    public void SetWindowVisible(string windowName, bool isVisible)
    {
        if (mAllWindowDic.TryGetValue(windowName, out var windowObj))
        {
            windowObj.GetComponent<WindowBase>().SetVisible(isVisible);
        }
        else
        {
            LoadWindow(windowName);
        }
    }

    public void LoadingDungeonWorld()
    {
        DestroyAllWindow();
        loadingWindow.FadeIn();
    }

    public void LoadingDungeonWorldDone()
    {
        LoadWindow("MinimapWindow");
        loadingWindow.FadeOut();
    }

    public void LoadingBattleWorld()
    {
        DestroyAllWindow();
        loadingWindow.FadeIn();
    }

    public void LoadingBattleWorldDone()
    {
        LoadWindow("BattleWindow");
        loadingWindow.FadeOut();
    }
    
    
    //只有切换场景时才会销毁窗口，配合下面的销毁所有窗口使用
    public void DestroyWindow(string windowName)
    {
        if (mAllWindowDic.TryGetValue(windowName, out var windowObj))
        {
            SetWindowVisible(windowName,false);
            GameObject.Destroy(windowObj);
        }
    }

    public void DestroyAllWindow()
    {
        foreach (var keyValuePair in mAllWindowDic)
        {
            string key = keyValuePair.Key;
            DestroyWindow(key);
        }
        mAllWindowDic.Clear();
        Resources.UnloadUnusedAssets();
    }

    private void OnDestroy()
    {
        
    }
}
