using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using HtcSharp.Core.Logging.Abstractions;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Http.Abstractions.Extensions;
using HtcSharp.HttpModule.Http.Features;
using HtcSharp.HttpModule.Http.Headers;

namespace HtcPlugin.Php.Processor.Models {
    public class PhpRequest {

        public static async Task Request(HttpContext httpContext, string filename, int timeout) {
            using var phpProcess = new Process {
                StartInfo = { FileName = PhpProcessor.PhpCgiExec }
            };
            var queryString = httpContext.Request.QueryString.ToString();
            if (!string.IsNullOrEmpty(queryString)) queryString = queryString.Remove(0, 1);
            phpProcess.StartInfo.EnvironmentVariables.Add("PHPRC", PhpProcessor.PhpPath);
            phpProcess.StartInfo.EnvironmentVariables.Add("HTTP_HOST", httpContext.Request.Headers["Host"]);
            phpProcess.StartInfo.EnvironmentVariables.Add("HTTP_CONNECTION", httpContext.Request.Headers["Connection"]);
            phpProcess.StartInfo.EnvironmentVariables.Add("HTTP_UPGRADE_INSECURE_REQUESTS", httpContext.Request.Headers["Upgrade-Insecure-Requests"]);
            phpProcess.StartInfo.EnvironmentVariables.Add("HTTP_USER_AGENT", httpContext.Request.Headers["User-Agent"]);
            phpProcess.StartInfo.EnvironmentVariables.Add("HTTP_DNT", httpContext.Request.Headers["DNT"]);
            phpProcess.StartInfo.EnvironmentVariables.Add("HTTP_ACCEPT", httpContext.Request.Headers["Accept"]);
            phpProcess.StartInfo.EnvironmentVariables.Add("HTTP_ACCEPT_ENCODING", httpContext.Request.Headers["Accept-Encoding"]);
            phpProcess.StartInfo.EnvironmentVariables.Add("HTTP_ACCEPT_LANGUAGE", httpContext.Request.Headers["Accept-Language"]);
            phpProcess.StartInfo.EnvironmentVariables.Add("HTTP_COOKIE", httpContext.Request.Headers["Cookie"]);
            phpProcess.StartInfo.EnvironmentVariables.Add("SERVER_SOFTWARE", "HtcSharp/1.2");
            phpProcess.StartInfo.EnvironmentVariables.Add("SERVER_SIGNATURE", $"<address>HtcSharp/1.0 Server at {httpContext.Request.Host.Host} Port {httpContext.Connection.RemotePort}</address>");
            phpProcess.StartInfo.EnvironmentVariables.Add("SERVER_ADDR", httpContext.Request.Host.ToString());
            phpProcess.StartInfo.EnvironmentVariables.Add("SERVER_NAME", httpContext.Request.Host.ToString());
            phpProcess.StartInfo.EnvironmentVariables.Add("SERVER_PORT", httpContext.Connection.RemotePort.ToString());
            phpProcess.StartInfo.EnvironmentVariables.Add("REMOTE_ADDR", httpContext.Connection.RemoteIpAddress.ToString());
            phpProcess.StartInfo.EnvironmentVariables.Add("DOCUMENT_ROOT", httpContext.ServerInfo.RootPath.Replace(@"\", "/"));
            phpProcess.StartInfo.EnvironmentVariables.Add("REQUEST_SCHEME", httpContext.Request.Scheme);
            phpProcess.StartInfo.EnvironmentVariables.Add("CONTEXT_DOCUMENT_ROOT", httpContext.ServerInfo.RootPath.Replace(@"\", "/"));
            phpProcess.StartInfo.EnvironmentVariables.Add("SCRIPT_FILENAME", filename);
            phpProcess.StartInfo.EnvironmentVariables.Add("REMOTE_PORT", httpContext.Connection.RemotePort.ToString());
            phpProcess.StartInfo.EnvironmentVariables.Add("GATEWAY_INTERFACE", "CGI/1.1");
            phpProcess.StartInfo.EnvironmentVariables.Add("SERVER_PROTOCOL", "HTTP/1.1");
            phpProcess.StartInfo.EnvironmentVariables.Add("REQUEST_METHOD", httpContext.Request.Method.ToString());
            phpProcess.StartInfo.EnvironmentVariables.Add("QUERY_STRING", queryString);
            phpProcess.StartInfo.EnvironmentVariables.Add("REQUEST_URI", httpContext.Request.RequestPath);
            phpProcess.StartInfo.EnvironmentVariables.Add("SCRIPT_NAME", httpContext.Request.RequestFilePath);
            phpProcess.StartInfo.EnvironmentVariables.Add("PHP_SELF", httpContext.Request.RequestFilePath); // Maybe the script file ??
            phpProcess.StartInfo.EnvironmentVariables.Add("REDIRECT_STATUS", "200");
            phpProcess.StartInfo.EnvironmentVariables.Add("HTTP_REFERER", httpContext.Request.Headers["Referer"]);
            phpProcess.StartInfo.EnvironmentVariables.Add("CONTENT_LENGTH", httpContext.Request.ContentLength.ToString());
            phpProcess.StartInfo.EnvironmentVariables.Add("CONTENT_TYPE", httpContext.Request.Headers["Content-Type"]);
            phpProcess.StartInfo.EnvironmentVariables.Add("HTTPS", httpContext.Request.IsHttps ? "1" : "0");
            phpProcess.StartInfo.EnvironmentVariables.Add("REMOTE_HOST", httpContext.Request.Host.Host);
            phpProcess.StartInfo.EnvironmentVariables.Add("CONTEXT_PREFIX", "");

            phpProcess.StartInfo.UseShellExecute = false;
            phpProcess.StartInfo.RedirectStandardInput = true;
            phpProcess.StartInfo.RedirectStandardOutput = true;
            phpProcess.StartInfo.RedirectStandardError = true;
            phpProcess.Start();
            await httpContext.Request.Body.CopyToAsync(phpProcess.StandardInput.BaseStream);
            await phpProcess.StandardInput.BaseStream.FlushAsync();
            try {
                var reader = new UnbufferedStreamReader(phpProcess.StandardOutput.BaseStream);
                while (true) {
                    string line = reader.ReadLine();
                    //PhpProcessor.Logger.LogInfo($"{line}");
                    if (line == null || line.Equals("\r")) break;
                    if (line[^1] == '\r') line = line.Remove(line.Length - 1, 1);
                    string[] tag = line.Split(": ", 2);
                    if (tag.Length == 1) continue;
                    if (httpContext.Response.HasStarted) return;
                    switch (tag[0].ToLower()) {
                        case "location":
                            httpContext.Response.Redirect(HttpUtility.UrlDecode(tag[1]));
                            break;
                        case "set-cookie":
                            //PhpProcessor.Logger.LogInfo("Cookie -> " + tag[1]);
                            string[] cookieData = tag[1].Split(";");
                            string[] baseCookie = cookieData[0].Split("=", 2);
                            var cookieOptions = new CookieOptions();
                            if (cookieData.Length > 1) {
                                for (var i = 1; i < cookieData.Length; i++) {
                                    string[] subData = cookieData[i].Split("=", 2);
                                    switch (subData[0].ToLower()) {
                                        case "path":
                                            cookieOptions.Path = HttpUtility.UrlDecode(subData[1]);
                                            break;
                                        case "httponly":
                                            cookieOptions.HttpOnly = true;
                                            break;
                                        case "max-age":
                                            cookieOptions.MaxAge = TimeSpan.FromSeconds(int.Parse(subData[1]));
                                            break;
                                        case "expires":
                                            cookieOptions.Expires = DateTimeOffset.Parse(subData[1]);
                                            break;
                                    }
                                }
                            }
                            httpContext.Response.Cookies.Append(baseCookie[0], HttpUtility.UrlDecode(baseCookie[1]), cookieOptions);
                            break;
                        default:
                            if (httpContext.Response.Headers.ContainsKey(tag[0])) {
                                httpContext.Response.Headers[tag[0]] = string.Concat(httpContext.Response.Headers[tag[0]], tag[1]);
                            } else {
                                httpContext.Response.Headers.Add(tag[0], HttpUtility.UrlDecode(tag[1]));
                            }
                            break;
                    }
                }
                await phpProcess.StandardOutput.BaseStream.CopyToAsync(httpContext.Response.Body);
            } catch (Exception ex) {
                PhpProcessor.Logger.LogTrace(ex.StackTrace, ex);
            }
            phpProcess.WaitForExit();
        }
    }
}
