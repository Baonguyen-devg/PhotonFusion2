using System;
using UnityEngine;
using DG.Tweening; 

public class GUIAnimation : MonoBehaviour
{
    [Serializable]
    public enum GUIAnimationType 
    {
        None = 0,
        ScaleUp = 1,
        ScaleDown = 2,
        FadeIn = 3,
        FadeOut = 4,
        BounceIn = 5,
        BounceOut = 6,
    }

    [Serializable]
    public enum GUIAnimPlayType
    {
        Once = 0,
        Several = 1,
        Loop = 2,
    }

    [SerializeField] private GUIAnimationType _animType = GUIAnimationType.None;
    [SerializeField] private GUIAnimPlayType _animPlayType = GUIAnimPlayType.Once;

    [SerializeField] private float _duration = 1.0f; 
    [SerializeField] private int _loopCount = 3;
    [SerializeField] private Ease _easeType = Ease.Linear; 

    private Tweener _tweener;

    public void PlayGUIAnimation() 
    {
        if (_tweener != null && _tweener.IsActive())
        {
            _tweener.Kill();
        }

        switch (_animType)
        {
            case GUIAnimationType.ScaleUp:
                _tweener = transform.DOScale(Vector3.one, _duration).From(Vector3.zero).SetEase(_easeType);
                break;

            case GUIAnimationType.ScaleDown:
                _tweener = transform.DOScale(Vector3.zero, _duration).SetEase(_easeType);
                break;

            case GUIAnimationType.FadeIn:
                if (TryGetComponent<CanvasGroup>(out var canvasGroupIn))
                {
                    canvasGroupIn.alpha = 0;
                    _tweener = canvasGroupIn.DOFade(1, _duration).SetEase(_easeType);
                }
                break;

            case GUIAnimationType.FadeOut:
                if (TryGetComponent<CanvasGroup>(out var canvasGroupOut))
                {
                    canvasGroupOut.alpha = 1;
                    _tweener = canvasGroupOut.DOFade(0, _duration).SetEase(_easeType);
                }
                break;

            case GUIAnimationType.BounceIn:
                _tweener = transform.DOScale(Vector3.one, _duration).From(Vector3.zero).SetEase(Ease.OutBounce);
                break;

            case GUIAnimationType.BounceOut:
                _tweener = transform.DOScale(Vector3.zero, _duration).SetEase(Ease.InBounce);
                break;

            default:
                Debug.LogWarning("No animation type specified.");
                break;
        }

        if (_animPlayType == GUIAnimPlayType.Loop)
        {
            _tweener.SetLoops(-1, LoopType.Restart);
        }
        else if (_animPlayType == GUIAnimPlayType.Several)
        {
            _tweener.SetLoops(_loopCount, LoopType.Restart);
        }
        _tweener.Play();
    }

    public void EndGUIAnimation() 
    {
        if (_tweener != null && _tweener.IsActive())
        {
            _tweener.Kill();
            _tweener = null;
        }

        switch (_animType)
        {
            case GUIAnimationType.ScaleUp:
            case GUIAnimationType.BounceIn:
                transform.localScale = Vector3.one;
                break;

            case GUIAnimationType.ScaleDown:
            case GUIAnimationType.BounceOut:
                transform.localScale = Vector3.zero;
                break;

            case GUIAnimationType.FadeIn:
                if (TryGetComponent<CanvasGroup>(out var canvasGroupIn))
                {
                    canvasGroupIn.alpha = 1;
                }
                break;

            case GUIAnimationType.FadeOut:
                if (TryGetComponent<CanvasGroup>(out var canvasGroupOut))
                {
                    canvasGroupOut.alpha = 0;
                }
                break;

            default:
                Debug.LogWarning("No reset logic for the specified animation type.");
                break;
        }
    }
}
