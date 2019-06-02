using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace HtcSharp.Core.Models.Http.Utils {
    public class HtcResponseCookies {
        private readonly IResponseCookies _cookies;

        public HtcResponseCookies(IResponseCookies cookies) {
            _cookies = cookies;
        }

        public void Append(string key, string value, int expire = -1, string path = null, string domain = null, bool httpOnly = false, bool isEssential = false, bool secure = false) {
            var options = new CookieOptions() {
                Domain = domain,
                HttpOnly = httpOnly,
                IsEssential = isEssential,
                Path = path,
                Secure = secure, 
            };
            if(expire != -1) {
                options.MaxAge = TimeSpan.FromSeconds(DateTime.UtcNow.AddSeconds(expire).Second);
            }
            _cookies.Append(key, value, options);
        }

        public void Append(string key, string value, CookieOptions options) {
            _cookies.Append(key, value, options);
        }

        public void Append(string key, string value) {
            _cookies.Append(key, value);
        }

        public void Delete(string key) {
            _cookies.Delete(key);
        }
    }
}