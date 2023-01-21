using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace VCXProjInterface
{
    [Serializable]
    [XmlRoot]
    public partial class Project
    {
        [XmlIgnore]
        public Solution ParentSolution { get; set; }
        [XmlIgnore]
        public string ProjectPath { get; set; }
        [XmlIgnore]
        public string ProjectGUID { get; set; }
        [XmlIgnore]
        public string ProjectName { get; set; }
        [XmlIgnore]
        public string LibDefFile { get; set; }

        [XmlIgnore]
        public LibraryCollection InstalledLibraries { get; set; }
        public static Project Deserialize(string path, string projName = "", string projGuid= "", string libDefFile = "libs.xml")
        {

            if (File.Exists(path + "_new.vcxproj"))
                File.Delete(path + "_new.vcxproj");
            var newFile = File.Open(path + "_new.vcxproj", FileMode.Create, FileAccess.ReadWrite);

            string[] lines = (string[])File.ReadLines(path).ToArray();

            {
                StreamWriter writer = new StreamWriter(newFile);
                foreach (var line in lines)
                {
                    if (line.Contains("xmlns"))
                        writer.WriteLine(Regex.Replace(line, " xmlns=\"[\\w\\:\\.\\/\\d]+\"", ""));
                    else
                        writer.WriteLine(line);
                }
                writer.Close();
            }

            newFile = File.Open(path + "_new.vcxproj", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            StreamReader reader = new StreamReader(newFile);


            XmlSerializer serializer = new XmlSerializer(typeof(Project));
            //StreamReader reader = new StreamReader(File.OpenRead(path));
            var proj = (Project)serializer.Deserialize(reader);
            for (int i = 0; i < proj.ItemDefinitionGroups.Length; i++)
            {
                proj.ItemDefinitionGroups[i]._clCompile.AdditionalIncludes = new AdditionalIncludeDir(proj.ItemDefinitionGroups[i]._clCompile.AdditionalIncludeDirectories);
                proj.ItemDefinitionGroups[i]._link.AdditionalLibs = new AdditionalLibraryDir(proj.ItemDefinitionGroups[i]._link.AdditionalLibraryDirectories);
            }
            proj.ProjectName = projName;
            proj.ProjectGUID = projGuid;
            proj.ProjectPath = path;
            proj.LibDefFile = Path.Combine(new FileInfo(proj.ProjectPath).DirectoryName, libDefFile);

            if (!File.Exists(proj.LibDefFile))
            {
                proj.InstalledLibraries = new LibraryCollection() { Libraries = new Library[0] };
                proj.InstalledLibraries.Save(proj.LibDefFile);
            }
            else
                proj.InstalledLibraries = LibraryCollection.Load(proj.LibDefFile);

            reader.Close();
            if (proj == null)
                return null;

            reader.Close();
            newFile.Close();
            File.Delete(path + "_new.vcxproj");
            return proj;
        }
        public Project()
        {
            PropertyGroups = new PropertyGroup[0];
            ImportGroups = new ImportGroup[0];
            ItemDefinitionGroups = new ItemDefinitionGroup[0];
            ItemGroups = new ItemGroup[0];
        }
        [XmlAttribute]
        public string DefaultTargets { get; set; }

        //[XmlAttribute]
        //public string xmlns { get; set; }
        void Serialize(string xmlPath)
        {
            for (int i  =0; i < ItemDefinitionGroups.Length; i++)
            {
                ItemDefinitionGroups[i]._link.AdditionalLibraryDirectories = ItemDefinitionGroups[i]._Link.AdditionalLibs.ToString();
                ItemDefinitionGroups[i]._clCompile.AdditionalIncludeDirectories = ItemDefinitionGroups[i].ClCompile.AdditionalIncludes.ToString();
            }


            XmlSerializer serializer = new XmlSerializer(typeof(Project));
            StreamWriter writer = new StreamWriter(xmlPath);
            serializer.Serialize(writer, this);
            writer.Close();
        }
        void MakeProjectVSCompatible(string path)
        {
            var lines = (string[])File.ReadLines(path).ToArray();
            lines[1] = "<Project DefaultTargets=\"Build\" xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">";
            File.WriteAllLines(path, lines);
        }
        public void SaveProject(string path)
        {
            Serialize(path);
            MakeProjectVSCompatible(path);
        }


        [XmlElement("ItemGroup")]
        public ItemGroup[] ItemGroups { get; set; }

        [XmlElement("PropertyGroup")]
        public PropertyGroup[] PropertyGroups { get; set; }

        [XmlElement("Import")]
        public Import[] Imports { get; set; }

        [XmlElement("ImportGroup")]
        public ImportGroup[] ImportGroups { get; set; }


        [XmlElement("ItemDefinitionGroup")]
        public ItemDefinitionGroup[] ItemDefinitionGroups { get; set; }
    }
    public struct ItemDefinitionGroup
    {
        public ItemDefinitionGroup()
        {
            _clCompile = new __ClCompile();
            _link = new __Link();
            _PreLinkEvent = new PreLinkEvent();
            Condition = null;
        }
        public struct PreLinkEvent
        {
            public PreLinkEvent() { Command = null; }
            [XmlElement("Command")]
            public string Command { get; set; }
        }
        public struct __ClCompile
        {
            public __ClCompile()
            {
                WarningLevel = null;
                SDLCheck = null;
                PreprocessorDefinitions = "%(PreprocessorDefinitions)";
                ConformanceMode = false;
                AdditionalIncludeDirectories = "%(AdditionalIncludeDirectories)";
                AdditionalIncludes = new AdditionalIncludeDir(AdditionalIncludeDirectories);
                LanguageStandard = null;
            }
            public string WarningLevel { get; set; }
            public string SDLCheck { get; set; }
            public string PreprocessorDefinitions { get; set; }
            public bool ConformanceMode { get; set; }
            public string AdditionalIncludeDirectories { get; set; }
            [XmlIgnore]
            public AdditionalIncludeDir AdditionalIncludes { get; set; }
            public string LanguageStandard { get; set; }
        }
        public struct __Link
        {
            public __Link()
            {
                SubSystem = null;
                GenerateDebugInformation = false;
                AdditionalLibraryDirectories = "%(AdditionalLibraryDirectories)";
                AdditionalLibs = new AdditionalLibraryDir(AdditionalLibraryDirectories);
                AdditionalDependencies = null;
                UACExecutionLevel = null;
            }
            public string SubSystem { get; set; }
            public bool GenerateDebugInformation { get; set; }
            public string AdditionalLibraryDirectories { get; set; }
            [XmlIgnore]
            public AdditionalLibraryDir AdditionalLibs { get; set; }
            public string AdditionalDependencies { get; set; }
            public string UACExecutionLevel { get; set; }
        }
        [XmlIgnore]
        public __ClCompile _clCompile;
        [XmlIgnore]
        public __Link _link;

        [XmlAttribute]
        public string Condition { get; set; }
        public __ClCompile ClCompile { get => _clCompile; set => _clCompile = value; }

        [XmlElement("Link")]
        public __Link _Link { get => _link; set => _link = value; }

        [XmlElement("PreLinkEvent")]
        public PreLinkEvent _PreLinkEvent { get; set; }
    }
    public struct ImportGroup
    {
        public ImportGroup() : this(null) { }
        public ImportGroup(string label)
        {
            Label = label;
            Import = null;
            Condition = null;
        }
        [XmlAttribute]
        public string Label { get; set; }
        [XmlAttribute]
        public string Condition { get; set; }
        public Import Import { get; set; }
    }
    public class Import
    {
        [XmlAttribute]
        public string Project { get; set; }
        [XmlAttribute]
        public string Condition { get; set; }
        [XmlAttribute]
        public string Label { get; set; }
    }
    [Serializable]
    public struct ItemGroup
    {
        public ItemGroup()
        {
            Label = null;
            ProjectConfigurations = null;
            ClIncludes = null;
            ClCompiles = null;
            _ProjectReference = null;
        }


        [XmlElement("ClInclude")]
        public ClInclude[] ClIncludes { get; set; }

        [XmlElement("ClCompile")]
        public ClCompile[] ClCompiles { get; set; }

        [XmlAttribute]
        public string Label { get; set; }

        [XmlElement("ProjectConfiguration")]
        public ProjectConfiguration[] ProjectConfigurations { get; set; }

        [XmlElement("ProjectReference")]
        public ProjectReference _ProjectReference { get; set; }


        public class ProjectReference
        {
            [XmlAttribute]
            public string Include { get; set; }
            [XmlElement("Project")]
            public string Project { get; set; }
        }
        public struct ClInclude
        {
            public ClInclude() : this(null) { }
            public ClInclude(string include)
            {
                Include = include;
            }

            [XmlAttribute]
            public string Include { get; set; }
        }
        public struct ClCompile
        {
            public ClCompile() : this(null) { }
            public ClCompile(string include)
            {
                Include = include;
            }
            [XmlAttribute]
            public string Include { get; set; }
        }
    }
    [Serializable]
    public struct ProjectConfiguration
    {
        public ProjectConfiguration()
        {
            Include = "";
            Configuration = "";
            Platform = "";
        }
        [XmlAttribute]
        public string Include { get; set; }
        public string Configuration { get; set; }
        public string Platform { get; set; }

    }


    [Serializable]
    public struct PropertyGroup
    {
        public PropertyGroup()
        {
            Label = null;
            VCProjectVersion = null;
            Keyword = null;
            ProjectGuid = null;
            RootNamespace = null;
            WindowsTargetPlatformVersion = null;
            ConfigurationType = null;
            UseDebugLibraries = null;
            PlatformToolset = null;
            CharacterSet = null;
            Condition = null;
        }
        [XmlAttribute]
        public string Condition { get; set; }
        [XmlAttribute]
        public string Label { get; set; }
        public string VCProjectVersion { get; set; }
        public string Keyword { get; set; }
        public string ProjectGuid { get; set; }
        public string RootNamespace { get; set; }
        public string WindowsTargetPlatformVersion { get; set; }
        public string ConfigurationType { get; set; }
        public string UseDebugLibraries { get; set; }
        public string PlatformToolset { get; set; }
        public string CharacterSet { get; set; }
    }


    public class AdditionalLibraryDir : DirectoryList
    {
        public AdditionalLibraryDir() : base() { }
        public AdditionalLibraryDir(string parseString) : base(parseString) { }
    }
    public class AdditionalIncludeDir : DirectoryList
    {
        public AdditionalIncludeDir() : base() { }
        public AdditionalIncludeDir(string parseString) : base(parseString) { }
    }
    public abstract class DirectoryList
    {
        public DirectoryList()
        {
            Directories = new List<string>();
        }
        public DirectoryList(string parseString) : this()
        {
            if (parseString == null) return;
            foreach (var dir in parseString.Split(';'))
            {
                Directories.Add(dir);
            }
        }
        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            if (Directories.Count == 0) return "";
            foreach (var dir in Directories)
            {
                str.Append(";" + dir);
            }
            return str.ToString().Substring(1);
        }
        public List<string> Directories { get; set; }
    }
}
