using System.Text;
using TMPro;
using UnityEngine;

public class TextTranslator : MonoBehaviour
{
    public TMP_Text txt;
    public string engStr = string.Empty;

    private string rusStr = string.Empty;

    private void Awake()
    {
        rusStr = txt.text;
    }

    private void Start()
    {
        LanguageManager.Instance.AddToTranslateQueue(this);
    }

    public void ReplaceText(string replace, string rus, string eng)
    {
        rusStr = rusStr.Replace(replace, rus);
        engStr = engStr.Replace(replace, eng);
        UpdateText();
    }

    public void UpdateText()
    {
        txt.text = LanguageManager.IsRus ? rusStr : engStr;
    }
}
