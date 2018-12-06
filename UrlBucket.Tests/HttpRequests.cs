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

    }
}
