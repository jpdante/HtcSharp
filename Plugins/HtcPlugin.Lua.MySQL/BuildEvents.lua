print("[LuaBuildEvents] Starting HtcPlugin.Lua.MySql PostBuild")
require("lua.io")

if args[2] == "Debug" then
    print("[LuaBuildEvents] Running in Debug Mode")
    pluginsPath = Path.combine(args[3], [[HtcSharp.Server\bin\Debug\plugins\]])
    if Directory.exists(pluginsPath) == false then
	    Directory.createDirectory(pluginsPath)
	end
	File.copy(args[4], Path.combine(args[3], [[HtcSharp.Server\bin\Debug\plugins\]] .. args[5]), true)
	File.copy(Path.combine(args[3], [[Plugins\HtcPlugin.Lua.MySQL\bin\Debug\MySqlConnector.dll]]), Path.combine(args[3], [[HtcSharp.Server\bin\Debug\plugins\MySqlConnector.dll]]), true)
elseif args[2] == "Release" then
    print("[LuaBuildEvents] Running in Release Mode")
    pluginsPath = Path.combine(args[3], [[HtcSharp.Server\bin\Release\plugins\]])
    if Directory.exists(pluginsPath) == false then
	    Directory.createDirectory(pluginsPath)
	end
	File.copy(args[4], Path.combine(args[3], [[HtcSharp.Server\bin\Release\plugins\]] .. args[5]), true)
	File.copy(Path.combine(args[3], [[Plugins\HtcPlugin.Lua.MySQL\bin\Release\MySqlConnector.dll]]), Path.combine(args[3], [[HtcSharp.Server\bin\Release\plugins\MySqlConnector.dll]]), true)
end
print("[LuaBuildEvents] Finishing HtcPlugin.Lua.MySql PostBuild")