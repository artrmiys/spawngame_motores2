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
    [SerializeField] TextMeshProUGUI messageText;

    [Header("Screens")]
    [SerializeField] GameObject winScreen;
    [SerializeField] GameObject loseScreen;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
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
        yield return new WaitForSeconds(delay);
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
}
