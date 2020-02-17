print("[LuaBuildEvents] Starting HtcPlugin.Lua.Processor PostBuild\n")
require("lua.io")

if args[2] == "Debug" then
    print("[LuaBuildEvents] Running in Debug Mode\n")
    pluginsPath = Path.combine(args[3], [[HtcSharp.Server/bin/Debug/plugins]])
    if Directory.exists(pluginsPath) == false then
	    Directory.createDirectory(pluginsPath)
	end
	File.copy(args[4], Path.combine(args[3], [[HtcSharp.Server/bin/Debug/plugins]], args[5]), true)
	File.copy(Path.combine(args[3], [[Plugins/HtcPlugin.Lua.Processor/bin/Debug]], "MoonSharp.Interpreter.dll"), Path.combine(args[3], [[HtcSharp.Server/bin/Debug/plugins]], "MoonSharp.Interpreter.dll"), true)
elseif args[2] == "Release" then
    print("[LuaBuildEvents] Running in Release Mode\n")
    pluginsPath = Path.combine(args[3], [[HtcSharp.Server/bin/Release/plugins]])
    if Directory.exists(pluginsPath) == false then
	    Directory.createDirectory(pluginsPath)
	end
	File.copy(args[4], Path.combine(args[3], [[HtcSharp.Server/bin/Release/plugins]], args[5]), true)
	File.copy(Path.combine(args[3], [[Plugins/HtcPlugin.Lua.Processor/bin/Release]], "MoonSharp.Interpreter.dll"), Path.combine(args[3], [[HtcSharp.Server/bin/Release/plugins]], "MoonSharp.Interpreter.dll"), true)
end
print("[LuaBuildEvents] Finishing HtcPlugin.Lua.Processor PostBuild\n")