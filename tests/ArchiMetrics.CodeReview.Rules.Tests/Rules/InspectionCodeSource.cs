// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InspectionCodeSource.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the InspectionCodeSource type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Tests.Rules
{
    using System.Collections;
    using ArchiMetrics.CodeReview.Rules.Semantic;

    public static class InspectionCodeSource
    {
        public static IEnumerable BrokenSemanticCode
        {
            get
            {
                yield return new object[]
                                 {
                                     @"private int SomeMethod(int x)
		{
			return x;
		}",
                                     typeof(PossibleStaticMethod)
                                 };
                yield return new object[]
                                 {
                                     @"private void SomeMethod(int x)
		{
			var name = MethodBase.GetCurrentMethod().Name;
		}",
                                     typeof(UnusedParametersInMethodRule)
                                 };
            }
        }

    }
}