namespace ArchiCop.VisualStudioData
{
    public class VisualStudioProjectLibraryReference
    {
        public string Include { get; set; }
        public string SpecificVersion { get; set; }
        public string HintPath { get; set; }
        public string RequiredTargetFramework { get; set; }

        public string Name
        {
            get { return Include.Split(',').Length > 1 ? Include.Split(',')[0] : Include; }
        }

        public string Version
        {
            get { return Include.Split(',').Length > 1 ? Include.Split(',')[1].Split('=')[1] : ""; }
        }
    }
}