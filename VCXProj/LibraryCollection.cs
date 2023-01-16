using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace VCXProjInterface
{
    public class LibraryCollection
    {
        [XmlElement("Library")]
        public Library[] Libraries { get; set; }
        public int Length { get => Libraries.Length; }
        public Library this[int index]
        {
            get
            {
                return Libraries[index];
            }
            set
            {
                Libraries[index] = value;
            }
        }
        public static void CreateTemplate()
        {
            LibraryCollection col = new LibraryCollection();
            col.Libraries = new Library[1]
            {
                new Library()
            };
            col.Save("libs.xml");
        }

        public static LibraryCollection Load(string libraryDefPath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(LibraryCollection));
            if (File.Exists(libraryDefPath) == false)
                return null;
            var file = (File.OpenRead(libraryDefPath));
            var collection = (LibraryCollection)serializer.Deserialize(file);
            file.Close();
            if (collection.Libraries == null) collection.Libraries = new Library[0];
            return collection;
        }

        public void Save(string libraryDefPath)
        {
            XmlSerializer xml = new XmlSerializer(typeof(LibraryCollection));
            FileStream file;
            if (!File.Exists(libraryDefPath))
                file = File.Open(libraryDefPath, FileMode.Create, FileAccess.Write);
            else
                file = File.Open(libraryDefPath, FileMode.Truncate, FileAccess.Write);


            xml.Serialize(file, this);
            file.Close();
        }
    }
}
