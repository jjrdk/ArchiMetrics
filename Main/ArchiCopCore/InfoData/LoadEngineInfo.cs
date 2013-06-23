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
        public string EngineName { get; set; }
        public LoadEngineType EngineType { get; set; }

        public string Arg1 { get; set; }
        public string Arg2 { get; set; }

        public object CreateLoadEngine()
        {
            switch (EngineName)
            {
                case "ArchiCop.VisualStudioData.VisualStudioProjectLoadEngine,ArchiCopCore":
                    EngineType=LoadEngineType.VisualStudio;                    
                    break;                
            }

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