using AWJ.EmailProviders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmailProviders.Tests.EmailTemplateProviderTests
{
    [TestClass]
    public class ReplaceShould
    {
        [TestMethod]
        public void HtmlEncodeByDefault()
        {
            // Act
            var actual = EmailTemplateProvider.Replace("<td>{param}</td>", new Dictionary<string, string> { { "param", "</>" } });

            // Assert
            Assert.AreEqual("<td>&lt;/&gt;</td>", actual);
        }

        [TestMethod]
        public void HtmlEncodeWhenPassedTrue()
        {
            // Act
            var actual = EmailTemplateProvider.Replace("<td>{param}</td>", new Dictionary<string, string> { { "param", "</>" } }, true);

            // Assert
            Assert.AreEqual("<td>&lt;/&gt;</td>", actual);
        }

        [TestMethod]
        public void NotHtmlEncodeWhenPassedFalse()
        {
            // Act
            var actual = EmailTemplateProvider.Replace("<td>{param}</td>", new Dictionary<string, string> { { "param", "</>" } }, false);

            // Assert
            Assert.AreEqual("<td></></td>", actual);
        }
    }
}
