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
namespace ArchiMetrics.Common.Tests
{
	using System;
	using System.Linq;
	using ArchiMetrics.Common.CodeReview;
	using ArchiMetrics.Common.Metrics;
	using NUnit.Framework;

	public class DtoReadTests
	{
		[TestCase(typeof(TypeParameterDocumentation))]
		[TestCase(typeof(ParameterDocumentation))]
		[TestCase(typeof(ExceptionDocumentation))]
		[TestCase(typeof(EvaluationResult))]
		public void CanReadProperty(Type type)
		{
			var constructorArgs = GetConstructorArgs(type);
			var instance = Activator.CreateInstance(type, constructorArgs);
			var properties = type.GetProperties();

			foreach (var property in properties)
			{
				Assert.DoesNotThrow(
					() => { var x = property.GetValue(instance); },
					"Could not read property: " + property.Name + " of type: " + type.Name);
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