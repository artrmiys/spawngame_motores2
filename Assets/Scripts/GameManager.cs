using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] string nextSceneName;
    [SerializeField] float nextLevelDelay = 1.5f;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            player.GetComponent<PlayerHealth>()?.onDied.AddListener(Lose);
    }

    public void Win()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            UIManager.Instance?.ShowMessage("Level cleared!", nextLevelDelay);
            StartCoroutine(LoadNextLevelRoutine());
            return;
        }

        Time.timeScale = 0f;
        UIManager.Instance?.ShowWin();
    }

    public void Lose()
    {
        Time.timeScale = 0f;
        UIManager.Instance?.ShowLose();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        AutoAttack.ResetPersistedWeaponLevel();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        AutoAttack.ResetPersistedWeaponLevel();
        SceneManager.LoadScene("MainMenu");
    }

    IEnumerator LoadNextLevelRoutine()
    {
        yield return new WaitForSeconds(nextLevelDelay);
        Time.timeScale = 1f;
        SceneManager.LoadScene(nextSceneName);
    }
}
