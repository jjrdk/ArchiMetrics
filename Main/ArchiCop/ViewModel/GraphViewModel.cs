using System;
using System.Collections.Generic;
using System.Linq;

namespace ArchiCop.ViewModel
{
    /// <summary>
    ///     Base class for all ViewModel classes in the application.
    ///     It provides support for property change notifications
    ///     and has a DisplayName property.  This class is abstract.
    /// </summary>
    public class GraphViewModel : WorkspaceViewModel
    {
        public GraphViewModel(GraphInfo info)
        {
            DisplayName = info.DisplayName;

            Type loadEngineType = Type.GetType(info.LoadEngine);

            if (loadEngineType != null)
            {
                IEnumerable<ArchiCopEdge> edges;

                if (info.Arg1 != null & info.Arg2 != null)
                {
                    edges =
                        (IEnumerable<ArchiCopEdge>)
                        Activator.CreateInstance(loadEngineType, new object[] {info.Arg1, info.Arg2});
                }
                else if (info.Arg1 != null)
                {
                    edges =
                        (IEnumerable<ArchiCopEdge>) Activator.CreateInstance(loadEngineType, new object[] {info.Arg1});
                }
                else
                {
                    edges = (IEnumerable<ArchiCopEdge>) Activator.CreateInstance(loadEngineType);
                }

                if (info.VertexRegexRules.Any())
                {
                    edges = new EdgeRegexEngine(edges, info.VertexRegexRules);
                }

                GraphToVisualize = new ArchiCopGraph(edges);
            }
        }

        public ArchiCopGraph GraphToVisualize { get; private set; }
    }
}