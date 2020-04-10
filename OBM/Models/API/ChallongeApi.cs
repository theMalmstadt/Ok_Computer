using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

/// <summary>
/// This is the source for this API handling class: https://github.com/WolfenCLI/Challonge-CSharp-API
/// </summary>

namespace OBM.Models.API
{
    public class ChallongeApi
    {
        private const string ChallongeApiUrl = "api.challonge.com/v1";
        private static readonly HttpClient client = new HttpClient();
        private ApiCredentials creds = new ApiCredentials();
        public enum Methods { GET, POST, DELETE}

        public bool checkCredentials(string username, string apiKey)
        {
            creds.Username = username;
            creds.ApiKey = apiKey;

            string encoded = Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(creds.Username + ":" + creds.ApiKey));
            //HTTP Basic Authentication
            if (!client.DefaultRequestHeaders.Contains("Authorization"))
            {
                client.DefaultRequestHeaders.Add("Authorization", "Basic " + encoded);
                return true;
            }
            return false;
        }

        public async Task<string> Fetch(ChallongeApi.Methods method, string path, Dictionary<string, string> parameters = null)
        {
            if (parameters == null)
            {
                parameters = new Dictionary<string, string>();
            }

            if (!parameters.ContainsKey("api_key"))
            {
                parameters.Add("api_key", creds.ApiKey);
            }

            FormUrlEncodedContent content = new FormUrlEncodedContent(parameters);

            // Full Path
            string fullpath = "https://" + ChallongeApiUrl + "/" + path;
            string query = "";

            HttpResponseMessage response;
            switch (method)
            {
                case Methods.GET:
                    foreach (KeyValuePair<string, string> entry in parameters)
                    {
                        query += HttpUtility.UrlEncode(entry.Key) + "=" + HttpUtility.UrlEncode(entry.Value) + "&";
                    }
                    response = await client.GetAsync(fullpath + "?" + query);
                    break;
                case Methods.POST:
                    response = await client.PostAsync(fullpath, content);
                    break;
                case Methods.DELETE:
                    foreach (KeyValuePair<string, string> entry in parameters)
                    {
                        query += HttpUtility.UrlEncode(entry.Key) + "=" + HttpUtility.UrlEncode(entry.Value) + "&";
                    }
                    response = await client.DeleteAsync(fullpath + "?" + query);
                    break;
                default:
                    response = null;
                    break;
            }

            if (response == null) return "";
            return await response.Content.ReadAsStringAsync();
        }
        /*
         * Returns the JObject (used like a Dictionary) of the response
         */
        public async Task<JObject> FetchAndParse(ChallongeApi.Methods method, string path, Dictionary<string, string> body)
        {
            string responseAsString = await Fetch(method, path, body);
            if (responseAsString.StartsWith("{"))
            {
                return new JObject(new JProperty("result", JObject.Parse(responseAsString)));
            }
            else
            {
                // The API either returns a {data...} which is a JSon object or a [data...] which is an array
                return new JObject(new JProperty("result", JArray.Parse(responseAsString)));
            }
        }
    }
}