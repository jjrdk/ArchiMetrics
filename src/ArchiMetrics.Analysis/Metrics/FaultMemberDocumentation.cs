// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FaultMemberDocumentation.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the FaultMemberDocumentation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Metrics
{
    using System.Collections.Generic;
    using ArchiMetrics.Analysis.Common.Metrics;

    internal class FaultMemberDocumentation : IMemberDocumentation
    {
        private readonly string _rawComment;

        public FaultMemberDocumentation(string rawComment)
        {
            _rawComment = rawComment;
        }

        public string Summary => _rawComment;

        public string Returns => _rawComment;

        public string Code => _rawComment;

        public string Example => _rawComment;

        public string Remarks => _rawComment;

        public IEnumerable<ParameterDocumentation> Parameters { get { yield break; } }

        public IEnumerable<TypeParameterDocumentation> TypeParameters { get { yield break; } }

        public IEnumerable<ExceptionDocumentation> Exceptions { get { yield break; } }
    }
}