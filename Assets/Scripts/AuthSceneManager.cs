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
    private static bool IsInit = false;

    private void Start()
    {
        Application.targetFrameRate = 60;
        versionTxt.text = "v." + Application.version;
        if (!IsInit) Invoke(nameof(LoadingDelay), initializingDelay);
        else Invoke(nameof(LoadingDelay), 0.2f);

        loadingPanel.SetActive(true);
        authPanel.SetActive(false);
        authLoadingPanel.SetActive(false);
        welcomePanel.SetActive(false);
    }

    private void LoadingDelay()
    {
        loadingPanel.SetActive(false);
        authPanel.SetActive(true);
        IsInit = true;
    }

    public void AuthViaVK()
    {
        authPanel.SetActive(false);
        authLoadingPanel.SetActive(true);
        VK_Api.Instance.AuthRequest(AuthDone);
    }

    public void GuestMode()
    {
        VK_Api.Username = "USER";
        VK_Api.UserID = string.Empty;
        AuthSuccess();
    }

    public void AuthDone(bool result)
    {
        if (!result) CancelAuth();
        else AuthSuccess();
    }

    private void AuthSuccess()
    {
        authPanel.SetActive(false);
        authLoadingPanel.SetActive(false);
        welcomePanel.SetActive(true);
        if (VK_Api.Username != "USER") welcomeTxt.ReplaceText("{USERNAME}", VK_Api.Username, VK_Api.Username);
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
