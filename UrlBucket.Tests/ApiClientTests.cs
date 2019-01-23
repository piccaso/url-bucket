using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;
using UrlBucket.Client;

namespace UrlBucket.Tests {
    public class ApiClientTests {
        private readonly WebApplicationFactory<Startup> _factory;

        public ApiClientTests() {
            _factory = new WebApplicationFactory<Startup>();
        }

        private ApiClient GetApiClient() {
            var httpClient = _factory.CreateClient();
            return new ApiClient(httpClient.BaseAddress.ToString(), httpClient);
        }

        [Test]
        public async Task StoreAndRetrieve() {
            var client = GetApiClient();
            var bytes = Encoding.UTF8.GetBytes("Hello World");

            var storeResponse = await client.ApiStoreBytesPostAsync(bytes, "xxx://xxx", "text/plain");
            var retrievedFile = await client.ApiRetrieveGetAsync("xxx://xxx");

            Assert.AreEqual(StatusCodes.Status200OK, storeResponse.Status);
            Assert.AreEqual(bytes, retrievedFile.Content);

            var httpClient = _factory.CreateClient();
            var bytesResponse = await httpClient.GetByteArrayAsync($"api/retrieve-bytes?url={Uri.EscapeDataString("xxx://xxx")}");
            Assert.AreEqual(bytes, bytesResponse);
        }
    }
}
