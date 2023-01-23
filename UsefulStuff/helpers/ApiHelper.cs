using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;


public class Response<T>
{
    public HttpStatusCode ResponseCode;
    public T ResponseObject;
}

public static class ApiHelper
{
    static string baseAddress = "https://localhost:7230/";
    static HttpClient client = new HttpClient();
    static string? token;
  
    public static async Task<HttpStatusCode> GetResponseCode(string apiRoute)
    {
        RenewClient();
        HttpResponseMessage responseMessage = await client.GetAsync(apiRoute);
        return responseMessage.StatusCode;
    }

    public static async Task<T> GetRequest<T>(string apiRoute)
    {
        RenewClient();       
        HttpResponseMessage responseMessage = await client.GetAsync(apiRoute);
        responseMessage.EnsureSuccessStatusCode();       

#pragma warning disable CS8603
        if (responseMessage.IsSuccessStatusCode)
        {        
           return await responseMessage.Content.ReadFromJsonAsync<T>();        
        }
        else
        {
            return default;
        }
    }

#pragma warning restore CS8603
    public static async Task<HttpResponseMessage> PostRequest<T>(string apiRoute, T obj)
    {
        RenewClient();       
        return await client.PostAsJsonAsync<T>(apiRoute, obj);      
    }

    public static async Task<HttpResponseMessage> DeleteRequest(string apiRoute)
    {
        RenewClient();
        return await client.SendAsync(new HttpRequestMessage(HttpMethod.Delete, apiRoute));
    }

    public static async Task<HttpResponseMessage> PutRequest<T>(string apiRoute, T obj)
    {
        RenewClient();      
        return await client.PutAsJsonAsync<T>(apiRoute, obj);
    }

    public static async Task<HttpResponseMessage> Put(string apiRoute)
    {
        RenewClient();
        return await client.PutAsync(apiRoute, null);
    }

    public static async Task<Response<string>> GetString(string apiRoute)
    {
        RenewClient();
        HttpResponseMessage res = await client.GetAsync(apiRoute);
        var resString = await res.Content.ReadAsStringAsync();
        Response<string> response = new Response<string>();
        response.ResponseCode = res.StatusCode;
        response.ResponseObject = resString.Trim();
        return response;
    }

    //public static void SetBaseUrlDebug()
    //{
    //    //android localhost ip
    //    //baseAddress = DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5209" : "https://localhost:7230/";
    //}

    /// <summary>
    /// default url = 192.168.100.6/
    /// </summary>
    /// <param name="url"></param>
    public static void SetUrl(string url)
    {
        baseAddress = url;
    }

    public static void SetAuthHeader(string Token)
    {             
        var res = Token.Replace('"', ' ');
        token = res.Trim();
    }

    static void RenewClient()
    {
        client = new HttpClient();
        client.BaseAddress = new Uri(baseAddress);
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        if (!string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}

