using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCXProjInterface
{
    public partial class Project
    {
        public void AddInclude(string includePath)
        {
            for (int i = 0; i < ItemDefinitionGroups.Length; i++)
            {
                ItemDefinitionGroups[i]._clCompile.AdditionalIncludes.Directories.Insert(0, includePath);
                ItemDefinitionGroups[i]._clCompile.AdditionalIncludeDirectories = ItemDefinitionGroups[i]._clCompile.AdditionalIncludes.ToString();
            }
        }
        public void AddLibrary(string libraryPath)
        {
            for (int i = 0; i < ItemDefinitionGroups.Length; i++)
            {
                ItemDefinitionGroups[i]._link.AdditionalLibs.Directories.Insert(0, libraryPath);
                ItemDefinitionGroups[i]._link.AdditionalLibraryDirectories = ItemDefinitionGroups[i]._link.AdditionalLibs.ToString();
            }
        }

        public void AddInclude(string includePath, ProjectConfig configuration)
        {
            for (int i = 0; i < ItemDefinitionGroups.Length; i++)
            {
                var config = new ProjectConfig(ItemDefinitionGroups[i].Condition);
                if (config.Platform == configuration.Platform &&
                    config.Configuration == configuration.Configuration)
                {
                    ItemDefinitionGroups[i]._clCompile.AdditionalIncludes.Directories.Insert(0, includePath);
                    ItemDefinitionGroups[i]._clCompile.AdditionalIncludeDirectories = ItemDefinitionGroups[i]._clCompile.AdditionalIncludes.ToString();
                }
            }
        }



        public void AddLibrary(string libraryPath, ProjectConfig configuration)
        {
            for (int i = 0; i < ItemDefinitionGroups.Length; i++)
            {
                var config = new ProjectConfig(ItemDefinitionGroups[i].Condition);
                if (config.Platform == configuration.Platform &&
                    config.Configuration == configuration.Configuration)
                {
                    ItemDefinitionGroups[i]._link.AdditionalLibs.Directories.Insert(0, libraryPath);
                    ItemDefinitionGroups[i]._link.AdditionalLibraryDirectories = ItemDefinitionGroups[i]._link.AdditionalLibs.ToString();
                }
            }
        }

        public void RemoveInclude(string includePath, ProjectConfig configuration)
        {
            for (int i = 0; i < ItemDefinitionGroups.Length; i++)
            {
                var config = new ProjectConfig(ItemDefinitionGroups[i].Condition);
                if (config.Platform == configuration.Platform &&
                    config.Configuration == configuration.Configuration)
                {

                    ItemDefinitionGroups[i]._clCompile.AdditionalIncludes.Directories.Remove(includePath);
                    //ItemDefinitionGroups[i]._clCompile.AdditionalIncludes.Directories.Insert(0, includePath);
                    ItemDefinitionGroups[i]._clCompile.AdditionalIncludeDirectories = ItemDefinitionGroups[i]._clCompile.AdditionalIncludes.ToString();
                }
            }
        }
        public void RemoveInclude(string includePath)
        {
            for (int i = 0; i < ItemDefinitionGroups.Length; i++)
            {
                ItemDefinitionGroups[i]._clCompile.AdditionalIncludes.Directories.Remove(includePath);
                ItemDefinitionGroups[i]._clCompile.AdditionalIncludeDirectories = ItemDefinitionGroups[i]._clCompile.AdditionalIncludes.ToString();
            }
        }
        public void RemoveLibrary(string libraryPath, ProjectConfig configuration)
        {
            for (int i = 0; i < ItemDefinitionGroups.Length; i++)
            {
                var config = new ProjectConfig(ItemDefinitionGroups[i].Condition);
                if (config.Platform == configuration.Platform &&
                    config.Configuration == configuration.Configuration)
                {
                    ItemDefinitionGroups[i]._link.AdditionalLibs.Directories.Remove(libraryPath);
                    ItemDefinitionGroups[i]._link.AdditionalLibraryDirectories = ItemDefinitionGroups[i]._link.AdditionalLibs.ToString();
                }
            }
        }
        public void RemoveLibrary(string libraryPath)
        {
            for (int i = 0; i < ItemDefinitionGroups.Length; i++)
            {
                ItemDefinitionGroups[i]._link.AdditionalLibs.Directories.Remove(libraryPath);
                ItemDefinitionGroups[i]._link.AdditionalLibraryDirectories = ItemDefinitionGroups[i]._link.AdditionalLibs.ToString();
            }
        }




        public void AddLibraryDef(Library lib)
        {
            foreach (var config in lib.Configurations)
            {

                foreach (var libPath in config.LibPaths)

                {
                    AddLibrary(Path.Combine( // e.g. "depedencies\\wxWidgets-3.2.1\\lib\\vc_lib"
                    "dependencies\\" + lib.Name + "-" + lib.Version,
                    libPath), config.GetConfig(VS.CompilationType.Debug));

                    AddLibrary(Path.Combine( // e.g. "depedencies\\wxWidgets-3.2.1\\lib\\vc_lib"
                    "dependencies\\" + lib.Name + "-" + lib.Version,
                    libPath), config.GetConfig(VS.CompilationType.Release));
                }
                foreach (var includePath in config.IncludePaths)
                {
                    // Add lib to debug
                    AddInclude(Path.Combine(
                    "dependencies\\" + lib.Name + "-" + lib.Version,
                    includePath), config.GetConfig(VS.CompilationType.Debug));


                    // Add lib to release
                    AddInclude(Path.Combine(
                        "dependencies\\" + lib.Name + "-" + lib.Version,
                        includePath), config.GetConfig(VS.CompilationType.Release));
                }


            }
        }
        public void RemoveLibraryDef(Library lib)
        {
            foreach (var config in lib.Configurations)
            {

                foreach (var libPath in config.LibPaths)

                {
                    RemoveLibrary(Path.Combine( // e.g. "depedencies\\wxWidgets-3.2.1\\lib\\vc_lib"
                    "dependencies\\" + lib.Name + "-" + lib.Version,
                    libPath), config.GetConfig(VS.CompilationType.Debug));

                    RemoveLibrary(Path.Combine( // e.g. "depedencies\\wxWidgets-3.2.1\\lib\\vc_lib"
                    "dependencies\\" + lib.Name + "-" + lib.Version,
                    libPath), config.GetConfig(VS.CompilationType.Release));
                }
                foreach (var includePath in config.IncludePaths)
                {
                    // Add lib to debug
                    RemoveLibrary(Path.Combine(
                    "dependencies\\" + lib.Name + "-" + lib.Version,
                    includePath), config.GetConfig(VS.CompilationType.Debug));


                    // Add lib to release
                    RemoveLibrary(Path.Combine(
                        "dependencies\\" + lib.Name + "-" + lib.Version,
                        includePath), config.GetConfig(VS.CompilationType.Release));
                }


                // Remove lib from debug
                //RemoveLibrary(Path.Combine( // e.g. "depedencies\\wxWidgets-3.2.1\\lib\\vc_lib"
                //    "dependencies\\" + lib.Name + "-" + lib.Version,
                //    config.LibPath), config.GetConfig(VS.CompilationType.Debug));

                //RemoveInclude(Path.Combine(
                //    "dependencies\\" + lib.Name + "-" + lib.Version,
                //    config.IncludePath), config.GetConfig(VS.CompilationType.Debug));

                //// Remove lib from release
                //RemoveLibrary(Path.Combine( // e.g. "depedencies\\wxWidgets-3.2.1\\lib\\vc_lib"
                //    "dependencies\\" + lib.Name + "-" + lib.Version,
                //    config.LibPath), config.GetConfig(VS.CompilationType.Release));
                //RemoveInclude(Path.Combine(
                //    "dependencies\\" + lib.Name + "-" + lib.Version,
                //    config.IncludePath), config.GetConfig(VS.CompilationType.Release));
            }
        }
    }
}
