using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;


public class ScenesManager : MonoSingleton<ScenesManager>
{
    //同步加载
    public void LoadScene(string name, System.Action fun)
    {
        SceneManager.LoadScene(name);
        fun();
    }
    
    //异步加载
    public void LoadSceneAsync(string name, System.Action fun)
    {
        StartCoroutine(ReallyLoadSceneAsync(name, fun));
    }
    
    private IEnumerator ReallyLoadSceneAsync(string name, System.Action fun)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(name);
        while(!ao.isDone)
        {
            //事件中心向外分发加载进度
            EventCenter.Instance.EventTrigger("加载进度条更新", ao.progress);
            yield return ao.progress;
        }
        EventCenter.Instance.EventTrigger("加载进度条更新", 1.0f);
        yield return new WaitForSeconds(1.0f);
        fun();
    }
}
