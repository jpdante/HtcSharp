print("[LuaBuildEvents] Starting HtcSharp.HttpModule PostBuild\n")
require("lua.io")

-- Filename and directory blacklist
function isBlackListed(path)
  if path == [[HtcSharp.Core.dll]] then return true
  elseif path == [[HtcSharp.Core.pdb]] then return true
  elseif path == [[HtcSharp.Logging.dll]] then return true
  elseif path == [[HtcSharp.Logging.pdb]] then return true
  elseif path == [[HtcSharp.Abstractions.dll]] then return true
  elseif path == [[HtcSharp.Abstractions.pdb]] then return true
  elseif path == [[HtcSharp.Shared.dll]] then return true
  elseif path == [[HtcSharp.Shared.pdb]] then return true
  elseif path == [[ref]] then return true
  else return false
  end
end

print("[LuaBuildEvents] Running in " .. args[2] .. " Mode\n")

-- Create base directory
pluginsPath = Path.combine(args[3], [[HtcSharp/bin/]] .. args[6] .. "/" .. args[2] .. [[/modules/http-module]])
if Directory.exists(pluginsPath) == false then
  Directory.createDirectory(pluginsPath)
end

-- Create directories
outputDirectory = Path.combine(args[4], args[5])
outputDirectories = Directory.getDirectories(outputDirectory, "*", SearchOption.AllDirectories)
for key,value in ipairs(outputDirectories) do
  directoryReplace = value:gsub(outputDirectory, "")
  if isBlackListed(directoryReplace) == true then
    print("Blacklisted directory: " .. value .. "\n")
    goto continue
  end
  fixedDirectory = Path.combine(pluginsPath, directoryReplace)
  if Directory.exists(fixedDirectory) == false then
    print("Creating directory: " .. fixedDirectory .. "\n")
    Directory.createDirectory(fixedDirectory)
  end

  -- Copy files
  outputFiles = Directory.getFiles(value, "*.*", SearchOption.TopDirectoryOnly)
  for key2,value2 in ipairs(outputFiles) do
    fileNameReplace = value2:gsub(outputDirectory, "")
    if isBlackListed(Path.getFileName(fileNameReplace)) == true then
      print("Blacklisted file: " .. Path.getFileName(fileNameReplace) .. "\n")
      goto continue2
    end
    fixedFileName = Path.combine(pluginsPath, fileNameReplace)
    print("Copying file: " .. fixedFileName .. "\n")
    File.copy(value2, fixedFileName, true)
    ::continue2::
  end

  ::continue::
end

-- Copy files
outputFiles = Directory.getFiles(outputDirectory, "*.*", SearchOption.TopDirectoryOnly)
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


print("[LuaBuildEvents] Finishing HtcSharp.HttpModule PostBuild\n")