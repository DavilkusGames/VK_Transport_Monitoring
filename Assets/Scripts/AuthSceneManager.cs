using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AuthSceneManager : MonoBehaviour
{
    public GameObject loadingPanel;
    public GameObject authPanel;
    public GameObject authLoadingPanel;
    public GameObject welcomePanel;

    public TMP_Text versionTxt;
    public TextTranslator welcomeTxt;
    public float initializingDelay = 2f;
    public float loadingDelay = 2f;

    public Sprite[] langIcons;

    private string username = "{USERNAME}";

    private void Start()
    {
        Application.targetFrameRate = 60;
        versionTxt.text = "v." + Application.version;
        Invoke(nameof(LoadingDelay), initializingDelay);

        loadingPanel.SetActive(true);
        authPanel.SetActive(false);
        authLoadingPanel.SetActive(false);
        welcomePanel.SetActive(false);
    }

    private void LoadingDelay()
    {
        loadingPanel.SetActive(false);
        authPanel.SetActive(true);
    }

    public void AuthViaVK()
    {
        authPanel.SetActive(true);
        authLoadingPanel.SetActive(true);
        VK_Api.Instance.AuthRequest(AuthDone);
    }

    public void GuestMode()
    {
        username = "USER";
        AuthSuccess();
    }

    public void AuthDone(bool result)
    {
        if (!result) CancelAuth();
        else
        {
            AuthSuccess();
        }
    }

    private void AuthSuccess()
    {
        authPanel.SetActive(false);
        authLoadingPanel.SetActive(false);
        welcomePanel.SetActive(true);
        if (username != "USER") welcomeTxt.ReplaceText("{USERNAME}", username, username);
        else welcomeTxt.ReplaceText("{USERNAME}", "Пользователь", "User");
        Invoke(nameof(LoadMainScene), loadingDelay);
    }

    public void CancelAuth()
    {
        authPanel.SetActive(true);
        authLoadingPanel.SetActive(false);
        VK_Api.Instance.CancelAuth();
    }

    public void SwitchLang(Button btn)
    {
        LanguageManager.Instance.ChangeLang(!LanguageManager.IsRus);
        btn.image.sprite = langIcons[LanguageManager.IsRus ? 0 : 1];
    }

    private void LoadMainScene()
    {
        SceneManager.LoadScene(1);
    }
}
