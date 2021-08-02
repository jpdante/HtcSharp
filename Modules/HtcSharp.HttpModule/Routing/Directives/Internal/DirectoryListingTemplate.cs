﻿namespace HtcSharp.HttpModule.Routing.Directives.Internal {
    public class DirectoryListingTemplate {
        
        public string Header { get; set; }

        public string FileRow { get; set; }

        public string DirectoryRow { get; set; }

        public string Footer { get; set; }

        public DirectoryListingTemplate() {
            Header = "<!DOCTYPE html><html lang=\"en\"><head><meta charset=\"utf-8\" /><title>Listing of $RelativePath</title><style>html{width:100%;height:100%;min-width:600px;background-color:#f8f8f8}body{margin:0;padding:30px}.container{background-color:#fff;border-radius:15px;font-family:Arial,Helvetica,sans-serif;color:#000;padding:10px}h1{margin:0 0 10px 0;word-wrap:break-word}table{table-layout:auto;border-collapse:collapse;width:100%}tr:nth-child(even){background-color:#f8f8f8}tr:hover{background-color:#ccc}tr:last-child td:first-child{border-bottom-left-radius:5px}tr:last-child td:last-child{border-bottom-right-radius:5px}th{padding:8px 8px 8px 8px;text-align:left;background-color:#008cff;color:#fff}th:first-child{border-top-left-radius:5px}th:last-child{border-top-right-radius:5px}td{padding:8px;white-space:nowrap}td:first-child{white-space:normal}a{text-decoration:none;width:100%;display:block}span{margin-top:5px;display:block}bold{font-weight:700}</style></head><body><div class=\"container\"><h1>Listing of $RelativePath</h1><table><tr><th>Name</th><th>Size</th><th>Last modified</th></tr>";
            FileRow = "<tr><td><a href=\"$RelativePath\">$FileName</a></td><td>$Size</td><td>$LastModified</td></tr>";
            DirectoryRow = "<tr><td><a href=\"$RelativePath\">$FileName</a></td><td>$Size</td><td>$LastModified</td></tr>";
            Footer = "</table> <span>Rendered by <bold>HtcSharp</bold> in $RenderTime.</span></div></body></html>";
        }

    }
}