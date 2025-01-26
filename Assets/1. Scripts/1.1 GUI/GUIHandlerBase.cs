using System;
using DG.Tweening;
using UnityEngine;

[Serializable]
public enum GUIHideAction 
{
    Disable = 0,
    Destroy = 1,   
}

[Serializable]
public enum GUIStatus 
{
    Invalid = 0,
    Ready = 1,
    Showing = 2,
    Showed = 3,
    Hiding = 4,
    Hidden = 5,
}

public class GUIHandlerBase : MonoBehaviour
{
    [Header("Requried")]
    [SerializeField] private GameObject _GUIPrefab;
    [SerializeField] private string _GUIName = "GUI_Default";
    [SerializeField] private bool _alwaysShow = false;

    public GameObject GUIPrefab => _GUIPrefab;
    public string GUIName => _GUIName;
    public bool AlwaysShow => _alwaysShow;

    public void SetGUIPrefab(GameObject GUIPrefab) => _GUIPrefab = GUIPrefab;
    public void SetGuiName(string GUIName) => _GUIName = GUIName;

    [SerializeField] private GUIStatus _GUIStatus = GUIStatus.Invalid;
    [SerializeField] private GUIHideAction _GUIHideAction = GUIHideAction.Disable;

    public GUIStatus GetGUIStatus() => _GUIStatus;
    public GUIHideAction GetGUIHideAction() => _GUIHideAction;

    public event Action OnBeginShowing = null;
    public event Action OnEndShowing = null;
    public event Action OnBeginHidding = null;
    public event Action OnEndHiding = null;

    private GUIBase _GUIBase;
    private GameObject _GUIObject;
    private GameObject _rootGUI = null;

    private Canvas _GUICanvas;
    private Camera _GUICamera;

    public void SetGUICanvas(Canvas GUICanvas) => _GUICanvas = GUICanvas;
    public void SetGUICamera(Camera GUICamera) => _GUICamera = GUICamera;

    public void OnEnable() => ResetGUIBase();
    private void ResetGUIBase()
    {
        if (_GUIBase != null)
        {
            _GUIBase.SetHandler(null);
            _GUIBase.SetHandler(this);

            OnBeginHidding = null;
            OnEndHiding = null;
            OnBeginShowing = null;
            OnEndShowing = null;

            OnBeginShowing += _GUIBase.OnBeginShowing;
            OnEndShowing += _GUIBase.OnEndShowing;
            OnBeginHidding += _GUIBase.OnBeginHidding;
            OnEndHiding += _GUIBase.OnEndHidding;
        }
    }

    public virtual bool Show(params object[] @parameter) 
    {
        if (_GUIStatus == GUIStatus.Invalid || _GUIBase == null) Init();
        switch (_GUIStatus) 
        {
            case GUIStatus.Showing: return true;
            case GUIStatus.Showed: return true;
            case GUIStatus.Ready:

            case GUIStatus.Hiding: 
                Debug.Log("[GUIBaseHandler] Show | GUIObject Hiding");
                _GUIStatus = GUIStatus.Showing;
                RunGUIAnimation(true, parameter);

                OnEndHiding?.Invoke();
                OnBeginShowing?.Invoke();
                return true;

            case GUIStatus.Hidden: 
                Debug.Log("[GUIBaseHandler] Show | GUIObject Hidden");
                _GUIStatus = GUIStatus.Showing;

                RunGUIAnimation(true, parameter);
                OnBeginShowing?.Invoke();
                return true;

            default: 
                Debug.Log("[GUIBaseHandler] Show |FAIL| Invalid status!");
                return false;
        }
    }

    public virtual void Hide(params object[] @parameter) 
    {
        if (_GUIBase == null) return;
        switch (_GUIStatus) 
        {
            case GUIStatus.Hiding: break;
            case GUIStatus.Hidden: break;
            case GUIStatus.Ready:

            case GUIStatus.Showing:
                Debug.Log("[GUIBaseHandler] Hide | GUIObject Showing");
                _GUIStatus = GUIStatus.Hiding;
                RunGUIAnimation(false, parameter);
                
                OnEndShowing?.Invoke();
                OnBeginHidding?.Invoke();
                break;

            case GUIStatus.Showed:
                Debug.Log("[GUIBaseHandler] Hide | GUIObject Showing");
                 _GUIStatus = GUIStatus.Hiding;
                
                OnBeginHidding?.Invoke();
                RunGUIAnimation(false, parameter);
                break;

            default: 
                Debug.Log("[GUIBaseHandler] Hide |FAIL| Invalid status!");
                break;
        }
    }

    public bool Init() 
    {
        if (_GUIStatus >= GUIStatus.Ready && _GUIBase != null) 
        {
            Debug.Log($"[GUIBaseHandler] Init | GUIbase {_GUIName} is inited");
            return true;
        }
        
        _GUIStatus = GUIStatus.Invalid;
        _GUIObject = CreateGUIObject();
        if (_GUIObject != null) 
        {
            Debug.Log($"[GUIBaseHandler] Init |SUCCESS|  GUIbase {_GUIName}");
            _GUIStatus = GUIStatus.Ready;
            return true;
        }
        return false;
    }

    public GameObject CreateGUIObject() 
    {
        if (_rootGUI == null) _rootGUI = GameObject.Find("GUIRoot");
        if (_rootGUI == null) _rootGUI = new GameObject("GUIRoot");

        Debug.Log($"[GUIBaseHandler] CreateGUIObject | Create a new GUI object {_GUIName}");
        GameObject GUIObject = Instantiate(_GUIPrefab, _rootGUI.transform);

        _GUIBase = GUIObject.GetComponent<GUIBase>();
        if (_GUIBase != null) ResetGUIBase();

        _GUICanvas = _GUIBase.GetComponentInChildren<Canvas>();
        _GUICanvas.renderMode = RenderMode.ScreenSpaceCamera;
        _GUICanvas.worldCamera = _GUICamera;
        return GUIObject;
    }

    protected void RunGUIAnimation(bool isShowing, params object[] @parameter) 
    {
        if (_GUIObject == null) return;

        if (isShowing)
        {
            _GUIObject.SetActive(true);
            _GUIBase.Show(@parameter);
            _GUIBase.PlayAnimations();
        }
        else 
        {
            _GUIBase.Hide(@parameter);
            _GUIBase.EndAnimations();

            switch (_GUIHideAction)
            {
                case GUIHideAction.Disable:
                    if (!_GUIBase.SelfHide)
                    {
                        _GUIBase.Hide(@parameter);
                    }

                    _GUIObject.SetActive(false);
                    _GUIStatus = GUIStatus.Hidden;
                    break;

                case GUIHideAction.Destroy:
                    if (!_GUIBase.SelfHide)
                    {
                        _GUIBase.Hide(@parameter);
                    }

                    _GUIObject.SetActive(false);
                    _GUIStatus = GUIStatus.Hidden;
                    DoDestroy();
                    break;
            }
        }
    }

    public bool IsShowed() => _GUIStatus == GUIStatus.Showed;
    public T GetGUI<T>() where T : GUIBase
    {
        if (_GUIBase != null) return (T)_GUIBase;
        return null;
    }

    public void DoDestroy()
    {
        if (_GUIObject == null) return;
        Destroy(_GUIObject);
        _GUIStatus = GUIStatus.Invalid;
    }
}
