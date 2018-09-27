using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace UrlBucket.Tests {
    public class HttpRequests {
        private readonly WebApplicationFactory<Startup> _factory;

        public HttpRequests() {
            _factory = new WebApplicationFactory<Startup>();
        }

        [Test]
        public async Task CanGenerateSwagger() {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/swagger/v1/swagger.json");
            var responseText = await response.Content.ReadAsStringAsync();
            TestContext.WriteLine(responseText);

            response.EnsureSuccessStatusCode();
        }

        // [Test] //TODO
        public async Task StoreFile() {
            var client = _factory.CreateClient();
            var model = new Dictionary<string, string> {
                {"url", "xxx://yyy"},
                {"contentType", "text/plain" },
                {"content", "Hello World" },
            };
            var json = JToken.FromObject(model).ToString();
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("api/store-file", content);         
            var responseText = await response.Content.ReadAsStringAsync();
            TestContext.WriteLine(responseText);

            response.EnsureSuccessStatusCode();
        }
    }
}
