using System;
using UnityEngine;

/// <summary>
/// Static event manager for Symbol Door puzzle events.
/// Safely broadcasts puzzle completion, failure, and reset states.
/// </summary>
public class SymbolDoorEventManager : MonoBehaviour
{
    public static event Action<bool> OnAnswerChecked;
    public static event Action OnDoorOpened;
    public static event Action OnDoorFailed;
    public static event Action OnPuzzleReset;

    private static SymbolDoorEventManager instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public static void RaiseOpened()
    {
        OnDoorOpened?.Invoke();
    }

    public static void RaiseAnswerChecked(bool isCorrect)
    {
        OnAnswerChecked?.Invoke(isCorrect);
    }

    public static void RaiseFailed()
    {
        OnDoorFailed?.Invoke();
    }

    public static void RaiseReset()
    {
        OnPuzzleReset?.Invoke();
    }

    public static void ClearAllListeners()
    {
        OnAnswerChecked = null;
        OnDoorOpened = null;
        OnDoorFailed = null;
        OnPuzzleReset = null;
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            ClearAllListeners();
            instance = null;
        }
    }
}
