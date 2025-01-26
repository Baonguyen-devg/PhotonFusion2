using System.Collections.Generic;
using UnityEngine;

public class GUIManager : Singleton<GUIManager>
{
    [SerializeField] private Camera _GUICamera;
    [SerializeField] private List<GUIHandlerBase> _listHandler = new List<GUIHandlerBase>();

    public Camera GUICamera => _GUICamera;
    public List<GUIHandlerBase> ListHandler => _listHandler;

    public T GetHandler<T>(int index) where T : GUIHandlerBase
    {
        return IsValidIndex(index) && _listHandler[index] is T handler ? handler : null;
    }

    public T GetGUI<T>(int index) where T : GUIBase
    {
        return IsValidIndex(index) ? _listHandler[index]?.GetGUI<T>() : null;
    }

    public GameObject ShowGUI(int index, params object[] parameters)
    {
        if (!IsValidIndex(index)) return null;
        var handler = _listHandler[index];

        handler.SetGUICamera(GUICamera);
        handler?.Show(parameters);
        
        return handler?.gameObject;
    }

    public void HideGUI(int index, params object[] parameters)
    {
        if (IsValidIndex(index))
        {
            _listHandler[index]?.Hide(parameters);
        }
    }

    public bool IsShowed(int index)
    {
        return IsValidIndex(index) && _listHandler[index]?.IsShowed() == true;
    }

    public bool IsGUI(int index, GUIBase guiBase)
    {
        return IsValidIndex(index) && _listHandler[index] == guiBase?.Handler;
    }

    public void HideAllGUI(List<string> ignores = null, List<string> forceDestroy = null)
    {
        foreach (var handler in _listHandler)
        {
            if (handler == null) continue;

            if (forceDestroy?.Contains(handler.GUIName) == true)
            {
                handler.DoDestroy();
            }
            else if (ignores == null || !ignores.Contains(handler.GUIName))
            {
                handler.Hide();
            }
        }
    }

    public void CloseAll()
    {
        foreach (var handler in _listHandler)
        {
            if (handler == null || handler.AlwaysShow) continue;

            var status = handler.GetGUIStatus();
            if (status == GUIStatus.Showed || status == GUIStatus.Showing)
            {
                handler.Hide();
            }
        }
    }

    private bool IsValidIndex(int index)
    {
        return index >= 0 && index < _listHandler.Count;
    }
}
