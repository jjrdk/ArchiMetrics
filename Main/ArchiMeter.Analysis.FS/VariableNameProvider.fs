namespace ArchiMeter.Analysis.Xaml
open System.Collections.Generic

type internal VariableNameProvider() =
    static let UnnamedCache = new Dictionary<string, int>()
    static member GetUnnamedVariable(className : string) =
        if not (UnnamedCache.ContainsKey className) then
            UnnamedCache.[className] <- 0
        let number = UnnamedCache.[className]
        let name = className.ToLower() + number.ToString()
        UnnamedCache.[className] <- (number + 1)
        name.Replace(".", "_")
