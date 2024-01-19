using SharedKernel.Libraries;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharedKernel.Providers
{
    public class HttpClientProvider : IHttpClientProvider
    {
        public readonly HttpClient _client;
        public readonly IServiceProvider _provider;
        public HttpClient httpClient => _client;


        public HttpClientProvider(IServiceProvider provider)
        {
            _provider = provider;

            var clientHandler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
            };
            _client = HttpClientFactory.Create(clientHandler);

            var config = provider.GetRequiredService<IConfiguration>();
            var env = Utility.GetEnvironmentLower();
            var baseAddress = config.GetRequiredSection($"HttpClient:BaseAddress:{env}");
            if (baseAddress.Exists())
            {
                _client.BaseAddress = new Uri(baseAddress.Value);
                if (!_client.BaseAddress.ToString().EndsWith("/"))
                {
                    _client.BaseAddress = new Uri($"{_client.BaseAddress}/");
                }
            }
        }

        #region Send Async
        public Task<HttpResponseMessage> SendAsync(string requestUri, object content, HttpMethod method, string token = "")
        {
            var stringContent = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage()
            {
                Method = method,
                RequestUri = new Uri(requestUri),
                Content = stringContent
            };
            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return _client.SendAsync(request);
        }

        public Task<HttpResponseMessage> SendAsync(string requestUri, object content, HttpMethod method, CancellationToken cancellationToken, string token = "")
        {
            var stringContent = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage()
            {
                Method = method,
                RequestUri = new Uri(requestUri),
                Content = stringContent
            };
            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return _client.SendAsync(request, cancellationToken);
        }
        #endregion

        #region Get
        public Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            return _client.GetAsync(requestUri);
        }

        public Task<HttpResponseMessage> GetAsync(string requestUri, CancellationToken cancellationToken)
        {
            return _client.GetAsync(requestUri, cancellationToken);
        }

        public Task<HttpResponseMessage> GetAsync(Uri requestUri)
        {
            return _client.GetAsync(requestUri);
        }

        public Task<HttpResponseMessage> GetAsync(Uri requestUri, CancellationToken cancellationToken)
        {
            return _client.GetAsync(requestUri, cancellationToken);
        }
        #endregion

        #region Delete
        public Task<HttpResponseMessage> DeleteAsync(string requestUri)
        {
            return _client.DeleteAsync(requestUri);
        }

        public Task<HttpResponseMessage> DeleteAsync(string requestUri, CancellationToken cancellationToken)
        {
            return _client.DeleteAsync(requestUri, cancellationToken);
        }

        public Task<HttpResponseMessage> DeleteAsync(Uri requestUri)
        {
            return _client.DeleteAsync(requestUri);
        }

        public Task<HttpResponseMessage> DeleteAsync(Uri requestUri, CancellationToken cancellationToken)
        {
            return _client.DeleteAsync(requestUri, cancellationToken);
        }
        #endregion

        #region Post
        public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
        {
            return _client.PostAsync(requestUri, content);
        }

        public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content, CancellationToken cancellationToken)
        {
            return _client.PostAsync(requestUri, content, cancellationToken);
        }


        public Task<HttpResponseMessage> PostAsync(Uri requestUri, HttpContent content)
        {
            return _client.PostAsync(requestUri, content);
        }

        public Task<HttpResponseMessage> PostAsync(Uri requestUri, HttpContent content, CancellationToken cancellationToken)
        {
            return _client.PostAsync(requestUri, content, cancellationToken);
        }
        #endregion

        #region Put
        public Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content)
        {
            return _client.PostAsync(requestUri, content);
        }

        public Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content, CancellationToken cancellationToken)
        {
            return _client.PostAsync(requestUri, content, cancellationToken);
        }


        public Task<HttpResponseMessage> PutAsync(Uri requestUri, HttpContent content)
        {
            return _client.PostAsync(requestUri, content);
        }

        public Task<HttpResponseMessage> PutAsync(Uri requestUri, HttpContent content, CancellationToken cancellationToken)
        {
            return _client.PostAsync(requestUri, content, cancellationToken);
        }
        #endregion
    }
}
