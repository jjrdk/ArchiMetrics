using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ArchiCop.Core
{
    public class BaseTest
    {
        protected static IEnumerable<VisualStudioProject> GetSampleProjects()
        {
            IVisualStudioProjectRepository projectRepository = new VisualStudioProjectRepository();
            string rootDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var projects = new List<string>(Directory.GetFiles(rootDirectory, "*.csproj", SearchOption.AllDirectories));

            return projectRepository.GetProjects(projects);
        }

        protected static VisualStudioProject GetSampleProject()
        {            
            return GetSampleProjects().First(item => item.ProjectName == "ArchiCop");
        }

        //private Dictionary<string, string> GetPropertiesWithValues(object obj)
        //{
        //    return obj.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(obj, null) as string);
        //}

        //private void Serialize<T>(string filename,T obj)
        //{
        //    var serializer = new XmlSerializer(typeof(T));
        //    TextWriter writer = new StreamWriter(filename);

        //    serializer.Serialize(writer, obj);
        //    writer.Close();
        //}

        //private T Deserialize<T>(string filename)
        //{
        //    var serializer = new XmlSerializer(typeof(T));
        //    var fs = new FileStream(filename, FileMode.Open);
        //    var t = (T)serializer.Deserialize(fs);
        //    fs.Close();
        //    return t;
        //}
    }
}