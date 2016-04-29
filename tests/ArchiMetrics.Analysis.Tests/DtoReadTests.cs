// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DtoReadTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   //   This source is subject to the Microsoft Public License (Ms-PL).
//   //   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   //   All other rights reserved.
// </copyright>
// <summary>
//   Defines the DtoReadTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMetrics.Analysis.Tests
{
    using System;
    using System.Linq;
    using Common.CodeReview;
    using Common.Metrics;
    using Xunit;

    public class DtoReadTests
    {
        [Theory]
        [InlineData(typeof(TypeParameterDocumentation))]
        [InlineData(typeof(ParameterDocumentation))]
        [InlineData(typeof(ExceptionDocumentation))]
        [InlineData(typeof(EvaluationResult))]
        public void CanReadProperty(Type type)
        {
            var constructorArgs = GetConstructorArgs(type);
            var instance = Activator.CreateInstance(type, constructorArgs);
            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                try
                {
                    var x = property.GetValue(instance);
                }
                catch
                {
                    Assert.True(false, "Could not read property: " + property.Name + " of type: " + type.Name);
                }
            }
        }

        private object[] GetConstructorArgs(Type type)
        {
            var constructors = type.GetConstructors();
            if (constructors.Any(_ => _.GetParameters().Length == 0))
            {
                return new object[0];
            }

            return constructors.First().GetParameters().Select(x => "a").Cast<object>().ToArray();
        }
    }
}