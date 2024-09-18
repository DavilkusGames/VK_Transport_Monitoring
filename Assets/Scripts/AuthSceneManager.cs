using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AuthSceneManager : MonoBehaviour
{
    public GameObject loadingPanel;
    public GameObject authPanel;

    public TMP_Text versionTxt;
    public float loadingDelay = 2f;

    private void Start()
    {
        Application.targetFrameRate = 60;
        versionTxt.text = "v." + Application.version;
        Invoke(nameof(LoadingDelay), loadingDelay);
    }

    private void LoadingDelay()
    {
        loadingPanel.SetActive(false);
        authPanel.SetActive(true);
    }

    public void AuthViaVK()
    {
        SceneManager.LoadScene(1);
    }
}
