// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuleProvider.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the RuleProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Tests
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Analysis.Common.CodeReview;
    using Moq;

    public class RuleProvider : IEnumerable<object[]>
    {
        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        public IEnumerator<object[]> GetEnumerator()
        {
            var spellChecker = new Mock<ISpellChecker>();
            spellChecker.Setup(x => x.Spell(It.IsAny<string>())).Returns(true);
            return AllRules.GetSyntaxRules(spellChecker.Object).Select(r => new object[] { r }).GetEnumerator();
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}