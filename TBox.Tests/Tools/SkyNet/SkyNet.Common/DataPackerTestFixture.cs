using System.IO;
using System.IO.Compression;
using Mnk.TBox.Tools.SkyNet.Common.Modules;
using NUnit.Framework;

namespace Mnk.TBox.Tests.Tools.SkyNet.SkyNet.Common
{
    [TestFixture]
    [Category("Integration")]
    internal class DataPackerTestFixture
    {
        private string zipPath;

        [SetUp]
        public void SetUp()
        {
            zipPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        }

        [TearDown]
        public void TearDown()
        {
            File.Delete(zipPath);
        }

        [Test]
        public void Should_unpack_data()
        {
            //Arrange
            var packer = new DataPacker();
            var expected = AppDomain.CurrentDomain.BaseDirectory;
            ZipFile.CreateFromDirectory(expected, zipPath);

            //Act
            string actual;
            using (var s = File.OpenRead(zipPath))
            {
                actual = packer.Unpack(s);
            }

            //Assert
            Assert.AreEqual(GetSize(expected), GetSize(actual));
        }

        private static long GetSize(string path)
        {
            var size = new DirectoryInfo(path).EnumerateFiles().Sum(x => x.Length);
            return size;
        }
    }
}
