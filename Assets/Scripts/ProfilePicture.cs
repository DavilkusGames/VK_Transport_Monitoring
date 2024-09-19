using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ProfilePicture : MonoBehaviour
{
    private RawImage rawImg;

    private void Start()
    {
        rawImg = GetComponent<RawImage>();
    }

    public void Load(string path)
    {
        StartCoroutine(nameof(DownloadImage), path);
    }

    private IEnumerator DownloadImage(string path)
    {
        yield return new WaitForSeconds(3f);
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(path);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            rawImg.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        }
    }
}
