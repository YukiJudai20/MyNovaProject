using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PoolData
{
    public GameObject fatherObj;
    public List<GameObject> poolList;

    public PoolData(GameObject obj, GameObject poolObj)
    {
        fatherObj = new GameObject(obj.name);
        fatherObj.transform.parent = poolObj.transform;
        poolList = new List<GameObject>() {};
        PushObj(obj);
    }
    
    public void PushObj(GameObject obj)
    {
        obj.SetActive(false);
        poolList.Add(obj);
        obj.transform.parent = fatherObj.transform;
    }
    public GameObject GetObj()
    {
        GameObject obj = null;
        obj = poolList[0];
        poolList.RemoveAt(0);
        obj.SetActive(true);
        obj.transform.parent = null;
        return obj;
    }
}


public class PoolManager : Singleton<PoolManager>
{
    public Dictionary<string, PoolData> poolDic = new Dictionary<string, PoolData>();
    private string poolDataPrefabPath = "PoolDataPrefab/";
    private GameObject poolObj;
    
    public void GetObj(string name, System.Action<GameObject> callBack)
    {
        //有该类对象父物体，且父物体下面有对象
        if (poolDic.ContainsKey(name) && poolDic[name].poolList.Count > 0)
        {
            callBack(poolDic[name].GetObj());
        }
        //1、有父物体，父物体下没对象 2、无父物体 两种情况都是加载资源后推入对象池
        else
        {
            //通过异步加载资源 创建对象给外部用
            ResManager.Instance.LoadGameObjectAsync(poolDataPrefabPath+name,poolDic[name].fatherObj, 
                (obj) =>
            {
                obj.name = name;
                callBack(obj);
                PushObj(name,obj);
            });

            //obj = GameObject.Instantiate(Resources.Load<GameObject>(name));
            //把对象名字改的和池子名字一样
            //obj.name = name;
        }
    }

    public void PushObj(string name, GameObject obj)
    {
        if (poolObj == null)
            poolObj = new GameObject("Pool");
        if (poolDic.ContainsKey(name))
        {
            poolDic[name].PushObj(obj);
        }
        else
        {
            poolDic.Add(name, new PoolData(obj, poolObj));
        }
    }
    public void Clear()
    {
        poolDic.Clear();
        poolObj = null;
    }
}
