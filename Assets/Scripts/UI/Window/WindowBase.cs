
using DG.Tweening;
using UnityEngine;

public class WindowBase:MonoBehaviour
{
    public string windowName;
    public bool Visible;
    protected CanvasGroup _canvasGroup;

    protected virtual void Awake()
    {
        windowName = gameObject.name;
        Visible = true;
        _canvasGroup = gameObject.GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
        {
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public virtual void SetVisible(bool isVisible)
    {
        _canvasGroup.alpha = isVisible ? 1 : 0;
        _canvasGroup.blocksRaycasts = isVisible;
        Visible = isVisible;
    }
    
    
    protected virtual void OnDestroy()
    {
        name = null;
    }
}
