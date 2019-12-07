local directory = require("lua.io.directory")
local file = require("lua.io.file")
local path = require("lua.io.path")

if args[2] == "Debug" then
    pluginsPath = path.combine(args[3], [[HtcSharp.Server\bin\Debug\plugins\]])
    if directory.exists(pluginsPath) == false then
	    directory.create(pluginsPath)
	end
	file.copy(args[4], path.combine(args[3], [[HtcSharp.Server\bin\Debug\plugins\]] .. args[5]), true)
elseif args[2] == "Release" then
    pluginsPath = path.combine(args[3], [[HtcSharp.Server\bin\Release\plugins\]])
    if directory.exists(pluginsPath) == false then
	    directory.create(pluginsPath)
	end
	file.copy(args[4], path.combine(args[3], [[HtcSharp.Server\bin\Release\plugins\]] .. args[5]), true)
end
