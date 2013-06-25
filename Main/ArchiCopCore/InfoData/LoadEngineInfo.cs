using System;

namespace ArchiCop.InfoData
{
    public enum LoadEngineType
    {
        Data,
        VisualStudio
    }

    public class LoadEngineInfo
    {
        public LoadEngineInfo(string engineName)
        {
            switch (engineName)
            {
                case "ArchiCop.VisualStudioData.VisualStudioProjectLoadEngine,ArchiCopCore":
                    EngineType = LoadEngineType.VisualStudio;
                    break;
                default:
                    EngineType = LoadEngineType.Data;
                    break;
            }
            EngineName = engineName;
        }

        public string EngineName { get; set; }

        public LoadEngineType EngineType { get; private set; }

        public string Arg1 { get; set; }
        public string Arg2 { get; set; }

        public object CreateLoadEngine()
        {
            Type type = Type.GetType(EngineName);
            object loadEngine;

            if (type == null)
            {
                string message = string.Format("LoadEngine with TypeName {0} doesn't exist.", EngineName);
                throw new ApplicationException(message);
            }
            if (!string.IsNullOrEmpty(Arg1) & !string.IsNullOrEmpty(Arg2))
            {
                loadEngine =
                    Activator.CreateInstance(type, new object[] {Arg1, Arg2});
            }
            else if (!string.IsNullOrEmpty(Arg1))
            {
                loadEngine =
                    Activator.CreateInstance(type, new object[] {Arg1});
            }
            else
            {
                loadEngine = Activator.CreateInstance(type);
            }

            return loadEngine;
        }
    }
}