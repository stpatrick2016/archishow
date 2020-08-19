using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArchiShow.UnitTests
{
    [TestClass]
    public class OptionsTests
    {
        [TestMethod]
        public void Options_WhenPathIsAbsolute_ShouldReturnAsIs()
        {
            var path = @"Z:\TestPath";
            var options = new Options
            {
                Path = path
            };

            var real = options.GetPath();

            Assert.AreEqual(path, real);
        }

        [TestMethod]
        public void Options_WhenPathIsRelative_ShouldExpandToAbsolute()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "TestPath");
            var options = new Options
            {
                Path = "TestPath"
            };

            var real = options.GetPath();

            Assert.AreEqual(path, real);
        }

        [TestMethod]
        public void Options_WhenPathIsNull_ShouldReturnCurrentDir()
        {
            var options = new Options();

            var real = options.GetPath();

            Assert.IsTrue(string.Equals(Environment.CurrentDirectory, real, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
