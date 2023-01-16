using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VCXProjInterface
{
    public class Solution
    {

        public Solution(string path)
        {
            List<Project> projects = new List<Project>() ;
            StreamReader reader = new StreamReader(path);
            var solDir = new FileInfo(path).DirectoryName;
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (line.StartsWith("Project"))
                {
                    projects.Add(LoadProject(line, solDir));
                }
            }
            Projects = projects.ToArray();
        }

        internal string Data()
        {

            StringBuilder sb = new StringBuilder() ;
            sb.AppendLine($"SolutionGUID: {SolutionGUID}");
            sb.AppendLine($"SolutionDir: {SolutionDir}");
            return sb.ToString();
        }
        Project LoadProject(string data, string solutionDir)
        {
            var result = Regex.Match(data, "Project\\(\"{([\\w\\-]+)}\"\\) = \"([\\w ]+)\", \"([\\w \\\\\\.]+)\", \"{([\\w\\-]+)}\"");

            if (result.Success == false)
                throw new Exception("Loading error");
            var ProjectName = result.Groups[2].Value;
            var ProjectPath = Path.Combine(solutionDir, result.Groups[3].Value);
            var ProjectGUID = result.Groups[4].Value;

            return Project.Deserialize(ProjectPath, ProjectName, ProjectGUID);

            //if (!result.Success) throw new Exception("Invalid data");
            //SolutionGUID = result.Groups[1].Value;
            //SolutionDir = solutionDir;
        }
        public Solution() { }
        public Project[] Projects { get; set; }
        public string SolutionGUID { get; }
        public string SolutionDir { get; }
    }

}
