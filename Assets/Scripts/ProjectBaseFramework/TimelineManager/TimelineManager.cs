using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineManager : MonoSingleton<TimelineManager>
{
    private PlayableDirector _director;
    protected override void OnAwake()
    {
        base.OnAwake();
        if (_director == null)
        {
            _director = gameObject.AddComponent<PlayableDirector>();
        }
    }

    public void PlayTimeline(string path)
    {
        ResManager.Instance.LoadAssetAsync<PlayableAsset>("Timeline/"+path, (playableAsset) =>
        {
            _director.playableAsset = playableAsset;
            _director.Play();
        });
    }
}
