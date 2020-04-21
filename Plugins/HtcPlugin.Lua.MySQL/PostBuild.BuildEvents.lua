print("[LuaBuildEvents] Starting HtcPlugin.Lua.MySql PostBuild\n")
require("lua.io")

-- Filename and directory blacklist
function isBlackListed(path)
  if path == [[CorePluginLoader.dll]] then return true
  elseif path == [[HtcSharp.Core.dll]] then return true
  elseif path == [[HtcSharp.Core.pdb]] then return true
  elseif path == [[runtimes]] then return true
  elseif path == [[runtimes\win]] then return true
  elseif path == [[runtimes\win\lib]] then return true
  elseif path == [[runtimes\win\lib\netcoreapp2.0]] then return true
  elseif path == [[runtimes\win\lib\netcoreapp2.0\System.Diagnostics.EventLog.dll]] then return true
  elseif path == [[HtcSharp.HttpModule.module.dll]] then return true
  elseif path == [[HtcSharp.HttpModule.module.pdb]] then return true
  elseif path == [[HtcPlugin.Lua.Processor.plugin.dll]] then return true
  elseif path == [[HtcPlugin.Lua.Processor.plugin.pdb]] then return true
  elseif path == [[Microsoft.Extensions.Configuration.Abstractions.dll]] then return true
  elseif path == [[Microsoft.Extensions.Configuration.Binder.dll]] then return true
  elseif path == [[Microsoft.Extensions.Configuration.CommandLine.dll]] then return true
  elseif path == [[Microsoft.Extensions.Configuration.dll]] then return true
  elseif path == [[Microsoft.Extensions.Configuration.EnvironmentVariables.dll]] then return true
  elseif path == [[Microsoft.Extensions.Configuration.FileExtensions.dll]] then return true
  elseif path == [[Microsoft.Extensions.Configuration.Json.dll]] then return true
  elseif path == [[Microsoft.Extensions.Configuration.UserSecrets.dll]] then return true
  elseif path == [[Microsoft.Extensions.DependencyInjection.Abstractions.dll]] then return true
  elseif path == [[Microsoft.Extensions.DependencyInjection]] then return true
  elseif path == [[Microsoft.Extensions.FileProviders.Abstractions.dll]] then return true
  elseif path == [[Microsoft.Extensions.FileProviders.Physical.dll]] then return true
  elseif path == [[Microsoft.Extensions.FileSystemGlobbing.dll]] then return true
  elseif path == [[Microsoft.Extensions.Hosting.Abstractions.dll]] then return true
  elseif path == [[Microsoft.Extensions.Hosting.dll]] then return true
  elseif path == [[Microsoft.Extensions.Logging.Abstractions.dll]] then return true
  elseif path == [[Microsoft.Extensions.Logging.Configuration.dll]] then return true
  elseif path == [[Microsoft.Extensions.Logging.Console.dll]] then return true
  elseif path == [[Microsoft.Extensions.Logging.Debug.dll]] then return true
  elseif path == [[Microsoft.Extensions.Logging.dll]] then return true
  elseif path == [[Microsoft.Extensions.Logging.EventLog.dll]] then return true
  elseif path == [[Microsoft.Extensions.Logging.EventSource.dll]] then return true
  elseif path == [[Microsoft.Extensions.ObjectPool.dll]] then return true
  elseif path == [[Microsoft.Extensions.Options.ConfigurationExtensions.dll]] then return true
  elseif path == [[Microsoft.Extensions.Options.dll]] then return true
  elseif path == [[Microsoft.Extensions.Primitives.dll]] then return true
  elseif path == [[Newtonsoft.Json.dll]] then return true
  elseif path == [[System.Diagnostics.EventLog.dll]] then return true
  elseif path == [[System.IO.Pipelines.dll]] then return true
  elseif path == [[Microsoft.Extensions.DependencyInjection.dll]] then return true
  else return false
  end
end

print("[LuaBuildEvents] Running in " .. args[2] .. " Mode\n")

-- Create base directory
pluginsPath = Path.combine(args[3], [[HtcSharp.Server/bin/]] .. args[2] .. [[/plugins/lua-mysql]])
if Directory.exists(pluginsPath) == false then
  Directory.createDirectory(pluginsPath)
end

-- Create directories
outputDirectory = Path.combine(args[4], args[5])
outputDirectories = Directory.getDirectories(outputDirectory, "*", SearchOption.AllDirectories)
for key,value in ipairs(outputDirectories) do
  directoryReplace = value:gsub(outputDirectory, "")
  if isBlackListed(directoryReplace) == true then
    print("Blacklisted directory: " .. Path.getFileName(directoryReplace) .. "\n")
    goto continue
  end
  fixedDirectory = Path.combine(pluginsPath, directoryReplace)
  if Directory.exists(fixedDirectory) == false then
    print("Creating directory: " .. fixedDirectory .. "\n")
    Directory.createDirectory(fixedDirectory)
  end
  ::continue::
end

-- Copy files
outputFiles = Directory.getFiles(outputDirectory, "*.*", SearchOption.AllDirectories)
for key,value in ipairs(outputFiles) do
  fileNameReplace = value:gsub(outputDirectory, "")
  if isBlackListed(Path.getFileName(fileNameReplace)) == true then
    print("Blacklisted file: " .. Path.getFileName(fileNameReplace) .. "\n")
    goto continue
  end
  fixedFileName = Path.combine(pluginsPath, fileNameReplace)
  print("Copying file: " .. fixedFileName .. "\n")
  File.copy(value, fixedFileName, true)
  ::continue::
end

print("[LuaBuildEvents] Finishing HtcPlugin.Lua.MySql PostBuild\n")