using System.Collections.Generic;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    public static bool IsRus = true;
    public static LanguageManager Instance { get; private set; }

    private List<TextTranslator> translateQueue = new List<TextTranslator>();

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public void ChangeLang(bool isRus)
    {
        IsRus = isRus;
        UpdateTexts();
    }

    private void UpdateTexts()
    {
        foreach (var txt in translateQueue)
        {
            txt.UpdateText();
        }
    }

    public void AddToTranslateQueue(TextTranslator txt)
    {
        translateQueue.Add(txt);
    }

    public void RemoveFromTranslateQueue(TextTranslator txt)
    {
        translateQueue.Remove(txt);
    }
}
