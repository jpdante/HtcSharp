using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace HTCSharp.Core.Models.Http.Utils {
    public class HTCResponseCookies {
        private IResponseCookies Cookies;

        public HTCResponseCookies(IResponseCookies cookies) {
            Cookies = cookies;
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
            Cookies.Append(key, value, options);
        }

        public void Delete(string key) {
            Cookies.Delete(key);
        }
    }
}