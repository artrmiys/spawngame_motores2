using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void PlayGame()
    {
        AutoAttack.ResetPersistedWeaponLevel();
        SceneManager.LoadScene("Level1");
    }
}
