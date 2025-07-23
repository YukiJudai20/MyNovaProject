using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T: MonoSingleton<T>
{
    protected static T m_instance = null;

    public static T Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = Object.FindObjectOfType<T>();
                if (m_instance == null)
                {
                    var obj = new GameObject(typeof(T).Name);
                    m_instance = obj.AddComponent<T>();
                    m_instance.OnAwake();
                    DontDestroyOnLoad(obj);
                }
            }

            return m_instance;
        }
    }

    protected virtual void OnAwake()
    {
        
    }

    public virtual void Dispose()
    {
        Destroy(m_instance.gameObject);
    }
}
