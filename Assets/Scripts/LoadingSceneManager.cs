using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingSceneManager : MonoBehaviour
{
    public float loadingDelay = 2f;

    private void Start()
    {
        Application.targetFrameRate = 60;
        Invoke(nameof(LoadMainScene), loadingDelay);
    }

    private void LoadMainScene()
    {
        SceneManager.LoadScene(1);
    }
}
