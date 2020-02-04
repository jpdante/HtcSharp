print("[LuaBuildEvents] Starting HtcSharp.HttpModule PostBuild")
require("lua.io")

if args[2] == "Debug" then
    print("[LuaBuildEvents] Running in Debug Mode")
    pluginsPath = Path.combine(args[3], [[HtcSharp.Server\bin\Debug\plugins\]])
    if Directory.exists(pluginsPath) == false then
	    Directory.createDirectory(pluginsPath)
	end
	File.copy(args[4], Path.combine(args[3], [[HtcSharp.Server\bin\Debug\plugins\]] .. args[5]), true)
elseif args[2] == "Release" then
    print("[LuaBuildEvents] Running in Release Mode")
    pluginsPath = Path.combine(args[3], [[HtcSharp.Server\bin\Release\plugins\]])
    if Directory.exists(pluginsPath) == false then
	    Directory.createDirectory(pluginsPath)
	end
	File.copy(args[4], Path.combine(args[3], [[HtcSharp.Server\bin\Release\plugins\]] .. args[5]), true)
end
print("[LuaBuildEvents] Finishing HtcSharp.HttpModule PostBuild")