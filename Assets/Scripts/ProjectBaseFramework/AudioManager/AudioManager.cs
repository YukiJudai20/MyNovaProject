using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoSingleton<AudioManager>
{
    private AudioSource audioSource;
    protected override void OnAwake()
    {
        base.OnAwake();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    //播放音乐，如果不指定来源，则用Manager的播，如果有来源，则在来源身上播
    public void PlayAudioClip(string clipPath,AudioSource playSource=null,bool loop =false)
    {
        ResManager.Instance.LoadAssetAsync<AudioClip>("Sounds/"+clipPath, (loadedClip) =>
        {
            if (playSource != null)
            {
                playSource.clip = loadedClip;
                playSource.loop = loop;
                playSource.Play();
            }
            else
            {
                audioSource.clip = loadedClip;
                audioSource.loop = loop;
                audioSource.Play();
            }
        });
    }
    //播放音乐，如果不指定来源，则用Manager的播，如果有来源，则在来源身上播
    public void PlayAudioClip(AudioClip audioClip,AudioSource playSource=null,bool loop = false)
    {
        if (playSource != null)
        {
            playSource.clip = audioClip;
            playSource.loop = loop;
            playSource.Play();
        }
        else
        {
            audioSource.clip = audioClip;
            audioSource.loop = loop;
            audioSource.Play();
        }
    } 
}
