using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace VS
{
    public enum CompilationType
    {
        None,
        Debug,
        Release
    }
    public enum Platform
    {
        None,
        x64,
        Win32
    }
}
namespace VCXProjInterface
{


    public struct ProjectConfig
    {
        public ProjectConfig(string condition)
        {
            var match = Regex.Match(condition, "'\\$\\(Configuration\\)\\|\\$\\(Platform\\)'=='(\\w+)\\|(\\w+)'");
            if (match.Success == false)
                throw new Exception("Parse error");
            Configuration = (VS.CompilationType)Enum.Parse(typeof(VS.CompilationType), match.Groups[1].Value);
            Platform = (VS.Platform)Enum.Parse(typeof(VS.Platform), match.Groups[2].Value);
            Valid = true;
        }
        public ProjectConfig()
        {
            Configuration = VS.CompilationType.None;
            Platform = VS.Platform.None;
            Valid = false;
        }
        public bool Valid { get; set; }
        public VS.CompilationType Configuration { get; set; }
        public VS.Platform Platform { get; set; }
    }
    public partial class Project
    {
        public static Project CreateProject(
            ProjectConfig config,
            string[] includeFiles = null,
            params string[] compileFiles
            )
        {
            if (includeFiles == null) includeFiles = new string[0];
            Project project = new Project();
            project.ItemGroups = new ItemGroup[]
            {
                new ItemGroup()
                {
                    Label="ProjectConfigurations",
                    ProjectConfigurations = new ProjectConfiguration[]
                    {
                        new ProjectConfiguration()
                        {
                            Include = config.Configuration.ToString() + "|" + config.Platform.ToString(),
                            Configuration = config.Configuration.ToString(),
                            Platform = config.Platform.ToString()
                        }
                    }

                },
                new ItemGroup()
                {
                    ClIncludes = new ItemGroup.ClInclude[]
                    {
                        //new ItemGroup.ClInclude("App.h"),
                        //new ItemGroup.ClInclude("Helpers.h"),
                        //new ItemGroup.ClInclude("UBT_MODULE.h"),
                        //new ItemGroup.ClInclude("MainFrame.h")
                    }
                },
                new ItemGroup()
                {
                    ClCompiles = new ItemGroup.ClCompile[]
                    {
                        //new ItemGroup.ClCompile("App.cpp"),
                        //new ItemGroup.ClCompile("Logger.cpp"),
                        //new ItemGroup.ClCompile("MainFrame.cpp"),
                        //new ItemGroup.ClCompile("UBT_MODULE.cpp"),
                    }
                },


                new ItemGroup()
                {
                    _ProjectReference = new ItemGroup.ProjectReference()
                    {
                        Include = "..\\eIO\\eIO.vcxproj",
                        Project = "{42adc546-9a01-43e7-9d2e-d9e4e3b28046}"
                    }
                }
            };

            {
                List<ItemGroup.ClInclude> includes = new List<ItemGroup.ClInclude>();

                List<ItemGroup.ClCompile> compiles = new List<ItemGroup.ClCompile>();

                foreach (var include in includeFiles)
                {
                    includes.Add(new ItemGroup.ClInclude(include));
                }
                foreach (var compile in compileFiles)
                    compiles.Add(new ItemGroup.ClCompile(compile));
                project.ItemGroups[1].ClIncludes = includes.ToArray(); // Set include files
                project.ItemGroups[2].ClCompiles = compiles.ToArray(); // Set compilation files
            }



            project.PropertyGroups = new PropertyGroup[]
            {
                new PropertyGroup()
                {
                    Label = "Globals",
                    VCProjectVersion = "16.0",
                    Keyword = "Win32Proj",
                    ProjectGuid = "{028d45ec-aff3-4675-8595-7b9c7ec6b626}",
                    RootNamespace = "UniversalBinTool_Pro",
                    WindowsTargetPlatformVersion = "10.0",
                    UseDebugLibraries = null
                },
                new PropertyGroup()
                {
                    ConfigurationType = "Application",
                    UseDebugLibraries = "true",
                    PlatformToolset = "v143",
                    CharacterSet = "Unicode",
                    Label = "Configuration",
                    Condition = $"'$(Configuration)|$(Platform)'=='{config.Configuration.ToString()}|{config.Platform.ToString()}'"
                },
                new PropertyGroup()
                {
                    Label = "UserMacros"
                }

            };
            project.Imports = new Import[] {
                new Import() { Project = "$(VCTargetsPath)\\Microsoft.Cpp.Default.props" },
                new Import() { Project = "$(VCTargetsPath)\\Microsoft.Cpp.props" },
                new Import() { Project = "$(VCTargetsPath)\\Microsoft.Cpp.targets" },
            };

            project.ImportGroups = new ImportGroup[]
            {
                new ImportGroup("ExtensionSettings"),
                new ImportGroup("Shared"),
                new ImportGroup("PropertySheets")
                {   Condition = $"'$(Configuration)|$(Platform)'=='{config.Configuration.ToString()}|{config.Platform.ToString()}'", Import = new Import()
                    {
                        Project = "$(UserRootDir)\\Microsoft.Cpp.$(Platform).user.props",
                        Condition="exists('$(UserRootDir)\\Microsoft.Cpp.$(Platform).user.props')",
                        Label = "LocalAppDataPlatform"
                    }
                },
                new ImportGroup("ExtensionTargets")
            };

            project.ItemDefinitionGroups = new ItemDefinitionGroup[]
            {
                new ItemDefinitionGroup()
                {
                    Condition = $"'$(Configuration)|$(Platform)'=='{config.Configuration.ToString()}|{config.Platform.ToString()}'",
                    ClCompile = new ItemDefinitionGroup.__ClCompile()
                    {
                        WarningLevel = "Level3",
                        SDLCheck = "true",
                        PreprocessorDefinitions = "WIN32;_DEBUG;_CONSOLE;%(PreprocessorDefinitions)",
                        ConformanceMode = true,
                        AdditionalIncludeDirectories = "$(WXWIN)\\include\\msvc;$(WXWIN)\\include;%(AdditionalIncludeDirectories)",
                        LanguageStandard = "stdcpp17"
                    },
                    _Link = new ItemDefinitionGroup.__Link()
                    {
                        SubSystem = "Windows",
                        GenerateDebugInformation = true,
                        AdditionalLibraryDirectories = "$(WXWIN)\\lib\\vc_lib;%(AdditionalLibraryDirectories)",
                        AdditionalDependencies = "%(AdditionalDependencies)",
                        UACExecutionLevel = "RequireAdministrator"
                    },
                    _PreLinkEvent = new ItemDefinitionGroup.PreLinkEvent()
                    {
                        Command = "echo Copying depency images....\r\ncopy \"$(ProjectDir)folder.ico\" \"$(OutDir)folder.ico\"\r\ncopy \"$(ProjectDir)folder.ico\" \"$(SolutionDir)x64\\$(ConfigurationName)\\folder.ico\"\r\ncopy \"$(ProjectDir)Gears.ico\" \"$(OutDir)Gears.ico\"\r\ncopy \"$(ProjectDir)Gears.ico\" \"$(SolutionDir)x64\\$(ConfigurationName)\\Gears.ico\"\r\necho Done!"
                    }
                },
            };


            return project;
        }
    }
}
