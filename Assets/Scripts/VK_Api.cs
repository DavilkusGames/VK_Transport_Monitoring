using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class VK_Api : MonoBehaviour
{
    public static VK_Api Instance { get; private set; }
    private static string accessToken = null;

    private HttpListener server;
    private string code = null;
    private string deviceId = null;
    private string codeVerifier = string.Empty;

    public delegate void AuthCallback(bool result);
    private AuthCallback authCallback = null;

    private Thread listenerThread = null;
    private Thread getAccessTokenThread = null;


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    private void OnDestroy() => Instance = null;

    public void AuthRequest(AuthCallback callback)
    {
        authCallback = callback;
        StartCoroutine(nameof(Auth));
    }

    public void CancelAuth()
    {
        if (server == null) return;

        if (listenerThread != null)
        {
            if (listenerThread.ThreadState == ThreadState.Running) listenerThread.Abort();
            listenerThread = null;
        }

        if (getAccessTokenThread != null)
        {
            if (getAccessTokenThread.ThreadState == ThreadState.Running) getAccessTokenThread.Abort();
            getAccessTokenThread = null;
        }

        if (server.IsListening) server.Stop();
        server = null;

        StopCoroutine(nameof(Auth));
    }

    private IEnumerator Auth()
    {
        codeVerifier = GenerateRandomString(60);

        string clientId = "52324294";
        string clientSecret = "Tnz6O3S0CH3MUmLqGhwU";
        string redirectUri = "https://1pixelgames.ru/";
        string codeChallenge = GenerateCodeChallenge(codeVerifier);
        string codeChallengeMethod = "s256";
        string state = "state";
        string scopes = "vkid.personal_info";

        server = new HttpListener();
        server.Prefixes.Add("http://localhost:5444/");
        server.Start();

        listenerThread = new Thread(StartListenerThread);
        listenerThread.Start();

        yield return new WaitForSeconds(1f);
        string authUrl = $"https://id.vk.com/authorize?client_id={clientId}&redirect_uri={redirectUri}&code_challenge={codeChallenge}&code_challenge_method={codeChallengeMethod}&state={state}&display=page&scope={scopes}&response_type=code";
        Debug.Log("Opening browser for authorization...");

        Application.OpenURL(authUrl);

        while (code == null) yield return null;
        Debug.Log("Code received. Getting access token...");
        getAccessTokenThread = new Thread(() => GetAccessTokenThread(clientId, clientSecret, code, deviceId, redirectUri));
        while (accessToken == null) yield return null;
        Debug.Log("Access Token: " + accessToken);
        authCallback(true);
    }

    private async void ListenerCallback(IAsyncResult result)
    {
        var context = server.EndGetContext(result);

        context.Response.AddHeader("Access-Control-Allow-Origin", "*");
        context.Response.AddHeader("Access-Control-Allow-Methods", "POST, GET, OPTIONS");
        context.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type");

        if (context.Request.HttpMethod == "OPTIONS")
        {
            context.Response.StatusCode = 200;
            context.Response.Close();
            return;
        }

        using (var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
        {
            string message = await reader.ReadToEndAsync();
            string[] data = message.Split('&');
            code = data[0];
            deviceId = data[1];
        }

        context.Response.Close();
    }

    private void StartListenerThread()
    {
        while (true)
        {
            var result = server.BeginGetContext(ListenerCallback, server);
            result.AsyncWaitHandle.WaitOne();
        }
    }

    private string GenerateCodeChallenge(string codeVerifier)
    {
        using (var sha256 = SHA256.Create())
        {
            var bytes = Encoding.ASCII.GetBytes(codeVerifier);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }
    }

    private void GetAccessTokenThread(string clientId, string clientSecret, string code, string deviceId, string redirectUri)
    {
        accessToken = GetAccessToken(clientId, clientSecret, code, deviceId, redirectUri).Result;
    }

    private async Task<string> GetAccessToken(string clientId, string clientSecret, string code, string deviceId, string redirectUri)
    {
        using (var httpClient = new HttpClient())
        {
            var values = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "code_verifier", codeVerifier },
                { "redirect_uri", redirectUri },
                { "code", code },
                { "client_id", clientId },
                { "device_id", deviceId },
                { "state", "state" }
            };

            var content = new FormUrlEncodedContent(values);

            var response = await httpClient.PostAsync("https://id.vk.com/oauth2/auth", content);
            var responseString = await response.Content.ReadAsStringAsync();

            return responseString;
        }
    }

    private static string GenerateRandomString(int length)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        StringBuilder result = new StringBuilder(length);
        System.Random random = new System.Random();

        for (int i = 0; i < length; i++)
        {
            result.Append(chars[random.Next(chars.Length)]);
        }

        return result.ToString();
    }
}
