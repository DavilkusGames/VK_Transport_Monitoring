using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class VK_Api : MonoBehaviour
{
    public static VK_Api Instance { get; private set; }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    private void OnDestroy() => Instance = null;

    public async Task Auth()
    {
        string clientId = "52324294";
        string clientSecret = "Tnz6O3S0CH3MUmLqGhwU";
        string redirectUri = "https://1pixelgames.ru/";
        string codeVerifier = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
        string codeChallenge = GenerateCodeChallenge(codeVerifier);
        string codeChallengeMethod = "s256";
        string state = "state";
        string scopes = "vkid.personal_info";

        var server = new HttpListener();
        server.Prefixes.Add("http://localhost:5444/");
        server.Start();

        string authUrl = $"https://id.vk.com/authorize?client_id={clientId}&redirect_uri={redirectUri}&code_challenge={codeChallenge}&code_challenge_method={codeChallengeMethod}&state={state}&display=page&scope={scopes}&response_type=code";
        Debug.Log("Открытие браузера для авторизации...");

        Application.OpenURL(authUrl);

        var context = await server.GetContextAsync();
        var query = context.Request.Url.Query;
        await Console.Out.WriteLineAsync(query);
        string message = "";
        string code = "";
        string deviceId = "";


        if (context.Request.IsWebSocketRequest)
        {
            var wsContext = await context.AcceptWebSocketAsync(null);
            var buffer = new byte[1024];
            var result = await wsContext.WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), System.Threading.CancellationToken.None);

            message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            string[] values = message.Split('&');
            code = values[0];
            deviceId = values[1];
        }

        if (message != null)
        {
            var accessToken = await GetAccessToken(clientId, clientSecret, code, deviceId, redirectUri);
            Console.WriteLine("Access Token: " + accessToken);
        }
        else
        {
            Console.WriteLine("Ошибка получения кода авторизации");
        }

        server.Close();
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

    private async Task<string> GetAccessToken(string clientId, string clientSecret, string code, string deviceId, string redirectUri)
    {
        using (var httpClient = new HttpClient())
        {
            var values = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "code_verifier", "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" },
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
}
