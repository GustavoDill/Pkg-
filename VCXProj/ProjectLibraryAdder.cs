using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VCXProjInterface.LibraryAdder
{
    public static class LibraryAdder
    {
        public static void RemoveLibraryFromFile(this string projectPath, Library lib, string libDir)
        {
            StreamReader reader = new StreamReader(File.OpenRead(projectPath));
            if (File.Exists(projectPath + "_mod.vcxproj"))
                File.Delete(projectPath + "_mod.vcxproj");
            StreamWriter writer = new StreamWriter(File.Create(projectPath + "_mod.vcxproj"));

            ProjectConfig current_config = new ProjectConfig();
            while (reader.EndOfStream == false)
            {
                string line = reader.ReadLine();

                var ret = GetConfig(line);
                if (ret.Valid)
                    current_config = ret;

                if (line.Contains("AdditionalIncludeDirectories"))
                {
                    // Changes to include directories

                    for (int i = 0; i < lib.Configurations.Length; i++)
                    {
                        if (lib.Configurations[i].Platform == current_config.Platform.ToString())
                        {
                            var dir = Regex.Match(line, "(\\s+)<AdditionalIncludeDirectories>(.+)</AdditionalIncludeDirectories>");
                            var add = new AdditionalIncludeDir(dir.Groups[2].Value);
                            foreach (var include in lib.Configurations[i].IncludePaths)
                                add.Directories.Remove(Path.Combine(libDir, include));
                            line = $"{dir.Groups[1].Value}<AdditionalIncludeDirectories>{add}</AdditionalIncludeDirectories>";

                        }
                    }
                }

                if (line.Contains("AdditionalLibraryDirectories"))
                {
                    // Changes to lib directories

                    for (int i = 0; i < lib.Configurations.Length; i++)
                    {
                        if (lib.Configurations[i].Platform == current_config.Platform.ToString())
                        {
                            var dir = Regex.Match(line, "(\\s+)<AdditionalLibraryDirectories>(.+)</AdditionalLibraryDirectories>");
                            var add = new AdditionalLibraryDir(dir.Groups[2].Value);
                            foreach (var libPath in lib.Configurations[i].LibPaths)
                                add.Directories.Remove(Path.Combine(libDir, libPath));
                            line = $"{dir.Groups[1].Value}<AdditionalLibraryDirectories>{add}</AdditionalLibraryDirectories>";

                        }
                    }
                }

                if (line.Contains("</ItemDefinitionGroup>")) // End of current config
                    current_config = new ProjectConfig();

                if (reader.EndOfStream)
                    writer.Write(line);
                else
                    writer.WriteLine(line);
            }
            reader.Close();
            writer.Close();
            File.Delete(projectPath);
            File.Move(projectPath + "_mod.vcxproj", projectPath);
        }
        public static void AddLibraryToFile(this string projectPath, Library lib, string libDir)
        {
            StreamReader reader = new StreamReader(File.OpenRead(projectPath));
            if (File.Exists(projectPath + "_mod.vcxproj"))
                File.Delete(projectPath + "_mod.vcxproj");
            StreamWriter writer = new StreamWriter(File.Create(projectPath + "_mod.vcxproj"));

            ProjectConfig current_config = new ProjectConfig();
            while (reader.EndOfStream == false)
            {
                string line = reader.ReadLine();
                
                var ret = GetConfig(line);
                if (ret.Valid)
                    current_config = ret;

                if (line.Contains("AdditionalIncludeDirectories"))
                {
                    // Changes to include directories

                    for (int i = 0; i < lib.Configurations.Length; i++)
                    {
                        if (lib.Configurations[i].Platform == current_config.Platform.ToString())
                        {
                            var dir = Regex.Match(line, "(\\s+)<AdditionalIncludeDirectories>(.+)</AdditionalIncludeDirectories>");
                            var add = new AdditionalIncludeDir(dir.Groups[2].Value);
                            foreach (var include in lib.Configurations[i].IncludePaths)
                                add.Directories.Insert(0, Path.Combine(libDir, include));
                            line = $"{dir.Groups[1].Value}<AdditionalIncludeDirectories>{add.ToString()}</AdditionalIncludeDirectories>";
                           
                        }
                    }
                }

                if (line.Contains("AdditionalLibraryDirectories"))
                {
                    // Changes to lib directories

                    for (int i = 0; i < lib.Configurations.Length; i++)
                    {
                        if (lib.Configurations[i].Platform == current_config.Platform.ToString())
                        {
                            var dir = Regex.Match(line, "(\\s+)<AdditionalLibraryDirectories>(.+)</AdditionalLibraryDirectories>");
                            var add = new AdditionalLibraryDir(dir.Groups[2].Value);
                            foreach (var libPath in lib.Configurations[i].LibPaths)
                                add.Directories.Insert(0, Path.Combine(libDir, libPath));
                            line = $"{dir.Groups[1].Value}<AdditionalLibraryDirectories>{add.ToString()}</AdditionalLibraryDirectories>";
                            
                        }
                    }
                }

                if (line.Contains("</ItemDefinitionGroup>")) // End of current config
                    current_config = new ProjectConfig();

                if (reader.EndOfStream)
                    writer.Write(line);
                else 
                    writer.WriteLine(line);
            }
            reader.Close();
            writer.Close();
            File.Delete(projectPath);
            File.Move(projectPath + "_mod.vcxproj", projectPath);
        }
        private static ProjectConfig GetConfig(string line)
        {
            var m = Regex.Match(line,
                @"<ItemDefinitionGroup Condition=""('\$\(Configuration\)\|\$\(Platform\)'=='\w+\|\w+')"">");
            if (m.Success)
            {
                return new ProjectConfig(m.Groups[1].Value);
            }
            else
                return new ProjectConfig()
                {

                };
        }
    }
}
