# HTCSharp
Web server for developing applications not based on the Asp.Net framework but using it as the basis for the server.

### Why use it ?

The HtcSharp project aims not to get the developer into how AspNet works by trying to be as minimal as possible, such as HttpListener but allowing some of AspNet's features to be used.

### Why was this project developed ?

When trying to work with Asp Net I felt very stuck the way the framework works and processes requests, the truth is that I prefer the low level of HttpListener, but this is a legacy technology that does not support encryption, so there was a need to use an Http API with encryption support.

The project's attempt is to make it look like the Nginx server by configuration, and allow the implementation of plugins and modules. The modularity of the project allows multiple modules to be implemented by simply adding a dll. And plugins allow scripts like Lua and PHP to be executed to render pages.

This also allows one part of the code to be executed in CSharp and the other part in a scripting language like Lua, so the page is rendered by Lua while the database, algorithms and others are executed in CSharp.

### The future of the project and what I expect from it

It all started just as a personal project to allow me to develop my sites more easily, the viability of the project for commercial or normal use is still very low, as I still consider it in Alpha.
Despite the use of Asp Net, this technology does not please me since I have to reprocess all requests, besides it seems slow by itself, so I am creating my own http server that would eliminate dependence on Asp Net but it may take some time until it is finished.

## Getting Started
To use this program you must have .Net Core 3.0 installed, in the future self-contained releases will be published.

### Prerequisites
*   [.Net Core 3.1](https://dotnet.microsoft.com/download)
*   [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json/12.0.2)
*   [CorePluginLoader](https://www.nuget.org/packages/CorePluginLoader/)
*   [Microsoft.Extensions.Logging.Abstractions](https://www.nuget.org/packages/Microsoft.Extensions.Logging.Abstractions/)
*   [Microsoft.Extensions.Configuration](https://www.nuget.org/packages/Microsoft.Extensions.Configuration/)
*   [Microsoft.Extensions.Configuration.Abstractions](https://www.nuget.org/packages/Microsoft.Extensions.Configuration.Abstractions/)
*   [Microsoft.Extensions.Configuration.Binder](https://www.nuget.org/packages/Microsoft.Extensions.Configuration.Binder/)
*   [Microsoft.Extensions.DependencyInjection.Abstractions](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection.Abstractions/)
*   [Microsoft.Extensions.Logging](https://www.nuget.org/packages/Microsoft.Extensions.Logging/)
*   [Microsoft.Extensions.ObjectPool](https://www.nuget.org/packages/Microsoft.Extensions.ObjectPool/)
*   [Microsoft.Extensions.Options](https://www.nuget.org/packages/Microsoft.Extensions.Options/)
*   [Microsoft.Extensions.Primitives](https://www.nuget.org/packages/Microsoft.Extensions.Primitives/)
*   [System.IO.Pipelines](https://www.nuget.org/packages/System.IO.Pipelines/)
*   [System.Security.Cryptography.Cng](https://www.nuget.org/packages/System.Security.Cryptography.Cng/)

### Prerequisites for compiling plugins:
*   [MoonSharp](https://www.nuget.org/packages/MoonSharp/)
*   [MySqlConnector](https://www.nuget.org/packages/MySqlConnector/)

### Installation
For now the only way to install is to download the repository and compile the solution, in the future binary versions will be released.

## Usage

### Console mode for Linux, Windows and Mac
```sh
dotnet <path>/HtcSharp.Server.dll <optional args>
```

### Daemon Service Mode for Linux
```sh
dotnet <path>/HtcSharp.Server.dll daemon-mode
```

#### Optional Args
*   daemon-mode - Causes the server to run in daemon mode by disabling console input.
*   "../HtcConfig.json" - Specifies the path where the server can get the configuration file.

## Configuration

I tried my best to make the configuration look like Nginx, so I think it's easy for those who already know about it.

Example Configuration:
```json
{
    "ModulesPath": "%WorkingPath%\\modules\\",
    "PluginsPath": "%WorkingPath%\\plugins\\",
    "Engines": {
        "htc-http": {
            "Servers": [
                {
                   "Hosts":[
                      "0.0.0.0:8080"
                   ],
                   "Domains":[
                      "*"
                   ],
                   "Default":[
                      "try_pages $uri",
                      "try_files $uri",
                      "index $internal_indexes",
                      "return 404"
                   ],
                   "Root":"%WorkingPath%/www/",
                   "SSL":false
                }
             ]
        }
    }
}
```

## Wiki
See the [wiki](https://github.com/jpdante/HtcSharp/wiki) for more information on how to use it.

## Roadmap
See the [open issues](https://github.com/jpdante/HtcSharp/issues) for a list of proposed features (and known issues).

## License
Distributed under the Apache-2.0 License. See `LICENSE` for more information.
