using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pkg__
{
    public static class Settings
    {
        [Setting]
        public static bool CopyLibraries { get; set; }

        public static void Save(string filename)
        {
            if (properties.Count == 0) throw new Exception("Load settings first");
            List<string> lines = new List<string>(properties.Count);
            foreach (var property in properties)
            {
                lines.Add(property.Key.ToString()+":"+property.Value.GetValue(null).ToString());
            }
            File.WriteAllLines(filename, lines.ToArray());
        }
        private static Dictionary<string, PropertyInfo> properties;
        private static void get_property_names()
        {
            var properties = typeof(Settings).GetProperties();
            Settings.properties = new Dictionary<string, PropertyInfo>(properties.Length);
            foreach (var property in properties)
            {
                if (HasAttribyte(property.CustomAttributes.ToArray(), typeof(SettingAttribute)))
                {
                    Settings.properties.Add(property.Name.ToLower(), property);
                }
            }

        }
        private static bool HasAttribyte(CustomAttributeData[] attributes, Type attribute)
        {
            foreach (var att in attributes)
            {
                if (att.AttributeType == attribute)
                    return true;
            }
            return false;
        }
        public static void Load(string filename)
        {
            if (properties == null) get_property_names();
            else if (properties.Count == 0) get_property_names();

            var lines = File.ReadAllLines(filename);
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line))
                    continue;

                if (line.Contains(":") == false)
                    continue;

                var s = line.Split(':');
                if (s.Length > 2)
                    continue;

                if (properties.ContainsKey(s[0]))
                {
                    properties[s[0]].SetValue(null, Convert.ChangeType(s[1], properties[s[0]].PropertyType));
                }
            }
        }

        private static Dictionary<string, object> get_default_values()
        {
            var dic = new Dictionary<string, object>()
            {
                { "CopyLibraries", true }
            };
            var newD = new Dictionary<string, object>();
            foreach (var kv in dic)
            {
                newD.Add(kv.Key.ToString().ToLower(), kv.Value);
            }
            dic.Clear();
            return newD;
        }

        public static void CreateDefault(string filename)
        {
            get_property_names();
            var defs = get_default_values();
            foreach (var prop in properties)
            {
                prop.Value.SetValue(null, Convert.ChangeType(defs[prop.Key].ToString(), prop.Value.PropertyType));
            }

            Save(filename);

        }
    }
}
