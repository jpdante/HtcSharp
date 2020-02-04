require("lua.io")

if args[2] == "Debug" then
    pluginsPath = Path.combine(args[3], [[HtcSharp.Server\bin\Debug\plugins\]])
    if Directory.exists(pluginsPath) == false then
	    Directory.createDirectory(pluginsPath)
	end
	File.copy(args[4], Path.combine(args[3], [[HtcSharp.Server\bin\Debug\plugins\]] .. args[5]), true)
	File.copy(Path.combine(args[3], [[Plugins\HtcPlugin.Lua.MySQL\bin\Debug\MySqlConnector.dll]]), Path.combine(args[3], [[HtcSharp.Server\bin\Debug\plugins\MySqlConnector.dll]]), true)
elseif args[2] == "Release" then
    pluginsPath = Path.combine(args[3], [[HtcSharp.Server\bin\Release\plugins\]])
    if Directory.exists(pluginsPath) == false then
	    Directory.createDirectory(pluginsPath)
	end
	File.copy(args[4], Path.combine(args[3], [[HtcSharp.Server\bin\Release\plugins\]] .. args[5]), true)
	File.copy(Path.combine(args[3], [[Plugins\HtcPlugin.Lua.MySQL\bin\Release\MySqlConnector.dll]]), Path.combine(args[3], [[HtcSharp.Server\bin\Release\plugins\MySqlConnector.dll]]), true)
end