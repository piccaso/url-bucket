using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using UrlBucket.Lib.Helper;

namespace UrlBucket {
    public class Config : IConfig {
        private readonly IConfiguration _configuration;
        public Config(IConfiguration configuration) {
            _configuration = configuration;
        }
        
        public string GetValue(string key, string defaultValue = null) {
            var value = _configuration.GetValue<string>(key);
            if (string.IsNullOrEmpty(value)) value = defaultValue;
            return value;
        }
    }
}
