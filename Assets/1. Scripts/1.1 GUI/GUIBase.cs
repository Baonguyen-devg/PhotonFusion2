using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIBase : MonoBehaviour
{
    [SerializeField] private GUIHandlerBase _handler;
    [SerializeField] private List<GUIAnimation> _animations = new List<GUIAnimation>();
    
    public GUIHandlerBase Handler => _handler;
    public void SetHandler(GUIHandlerBase handler) => _handler = handler;

    protected bool _selfHide;
    public bool SelfHide => _selfHide;

    public void PlayAnimations() 
    {
        foreach (GUIAnimation animation in _animations)
            animation.PlayGUIAnimation();
    }

    public void EndAnimations() 
    {
        foreach (GUIAnimation animation in _animations)
            animation.EndGUIAnimation();
    }

    public virtual bool Show(params object[] @parameter)
    {
        if (_handler == null) return false;

        _selfHide = true;
        return _handler.Show(@parameter);
    }

    public virtual void Hide(params object[] @parameter)
    {
        if (_handler == null) return;

        _selfHide = false;
        _handler.Hide(@parameter);
    }

    public virtual void OnBeginShowing() {}
    public virtual void OnEndShowing() {}
    public virtual void OnBeginHidding() {}
    public virtual void OnEndHidding() {}
}
