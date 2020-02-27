print("[LuaBuildEvents] Starting HtcPlugin.Php.Processor PostBuild\n")
require("lua.io")

if args[2] == "Debug" then
    print("[LuaBuildEvents] Running in Debug Mode\n")
    pluginsPath = Path.combine(args[3], [[HtcSharp.Server/bin/Debug/plugins]])
    if Directory.exists(pluginsPath) == false then
	    Directory.createDirectory(pluginsPath)
	end
	File.copy(args[4], Path.combine(args[3], [[HtcSharp.Server/bin/Debug/plugins]], args[5]), true)
	File.copy(Path.combine(args[3], [[Plugins/HtcPlugin.Php.Processor/bin/Debug]], "HtcPlugin.Php.Processor.plugin.pdb"), Path.combine(args[3], [[HtcSharp.Server/bin/Debug/plugins]], "HtcPlugin.Php.Processor.plugin.pdb"), true)
elseif args[2] == "Release" then
    print("[LuaBuildEvents] Running in Release Mode\n")
    pluginsPath = Path.combine(args[3], [[HtcSharp.Server/bin/Release/plugins]])
    if Directory.exists(pluginsPath) == false then
	    Directory.createDirectory(pluginsPath)
	end
	File.copy(args[4], Path.combine(args[3], [[HtcSharp.Server/bin/Release/plugins]], args[5]), true)
	File.copy(Path.combine(args[3], [[Plugins/HtcPlugin.Php.Processor/bin/Release]], "HtcPlugin.Php.Processor.plugin.pdb"), Path.combine(args[3], [[HtcSharp.Server/bin/Release/plugins]], "HtcPlugin.Php.Processor.plugin.pdb"), true)
end
print("[LuaBuildEvents] Finishing HtcPlugin.Php.Processor PostBuild\n")