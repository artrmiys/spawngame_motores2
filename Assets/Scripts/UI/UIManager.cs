using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("HUD")]
    [SerializeField] Slider    hpBar;
    [SerializeField] TextMeshProUGUI waveText;
    [SerializeField] TextMeshProUGUI weaponText;
    [SerializeField] TextMeshProUGUI messageText;

    [Header("Screens")]
    [SerializeField] GameObject winScreen;
    [SerializeField] GameObject loseScreen;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        EnsureCanvasScaler();
        EnsureWeaponText();
        if (messageText) messageText.gameObject.SetActive(false);
        if (winScreen)   winScreen.SetActive(false);
        if (loseScreen)  loseScreen.SetActive(false);
    }

    void Start()
    {
        var playerHealth = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerHealth>();
        if (playerHealth == null)
            return;

        playerHealth.onHealthChanged.AddListener(UpdateHP);
        UpdateHP(playerHealth.CurrentHP, playerHealth.MaxHP);

        var autoAttack = playerHealth.GetComponent<AutoAttack>();
        if (autoAttack != null)
        {
            autoAttack.onWeaponLevelChanged.AddListener(UpdateWeaponLevel);
            UpdateWeaponLevel(autoAttack.WeaponLevel);
        }
    }

    public void UpdateHP(int current, int max)
    {
        if (hpBar == null) return;
        hpBar.maxValue = max;
        hpBar.value    = current;
    }

    public void SetWave(int current, int total)
    {
        if (waveText) waveText.text = $"Wave {current} / {total}";
    }

    public void UpdateWeaponLevel(int level)
    {
        if (weaponText == null) return;

        weaponText.text = level switch
        {
            0 => "Weapon: Basic",
            1 => "Weapon: Rapid",
            2 => "Weapon: Spread",
            _ => "Weapon: Blast"
        };
    }

    public void ShowMessage(string msg, float duration = 2f)
    {
        if (messageText == null) return;
        StopCoroutine(nameof(HideMessageRoutine));
        messageText.text = msg;
        messageText.gameObject.SetActive(true);
        StartCoroutine(HideMessageRoutine(duration));
    }

    IEnumerator HideMessageRoutine(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        if (messageText) messageText.gameObject.SetActive(false);
    }

    public void ShowWin()
    {
        if (winScreen) winScreen.SetActive(true);
    }

    public void ShowLose()
    {
        if (loseScreen) loseScreen.SetActive(true);
    }

    void EnsureCanvasScaler()
    {
        var canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
            return;

        var scaler = canvas.GetComponent<CanvasScaler>();
        if (scaler == null)
            scaler = canvas.gameObject.AddComponent<CanvasScaler>();

        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
    }

    void EnsureWeaponText()
    {
        if (weaponText != null)
            return;

        var parent = transform as RectTransform;
        if (parent == null)
            return;

        var go = new GameObject("WeaponText");
        go.transform.SetParent(transform, false);

        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(1, 1);
        rect.anchorMax = new Vector2(1, 1);
        rect.pivot = new Vector2(1, 1);
        rect.anchoredPosition = new Vector2(-28, -32);
        rect.sizeDelta = new Vector2(320, 44);

        weaponText = go.AddComponent<TextMeshProUGUI>();
        weaponText.text = "Weapon: Basic";
        weaponText.fontSize = 22;
        weaponText.color = new Color(1f, 0.9f, 0.45f);
        weaponText.alignment = TextAlignmentOptions.Right;
        weaponText.fontStyle = FontStyles.Bold;
        weaponText.outlineColor = Color.black;
        weaponText.outlineWidth = 0.2f;
    }
}
