using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.IO;
using CSharpExtendedCommands.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CSharpExtendedCommands.DataTypeExtensions;

namespace Pkg__
{
    public class PathAttribute : Attribute
    {
        public PathAttribute(string path)
        {
            Path = path;
        }

        public string Path { get; }
    }
    public static class Manager
    {
        public static void LoadResources()
        {
            var res = CSharpExtendedCommands.Data.Resource.GetEmbeddedTextFileContent("Pkg__.res.ResourceDefinitions.txt").Replace("\r", "");
            var split = res.Split('\n');
            Resources = new Dictionary<string, ResourceDef>(split.Length);
            foreach (var line in split)
            {
                var r = ParseResource(line);
                Resources.Add(r.Name, r);
            }
        }
        public static ResourceDef ParseResource(string line)
        {
            var split = line.Split(' ');
            var name = split[1];
            var path = split[2];
            var res = GetResource(path, split[0]);
            return new ResourceDef(name, res);
        }
        enum RegisteredTypes
        {
            System_Drawing_Image,
            System_String,
            System_Drawing_Icon
        }
        public static object GetResource(string path, string type)
        {
            object value;
            switch ((RegisteredTypes)Enum.Parse(typeof(RegisteredTypes), type.Replace(".", "_")))
            {
                case RegisteredTypes.System_Drawing_Image:
                    value = Resource.LoadResource(path, (m) => { return System.Drawing.Image.FromStream(m); });
                    break;

                case RegisteredTypes.System_String:
                    value = Resource.LoadResource(path, (m) => { var r = new BinaryReader(m); return Encoding.ASCII.GetString(r.ReadBytes((int)r.BaseStream.Length)); });
                    break;

                case RegisteredTypes.System_Drawing_Icon:
                    value = Resource.LoadResource(path, (m) => { return new System.Drawing.Icon(m); });
                    break;

                default: value = null; break;
            }
            return value;
        }

        //static Dictionary<string, Type> RegisteredTypes { get; set; }
        public static Dictionary<string, ResourceDef> Resources { get; set; }
    }

    public class ResourceDef
    {
        public ResourceDef()
        {

        }
        public ResourceDef(string name, object value)
        {
            Name = name;
            Value = value;
        }
        public T Get<T>()
        {
            return (T)Value;
        }
        //public MemoryStream GetStream()
        //{
        //    return new MemoryStream(Data);
        //}
        public object Value { get; }
        public string Name { get; }

    }
}
