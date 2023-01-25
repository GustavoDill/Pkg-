using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace VCXProjInterface
{

    public class Library
    {

        public bool EqualsTo(Library selectedLib)
        {
            return (Name == selectedLib.Name &&
                Version == selectedLib.Version &&
                LibraryPath == selectedLib.LibraryPath);
        }


        public struct LibConfig
        {

            public ProjectConfig GetConfig(VS.CompilationType configuration)
            {
                return new ProjectConfig
                {
                    Configuration = configuration,
                    Platform = (VS.Platform)Enum.Parse(typeof(VS.Platform), Platform)
                };

            }
            [XmlAttribute]
            public string Platform { get; set; }

            [XmlElement("LibPath")]
            public string[] LibPaths { get; set; }
            [XmlElement("IncludePath")]
            public string[] IncludePaths { get; set; }
        }
        public Library()
        {
            Name = "LIBRARY_NAME";
            Configurations = new LibConfig[]
            {
                new LibConfig()
                {
                    Platform = "Win32",
                    LibPaths = new string[]{"."},
                    IncludePaths = new string[]{"."}
                }
            };
            LibraryPath = ".";
            Version = "1.0";

        }
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string Version { get; set; }
        public string LibraryPath { get; set; }

        [XmlElement("Configuration")]
        public LibConfig[] Configurations { get; set; }

    }
}
