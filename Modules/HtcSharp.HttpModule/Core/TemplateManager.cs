using System.Collections.Generic;
using HtcSharp.HttpModule.Abstractions;
using HtcSharp.HttpModule.Core.Templates;
using HtcSharp.HttpModule.Http;

namespace HtcSharp.HttpModule.Core {
    public class TemplateManager {

        private const string Templete400 = "<!DOCTYPE html><html lang=\"en\"><head><meta charset=\"utf-8\" /><meta name=\"viewport\" content=\"width=device-width, initial-scale=1\" /><title>400 - $Path</title><style>html{width:100%;height:100%;min-width:350px;background-color:#f8f8f8}body{margin:0;padding:30px}.container{background-color:#fff;border-radius:15px;font-family:Arial,Helvetica,sans-serif;color:black;padding:20px}h1{margin:0 0 10px 0;word-wrap:break-word;text-align:center;font-size:40px}h2{margin:0 0 10px 0;word-wrap:break-word;text-align:center}span{margin-top:5px;display:block;text-align:center}bold{font-weight:bold}.red{color:#C20D2F}</style></head><body><div class=\"container\"><h1><color class=\"red\">400 - Bad Request</color></h1><h2>An internal server error occurred and it was not possible to process your request.</h2><h2>Please try again later.</h2> <span>Rendered by <bold>HtcSharp</bold></span></div></body></html>";
        private const string Templete403 = "<!DOCTYPE html><html lang=\"en\"><head><meta charset=\"utf-8\" /><meta name=\"viewport\" content=\"width=device-width, initial-scale=1\" /><title>403 - $Path</title><style>html{width:100%;height:100%;min-width:350px;background-color:#f8f8f8}body{margin:0;padding:30px}.container{background-color:#fff;border-radius:15px;font-family:Arial,Helvetica,sans-serif;color:black;padding:20px}h1{margin:0 0 10px 0;word-wrap:break-word;text-align:center;font-size:40px}h2{margin:0 0 10px 0;word-wrap:break-word;text-align:center}span{margin-top:5px;display:block;text-align:center}bold{font-weight:bold}.red{color:#C20D2F}</style></head><body><div class=\"container\"><h1><color class=\"red\">403 - Forbidden</color></h1><h2>It was not possible to process the request because you are not authorized.</h2> <span>Rendered by <bold>HtcSharp</bold></span></div></body></html>";
        private const string Templete404 = "<!DOCTYPE html><html lang=\"en\"><head><meta charset=\"utf-8\" /><meta name=\"viewport\" content=\"width=device-width, initial-scale=1\" /><title>404 - $Path</title><style>html{width:100%;height:100%;min-width:350px;background-color:#f8f8f8}body{margin:0;padding:30px}.container{background-color:#fff;border-radius:15px;font-family:Arial,Helvetica,sans-serif;color:black;padding:20px}h1{margin:0 0 10px 0;word-wrap:break-word;text-align:center;font-size:40px}h2{margin:0 0 10px 0;word-wrap:break-word;text-align:center}span{margin-top:5px;display:block;text-align:center}bold{font-weight:bold}.red{color:#C20D2F}</style></head><body><div class=\"container\"><h1><color class=\"red\">404 - Page Not Found</color></h1><h2>The requested url '%RequestUrl%' was not found.</h2> <span>Rendered by <bold>HtcSharp</bold></span></div></body></html>";
        private const string Templete500 = "<!DOCTYPE html><html lang=\"en\"><head><meta charset=\"utf-8\" /><meta name=\"viewport\" content=\"width=device-width, initial-scale=1\" /><title>500 - $Path</title><style>html{width:100%;height:100%;min-width:350px;background-color:#f8f8f8}body{margin:0;padding:30px}.container{background-color:#fff;border-radius:15px;font-family:Arial,Helvetica,sans-serif;color:black;padding:20px}h1{margin:0 0 10px 0;word-wrap:break-word;text-align:center;font-size:40px}h2{margin:0 0 10px 0;word-wrap:break-word;text-align:center}span{margin-top:5px;display:block;text-align:center}bold{font-weight:bold}.red{color:#C20D2F}</style></head><body><div class=\"container\"><h1><color class=\"red\">500 - Internal Server Error</color></h1><h2>An internal server error occurred and it was not possible to process your request.</h2><h2>Please try again later.</h2> <span>Rendered by <bold>HtcSharp</bold></span></div></body></html>";

        private const string DirListHeader = "<!DOCTYPE html><html lang=\"en\"><head><meta charset=\"utf-8\" /><title>Listing of $RelativePath</title><style>html{width:100%;height:100%;min-width:600px;background-color:#f8f8f8}body{margin:0;padding:30px}.container{background-color:#fff;border-radius:15px;font-family:Arial,Helvetica,sans-serif;color:#000;padding:10px}h1{margin:0 0 10px 0;word-wrap:break-word}table{table-layout:auto;border-collapse:collapse;width:100%}tr:nth-child(even){background-color:#f8f8f8}tr:hover{background-color:#ccc}tr:last-child td:first-child{border-bottom-left-radius:5px}tr:last-child td:last-child{border-bottom-right-radius:5px}th{padding:8px 8px 8px 8px;text-align:left;background-color:#008cff;color:#fff}th:first-child{border-top-left-radius:5px}th:last-child{border-top-right-radius:5px}td{padding:8px;white-space:nowrap}td:first-child{white-space:normal}a{text-decoration:none;width:100%;display:block}span{margin-top:5px;display:block}bold{font-weight:700}</style></head><body><div class=\"container\"><h1>Listing of $RelativePath</h1><table><tr><th>Name</th><th>Size</th><th>Last modified</th></tr>";
        private const string DirListFRow = "<tr><td><a href=\"$RelativePath\">$FileName</a></td><td>$Size</td><td>$LastModified</td></tr>";
        private const string DirListDRow = "<tr><td><a href=\"$RelativePath\">$FileName</a></td><td>$Size</td><td>$LastModified</td></tr>";
        private const string DirListFooter = "</table> <span>Rendered by <bold>HtcSharp</bold> in $RenderTime.</span></div></body></html>";

        private readonly Dictionary<string, ITemplate> _templates;

        public TemplateManager() {
            _templates = new Dictionary<string, ITemplate> {
                {"400", new StaticTemplate(Templete400, ContentType.HTML)},
                {"403", new StaticTemplate(Templete403, ContentType.HTML)},
                {"404", new StaticTemplate(Templete404, ContentType.HTML)},
                {"500", new StaticTemplate(Templete500, ContentType.HTML)},
                {"DirListHeader", new StaticTemplate(DirListHeader, ContentType.HTML)},
                {"DirListFRow", new StaticTemplate(DirListFRow, ContentType.HTML)},
                {"DirListDRow", new StaticTemplate(DirListDRow, ContentType.HTML)},
                {"DirListFooter", new StaticTemplate(DirListFooter, ContentType.HTML)},
            };
        }

        public void SetTemplate(string key, ITemplate template) {
            _templates[key] = template;
        }

        public void RemoveTemplate(string key) {
            _templates.Remove(key);
        }

        public ITemplate GetTemplate(string key) {
            return _templates.TryGetValue(key, out var value) ? value : null;
        }

        public bool TryGetTemplate(string key, out ITemplate value) {
            return _templates.TryGetValue(key, out value);
        }
    }
}