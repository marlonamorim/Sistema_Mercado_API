using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

namespace APIProdutos.Helpers
{
    public class IntegrationAPIRest
    {
        private readonly IConfiguration _configuration;
        public IntegrationAPIRest(IConfiguration configuration) =>
            _configuration = configuration;

        public HttpResponseMessage ExecutePostRestUrl(string functionName, JObject jObject, string username, string password) {

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            if (jObject == null) {
                jObject = new JObject();
            }

            string parameters = jObject.ToString();

            var request = WebRequest.Create(GetRestEndPoint(functionName));
            request.Headers.Add("Authorization", $"Basic {GetToken($"{username}:{password}")}");
            request.Method = HttpMethod.Post.Method;

            var bytes = Encoding.UTF8.GetBytes(parameters);

            request.ContentLength = bytes.Length;
            request.ContentType = "application/json";

            using (var requestStream = request.GetRequestStream()) {
                requestStream.Write(bytes, 0, bytes.Length);

                try {
                    using (var response = request.GetResponse()) {
                        return GetResponseMessage((HttpWebResponse)response);
                    }
                }
                catch (WebException we) {
                    return GetResponseMessage((HttpWebResponse)we.Response);
                }
            }
        }

        private Uri GetRestEndPoint(string functionName) {
            string mainPath = string.Empty;
            if (_configuration.GetSection("SiteMercado:UrlIntegracao") != null) {
                mainPath = _configuration.GetSection("SiteMercado:UrlIntegracao").Value;
            }

            return new Uri($"{mainPath}/{functionName}");
        }

        private string GetToken(string credentials) {

            String token = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(credentials));
            return token;
        }

        private HttpResponseMessage GetResponseMessage(HttpWebResponse response) {
            var responseMessage = new HttpResponseMessage(response.StatusCode);

            using (var stream = response.GetResponseStream()) {
                using (var reader = new StreamReader(stream)) {

                    var body = reader.ReadToEnd();

                    responseMessage.Content = new StringContent(body, Encoding.UTF8, "application/json");
                }
            }

            return responseMessage;
        }
    }
}
