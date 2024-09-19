using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PostCntrl : MonoBehaviour
{
    public TMP_Text authorLabel;
    public TMP_Text txtLabel;

    private string author = string.Empty;
    private string text = string.Empty;
    private string link = string.Empty;

    public void Initialize(string author, string text, string link, string keyword)
    {
        text = text.Replace(keyword, "<color=\"red\">" + keyword + "</color>");

        this.author = author;
        this.text = text;
        this.link = link;

        authorLabel.text = author;
        txtLabel.text = text; 
    }

    public void GoToPost()
    {
        Application.OpenURL(link);
    }
}
