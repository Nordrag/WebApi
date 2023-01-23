using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;

public class ApiHelperInstance
{
    string baseAddress = "https://localhost:7230/";
    HttpClient client = new HttpClient();
    string? token;

    public ApiHelperInstance(string Token)
    {
        var res = Token.Replace('"', ' ');
        token = res.Trim();
    }

    public ApiHelperInstance()
    {
    }

    public void SetToken(string Token)
    {
        var res = Token.Replace('"', ' ');
        token = res.Trim();
    }

    public async Task<HttpStatusCode> GetResponseCode(string apiRoute)
    {
        RenewClient();
        HttpResponseMessage responseMessage = await client.GetAsync(apiRoute);
        return responseMessage.StatusCode;
    }

    public async Task<T> GetRequest<T>(string apiRoute)
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
    public async Task<HttpResponseMessage> PostRequest<T>(string apiRoute, T obj)
    {
        RenewClient();              
        return await client.PostAsJsonAsync<T>(apiRoute, obj);
    }

    public async Task<HttpResponseMessage> DeleteRequest(string apiRoute)
    {
        RenewClient();
        return await client.SendAsync(new HttpRequestMessage(HttpMethod.Delete, apiRoute));
    }

    public async Task<HttpResponseMessage> PutRequest<T>(string apiRoute, T obj)
    {
        RenewClient();
        return await client.PutAsJsonAsync<T>(apiRoute, obj);
    }

    public async Task<HttpResponseMessage> Put(string apiRoute)
    {
        RenewClient();
        return await client.PutAsync(apiRoute, null);
    }

    public async Task<Response<string>> GetString(string apiRoute)
    {
        RenewClient();
        HttpResponseMessage res = await client.GetAsync(apiRoute);
        var resString = await res.Content.ReadAsStringAsync();
        Response<string> response = new Response<string>();
        response.ResponseCode = res.StatusCode;
        response.ResponseObject = resString.Trim();
        return response;
    }

    //public void SetBaseUrlDebug()
    //{
    //    baseAddress = DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5209" : "https://localhost:7230/";
    //}

    /// <summary>
    /// default url = 192.168.100.6/
    /// </summary>
    /// <param name="url"></param>
    public void SetUrl(string url)
    {
        baseAddress = url;
    }

    public void SetAuthHeader(string Token)
    {
        var res = Token.Replace('"', ' ');
        token = res.Trim();
    }

    void RenewClient()
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