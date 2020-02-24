using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Http.Features;
using HtcSharp.HttpModule.Http.Headers;
using Microsoft.Extensions.Primitives;

namespace HtcPlugin.Php.Processor.Models {
    public class PhpRequest {
        public static async Task Request(HttpContext httpContext, string filename, int timeout) {
            //httpContext.Response.ContentType = "text/html; charset=utf-8";
            using var phpProcess = new Process {
                StartInfo = {FileName = PhpProcessor.PhpCgiExec}
            };
            //var pathVars = phpProcess.StartInfo.EnvironmentVariables["Path"];
            var queryString = httpContext.Request.QueryString.ToString();
            if(!string.IsNullOrEmpty(queryString)) queryString = queryString.Remove(0,1);
            //queryString = Uri.UnescapeDataString(queryString);
            //phpProcess.StartInfo.EnvironmentVariables.Clear();
            phpProcess.StartInfo.EnvironmentVariables.Add("PHPRC", PhpProcessor.PhpPath);
            phpProcess.StartInfo.EnvironmentVariables.Add("HTTP_HOST", httpContext.Request.Headers.GetValueOrDefault("Host"));
            phpProcess.StartInfo.EnvironmentVariables.Add("HTTP_CONNECTION", httpContext.Request.Headers.GetValueOrDefault("Connection"));
            phpProcess.StartInfo.EnvironmentVariables.Add("HTTP_UPGRADE_INSECURE_REQUESTS", httpContext.Request.Headers.GetValueOrDefault("Upgrade-Insecure-Requests"));
            phpProcess.StartInfo.EnvironmentVariables.Add("HTTP_USER_AGENT", httpContext.Request.Headers.GetValueOrDefault("User-Agent"));
            phpProcess.StartInfo.EnvironmentVariables.Add("HTTP_DNT", httpContext.Request.Headers.GetValueOrDefault("DNT"));
            phpProcess.StartInfo.EnvironmentVariables.Add("HTTP_ACCEPT", httpContext.Request.Headers.GetValueOrDefault("Accept"));
            phpProcess.StartInfo.EnvironmentVariables.Add("HTTP_ACCEPT_ENCODING", httpContext.Request.Headers.GetValueOrDefault("Accept-Encoding"));
            phpProcess.StartInfo.EnvironmentVariables.Add("HTTP_ACCEPT_LANGUAGE", httpContext.Request.Headers.GetValueOrDefault("Accept-Language"));
            phpProcess.StartInfo.EnvironmentVariables.Add("HTTP_COOKIE", httpContext.Request.Headers.GetValueOrDefault("Cookie"));
            //phpProcess.StartInfo.EnvironmentVariables.Add("PATH", pathVars);
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
            phpProcess.StartInfo.EnvironmentVariables.Add("HTTP_REFERER", httpContext.Request.Headers.GetValueOrDefault("Referer"));
            phpProcess.StartInfo.EnvironmentVariables.Add("CONTENT_LENGTH", httpContext.Request.ContentLength.ToString());
            phpProcess.StartInfo.EnvironmentVariables.Add("CONTENT_TYPE", httpContext.Request.Headers.GetValueOrDefault("Content-Type"));
            phpProcess.StartInfo.EnvironmentVariables.Add("HTTPS", httpContext.Request.IsHttps ? "1" : "0");
            phpProcess.StartInfo.EnvironmentVariables.Add("REMOTE_HOST", httpContext.Request.Host.Host);
            phpProcess.StartInfo.EnvironmentVariables.Add("CONTEXT_PREFIX", "");
            //phpProcess.StartInfo.EnvironmentVariables.Add("PATH_INFO", httpContext.Request.RequestPath);
            //phpProcess.StartInfo.EnvironmentVariables.Add("PATH_TRANSLATED", httpContext.Request.TranslatedPath);

            phpProcess.StartInfo.UseShellExecute = false;
            phpProcess.StartInfo.RedirectStandardInput = true;
            phpProcess.StartInfo.RedirectStandardOutput = true;
            phpProcess.StartInfo.RedirectStandardError = true;
            phpProcess.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            var isResponse = false;
            phpProcess.Start();
            httpContext.Request.Body.CopyTo(phpProcess.StandardInput.BaseStream);
            phpProcess.StandardInput.BaseStream.Flush();
            phpProcess.StandardInput.Flush();
            //try {
            while (!phpProcess.StandardOutput.EndOfStream) {
                string line = phpProcess.StandardOutput.ReadLine();
                if (line == null) continue;
                if (line == "" && isResponse == false) {
                    isResponse = true;
                    continue;
                }
                if (isResponse) await httpContext.Response.WriteAsync(line + Environment.NewLine);
                else {
                    //var tag = line.Split(new[] {':'}, 2);
                    //tag[1] = tag[1].Remove(0, 1);
                    //httpContext.Response.Headers.Add(tag[0], tag[1]);
                    //Logger.Info($"{tag[0]}=>{tag[1]}");
                    //Logger.Info($"{line}");
                    string[] tag = line.Split(new[] {':'}, 2);
                    tag[1] = tag[1].Remove(0, 1);
                    switch (tag[0]) {
                        case "Location":
                            httpContext.Response.Redirect(tag[1]);
                            break;
                        case "Set-Cookie":
                            var cookieOptions = new CookieOptions() {
                                SameSite = SameSiteMode.None
                            };
                            string[] cookieData = tag[1].Split(';', 2);
                            string[] cookieKeyValue = cookieData[0].Split('=', 2);
                            var setCookieHeaderValue = new SetCookieHeaderValue(Uri.UnescapeDataString(cookieKeyValue[0]), Uri.UnescapeDataString(cookieKeyValue[1]));
                            foreach (string cookieConfig in cookieData[1].Split(';')) {
                                string[] config = cookieConfig.Split('=', 2);
                                config[0] = config[0].Remove(0, 1);
                                if (config[0].Equals("Max-Age", StringComparison.CurrentCultureIgnoreCase)) {
                                    setCookieHeaderValue.MaxAge = TimeSpan.FromSeconds(int.Parse(config[1]));
                                } else if (config[0].Equals("path", StringComparison.CurrentCultureIgnoreCase)) {
                                    setCookieHeaderValue.Path = config[1];
                                } else if (config[0].Equals("domain", StringComparison.CurrentCultureIgnoreCase)) {
                                    setCookieHeaderValue.Domain = config[1];
                                } else if (config[0].Equals("HttpOnly", StringComparison.CurrentCultureIgnoreCase)) {
                                    setCookieHeaderValue.HttpOnly = true;
                                } else if (config[0].Equals("expires", StringComparison.CurrentCultureIgnoreCase)) {
                                    setCookieHeaderValue.Expires = DateTimeOffset.Parse(config[1]);
                                } else if (config[0].Equals("SameSite", StringComparison.CurrentCultureIgnoreCase)) {
                                    setCookieHeaderValue.SameSite = SameSiteMode.Lax;
                                }
                            }
                            if (cookieKeyValue[1] == "+") cookieKeyValue[1] = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString();
                            //Logger.Info($"{Uri.UnescapeDataString(cookieKeyValue[0])}: {Uri.UnescapeDataString(cookieKeyValue[1])}");
                            var cookieValue = setCookieHeaderValue.ToString();
                            httpContext.Response.Headers[HeaderNames.SetCookie] = StringValues.Concat(httpContext.Response.Headers[HeaderNames.SetCookie], cookieValue);
                            break;
                        default:
                            if (httpContext.Response.Headers.ContainsKey(tag[0])) {
                                httpContext.Response.Headers[tag[0]] = string.Concat(httpContext.Response.Headers[tag[0]], tag[1]);
                            } else {
                                httpContext.Response.Headers.Add(tag[0], tag[1]);
                            }
                            break;
                    }
                }
            }
            //} catch(Exception ex) {
            //    throw ex;
            //httpContext.Response.Write(phpProcess.StandardOutput.ReadToEnd());
            //}
            phpProcess.WaitForExit();
        }
    }
}
