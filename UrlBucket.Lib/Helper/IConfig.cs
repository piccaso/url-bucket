using System;
using System.Collections.Generic;
using System.Text;

namespace UrlBucket.Lib.Helper {
    public interface IConfig {
        string GetValue(string key, string defaultValue = null);
    }
}
