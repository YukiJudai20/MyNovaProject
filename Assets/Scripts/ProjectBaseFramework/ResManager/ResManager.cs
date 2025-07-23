using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResManager : MonoSingleton<ResManager>
{
    private GameObject loaderObj;
    
    // 异步加载泛型资源
    public void LoadAssetAsync<T>(string path, System.Action<T> onComplete=null, System.Action<float> onProgress = null) 
        where T : UnityEngine.Object
    {
        StartCoroutine(LoadAssetCoroutine<T>(path, onComplete, onProgress));
    }
    
    private IEnumerator LoadAssetCoroutine<T>(string path, System.Action<T> onComplete, System.Action<float> onProgress) 
        where T : UnityEngine.Object
    {
        ResourceRequest request = Resources.LoadAsync<T>(path);
        
        while (!request.isDone)
        {
            onProgress?.Invoke(request.progress);
            yield return null;
        }
        
        if (request.asset == null)
        {
            Debug.LogError($"资源加载失败: {path}");
            onComplete?.Invoke(null);
            yield break;
        }

        T asset = request.asset as T;
        onComplete?.Invoke(asset);
        onProgress?.Invoke(1f); 
    }
    
    //异步加载GameObject，带加载进度
    public void LoadGameObjectAsync(string path,GameObject parent = null,System.Action<GameObject> onLoadComplete=null,
        System.Action<float> onProgress=null)
    {
        StartCoroutine(LoadResWithProgressCoroutine(path, parent,onLoadComplete,onProgress));
    }
    
    //异步加载资源协程，带加载进度
    private IEnumerator LoadResWithProgressCoroutine(string path, GameObject parent,System.Action<GameObject> onLoadComplete,
        System.Action<float> onProgress)
    {
        ResourceRequest request = Resources.LoadAsync<GameObject>(path);
        while (!request.isDone)
        {
            onProgress?.Invoke(request.progress);
            yield return null;
        }

        if (request.asset == null)
        {
            Debug.LogError("资源加载失败，请检查路径是否错误或文件是否存在！");
            onLoadComplete?.Invoke(null);
            yield break;
        }
        GameObject loadedObj = Instantiate(request.asset) as GameObject;
        if (parent != null)
        {
            loadedObj.transform.SetParent(parent.transform,false);
            loadedObj.transform.localPosition = Vector3.zero;
            loadedObj.transform.localRotation = Quaternion.identity;
            loadedObj.transform.localScale = Vector3.one;
        }
        onLoadComplete?.Invoke(loadedObj);
        onProgress?.Invoke(1.0f);
    }
}
