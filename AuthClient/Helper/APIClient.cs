using System;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using RestSharp;
using Thinktecture.IdentityModel.Client;

namespace AuthClient.Helper
{
    public class User
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string GrantType { get; set; }
    }

    public sealed class ApiClient : IAPIClient
    {
        private OAuth2Client client;
        private readonly string url = ConfigurationManager.AppSettings["tokenURL"];

        public ApiClient()
        {
            //client = new RestClient(url); //.Options(request: new ContentType("application/x-www-form-urlencoded"));
        }

        //public string Validate(string token)
        //{
        //    var httpClient = new HttpClient();
        //    httpClient.SetBearerToken(token);
        //    var response = httpClient.GetStringAsync(new Uri(url + @"/api/validate"));
        //    while (!(response.IsCompleted))
        //    {
        //    }

        //    return response;
        //}

        public Task<TokenResponse> GetTokenNew(string userId, string password,string clientKey,string clientSecret)
        {
            client = new OAuth2Client(new Uri(url + @"/token"), clientKey, clientSecret);

            var response = client.RequestResourceOwnerPasswordAsync(userId, password);

            while (!(response.IsCompleted))
            {
            }
            return response;
        }

        public Task<TokenResponse> GetRefreshToken(string refreshToken, string clientKey, string clientSecret)
        {
            client = new OAuth2Client(new Uri(url + @"/token"), clientKey, clientSecret);

            var response = client.RequestRefreshTokenAsync(refreshToken);
            while (!(response.IsCompleted))
            {
            }
            return response;
        }

        public Task<string> GetIdentity(string token)
        {
            var httpClient = new HttpClient();
            httpClient.SetBearerToken(token);
            var response = httpClient.GetStringAsync(new Uri(url + @"/api/identity"));
            while (!(response.IsCompleted))
            {
            }

            return response;
        }
        //not required
        public object GetToken(string userId, string password)
        {
            var restClient = new RestClient();
            const string body = @"'Grant_Type':'password','UserName': 'test.test@mail.com','Password': 'test123'";
            var request = new RestRequest("/token", Method.POST) {RequestFormat = DataFormat.Json};
            //request.AddBody(new User{grant_type ="password", userid = userId, password = password});
            //request.AlwaysMultipartFormData = true;
            request.AddBody(body);
            //request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.JsonSerializer.ContentType = "application/x-www-form-urlencoded";

            var response = restClient.Execute(request);
            return response;


            //using (System.Net.Http.HttpClient client = new System.Net.Http.HttpClient())
            //{
            //    client.BaseAddress = new Uri(_url);
            //    client.DefaultRequestHeaders.Accept.Clear();
            //    //client.DefaultRequestHeaders.Accept.Add(
            //    //    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            //    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            //    string body = @"'grant_type':'password','username': 'test.test@mail.com','password': 'test123'";
            //    var usr=new User {Grant_Type = "password", UserName = "test.test@mail.com", Password = "test123"};

            //    var response = client.PostAsJsonAsync("/token",usr);
            //    while (response.IsCompleted == false)
            //    {
            //        var abc = "1";
            //    }
            //    var xyz = response.Result.StatusCode.ToString();
            //    return response;
            //}
        }
    }
}