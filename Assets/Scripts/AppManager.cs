using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
public class Post {
    public string author;
    public string txt;
    public string link;
}


public class AppManager : MonoBehaviour
{
    public ProfilePicture pfp;
    public TextTranslator usernameLabel;
    public TextTranslator userIdLabel;
    public TextTranslator sessionTimeLabel;

    public TMP_InputField requestInput;
    public TMP_Text versionTxt;

    public GameObject postPrefab;
    public Transform postsParent;

    public List<Post> posts = new List<Post>();
    private List<GameObject> postsObjs = new List<GameObject>();

    private void Start()
    {
        Application.targetFrameRate = 60;
        versionTxt.text = "v." + Application.version;

        if (string.IsNullOrEmpty(VK_Api.Username) || VK_Api.Username == "USER") usernameLabel.ReplaceText("{USERNAME}", "Пользователь", "User");
        else usernameLabel.ReplaceText("{USERNAME}", VK_Api.Username, VK_Api.Username);

        if (string.IsNullOrEmpty(VK_Api.UserID)) userIdLabel.ReplaceText("{ID}", "None", "None");
        else userIdLabel.ReplaceText("{ID}", VK_Api.UserID, VK_Api.UserID);

        if (!string.IsNullOrEmpty(VK_Api.UserID))
        {
            string sessionTime = DateTime.Now.AddHours(1).ToString("g");
            sessionTimeLabel.ReplaceText("{SESSION_TIME}", sessionTime, sessionTime);
        }
        else sessionTimeLabel.gameObject.SetActive(false);

        if (!string.IsNullOrEmpty(VK_Api.PfpLink))
        {
            pfp.Load(VK_Api.PfpLink);
        }
    }

    private void UpdatePosts(string request)
    {
        string[] keywords = request.Split(',');

        for (int i = 0; i < postsObjs.Count; i++)
        {
            Destroy(postsObjs[i].gameObject);
        }
        postsObjs.Clear();

        for (int i = posts.Count-1; i >= 0; i--)
        {
            bool publishPost = false;
            string keyword = string.Empty;

            for (int j = 0; j < keywords.Length; j++) {
                if (posts[i].txt.ToLower().Contains(keywords[j]))
                {
                    keyword = keywords[j];
                    publishPost = true;
                    break;
                }
            }
            if (publishPost)
            {
                GameObject postObj = Instantiate(postPrefab);
                postObj.transform.SetParent(postsParent);
                postObj.transform.localScale = Vector3.one;
                postObj.GetComponent<PostCntrl>().Initialize(posts[i].author, posts[i].txt, posts[i].link, keyword);
                postsObjs.Add(postObj);
            }
        }
    }

    public void Request()
    {
        string request = requestInput.text;
        request = request.Remove(request.Length - 1);
        UpdatePosts(request.Replace(" ", string.Empty).ToLower());
    }

    public void Quit()
    {
        SceneManager.LoadScene(0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(2);
        }
    }
}
