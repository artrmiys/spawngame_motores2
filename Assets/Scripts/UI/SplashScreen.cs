using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    [SerializeField] float displayTime = 3f;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(displayTime);
        SceneManager.LoadScene("MainMenu");
    }
}
