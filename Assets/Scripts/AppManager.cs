using TMPro;
using UnityEngine;

public class AppManager : MonoBehaviour
{
    public TMP_Text versionTxt;

    private void Start()
    {
        Application.targetFrameRate = 60;
        versionTxt.text = "v." + Application.version;
    }
}
