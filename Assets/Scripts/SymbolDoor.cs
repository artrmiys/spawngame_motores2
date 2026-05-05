using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Symbol Door puzzle manager. Orchestrates model, view, and controller.
/// Handles puzzle display, state management, and cleanup.
/// </summary>
public class SymbolDoor : MonoBehaviour
{
    public static SymbolDoor Instance { get; private set; }

    [SerializeField] private SymbolDoorView viewPrefab;
    [SerializeField] private PuzzleConfig defaultConfig;
    [SerializeField] private float doorOpenDelay = 1f;

    private ISymbolDoorModel model;
    private SymbolDoorView view;
    private SymbolDoorController controller;
    private bool isOpen = false;
    private Coroutine openRoutine;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void OnEnable()
    {
        SymbolDoorEventManager.OnDoorOpened += HandleDoorOpened;
        SymbolDoorEventManager.OnDoorFailed += HandleDoorFailed;
    }

    void OnDisable()
    {
        SymbolDoorEventManager.OnDoorOpened -= HandleDoorOpened;
        SymbolDoorEventManager.OnDoorFailed -= HandleDoorFailed;
        StopOpenRoutine();
    }

    void OnDestroy()
    {
        CleanupPuzzle();
    }

    public void ShowPuzzle(PuzzleConfig config = null)
    {
        if (isOpen) return;

        config ??= defaultConfig;
        if (config == null)
        {
            var puzzle = SymbolDoorPuzzleBuilder.CreateDefault().Build();
            ShowPuzzle(puzzle.Symbols, puzzle.Meanings, puzzle.CorrectPairs);
            return;
        }

        InitializePuzzle(config);
        gameObject.SetActive(true);
    }

    public void ShowPuzzle(List<string> symbols, List<string> meanings, Dictionary<string, string> correctPairs)
    {
        if (isOpen) return;

        if (symbols == null || meanings == null || correctPairs == null || symbols.Count == 0)
        {
            Debug.LogError("[SymbolDoor] Invalid puzzle data!");
            return;
        }

        model = new SymbolDoorModel(symbols, meanings, correctPairs);
        SetupController();
        gameObject.SetActive(true);
    }

    public void HidePuzzle()
    {
        isOpen = false;
        CleanupPuzzle();
        gameObject.SetActive(false);
    }

    private void InitializePuzzle(PuzzleConfig config)
    {
        var symbols = config.GetSymbols();
        var meanings = config.GetMeanings();
        var pairs = config.GetPairs();

        if (symbols.Count == 0)
        {
            Debug.LogError("[SymbolDoor] Empty puzzle config!");
            return;
        }

        model = new SymbolDoorModel(symbols, meanings, pairs);
        SetupController();
    }

    private void SetupController()
    {
        if (model == null)
        {
            Debug.LogError("[SymbolDoor] Model not initialized!");
            return;
        }

        if (controller != null)
            Destroy(controller);

        controller = gameObject.AddComponent<SymbolDoorController>();

        if (view == null && viewPrefab != null)
            view = Instantiate(viewPrefab, transform);

        if (view == null)
            view = CreateRuntimeView();

        if (view == null)
        {
            Debug.LogError("[SymbolDoor] View prefab not set!");
            return;
        }

        view.Setup(model.GetSymbols(), model.GetMeanings(), controller);
        controller.Initialize(model, view);
    }

    private void CleanupPuzzle()
    {
        if (view != null)
        {
            Destroy(view.gameObject);
            view = null;
        }

        if (controller != null)
        {
            Destroy(controller);
            controller = null;
        }

        model = null;
        StopOpenRoutine();
    }

    private void HandleDoorOpened()
    {
        isOpen = true;
        openRoutine = StartCoroutine(DoorOpenRoutine());
    }

    private void HandleDoorFailed()
    {
        controller?.OnResetClicked();
    }

    private IEnumerator DoorOpenRoutine()
    {
        yield return new WaitForSecondsRealtime(doorOpenDelay);
        openRoutine = null;
        HidePuzzle();
    }

    private void StopOpenRoutine()
    {
        if (openRoutine != null)
        {
            StopCoroutine(openRoutine);
            openRoutine = null;
        }
    }

    private SymbolDoorView CreateRuntimeView()
    {
        var viewObject = new GameObject("SymbolDoorRuntimeView");
        viewObject.transform.SetParent(transform, false);
        return viewObject.AddComponent<SymbolDoorView>();
    }
}

public readonly struct SymbolDoorPuzzleData
{
    public SymbolDoorPuzzleData(
        List<string> symbols,
        List<string> meanings,
        Dictionary<string, string> correctPairs)
    {
        Symbols = symbols;
        Meanings = meanings;
        CorrectPairs = correctPairs;
    }

    public List<string> Symbols { get; }
    public List<string> Meanings { get; }
    public Dictionary<string, string> CorrectPairs { get; }
}

public class SymbolDoorPuzzleBuilder
{
    readonly List<string> symbols = new();
    readonly List<string> meanings = new();
    readonly Dictionary<string, string> correctPairs = new();

    public static SymbolDoorPuzzleBuilder CreateDefault()
    {
        return new SymbolDoorPuzzleBuilder()
            .AddPair("\u2600", "Day")
            .AddPair("\u263E", "Night")
            .AddPair("\u2192", "Go");
    }

    public SymbolDoorPuzzleBuilder AddPair(string symbol, string meaning)
    {
        if (string.IsNullOrEmpty(symbol) || string.IsNullOrEmpty(meaning))
            return this;

        symbols.Add(symbol);
        meanings.Add(meaning);
        correctPairs[symbol] = meaning;
        return this;
    }

    public SymbolDoorPuzzleData Build()
    {
        return new SymbolDoorPuzzleData(
            new List<string>(symbols),
            new List<string>(meanings),
            new Dictionary<string, string>(correctPairs));
    }
}
