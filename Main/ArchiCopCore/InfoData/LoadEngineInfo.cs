using System;

namespace ArchiCop.InfoData
{
    public class LoadEngineInfo
    {
        public string EngineName { get; set; }

        public string Arg1 { get; set; }
        public string Arg2 { get; set; }

        public object CreateLoadEngine()
        {
            Type loadEngineType = Type.GetType(EngineName);
            object loadEngine;

            if (loadEngineType == null)
            {
                string message = string.Format("LoadEngine with TypeName {0} doesn't exist.", EngineName);
                throw new ApplicationException(message);
            }
            if (!string.IsNullOrEmpty(Arg1) & !string.IsNullOrEmpty(Arg2))
            {
                loadEngine =
                    Activator.CreateInstance(loadEngineType, new object[] {Arg1, Arg2});
            }
            else if (!string.IsNullOrEmpty(Arg1))
            {
                loadEngine =
                    Activator.CreateInstance(loadEngineType, new object[] {Arg1});
            }
            else
            {
                loadEngine = Activator.CreateInstance(loadEngineType);
            }

            return loadEngine;
        }
    }
}