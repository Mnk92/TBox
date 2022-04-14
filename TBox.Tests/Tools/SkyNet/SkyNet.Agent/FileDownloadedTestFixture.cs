using Mnk.TBox.Tools.SkyNet.Agent.Code;
using Mnk.TBox.Tools.SkyNet.Common;
using Mnk.TBox.Tools.SkyNet.Common.Modules;
using Mnk.TBox.Tools.SkyNet.Contracts;
using Moq;
using NUnit.Framework;

namespace Mnk.TBox.Tests.Tools.SkyNet.SkyNet.Agent
{
    [TestFixture]
    class FileDownloadedTestFixture
    {
        [Test]
        public void Should_return_empty_string_if_nothing_to_do()
        {
            //Arrange
            var config = new AgentConfig();
            var packer = new Mock<IDataPacker>();
            var configProvider = new Mock<ConfigProvider<AgentConfig>>();
            configProvider.Setup(x => x.Config).Returns(config);
            var downloader = new FilesDownloader(configProvider.Object, packer.Object);

            //Act
            var result = downloader.DownloadAndUnpackFiles(string.Empty);

            //Assert
            Assert.AreEqual(string.Empty, result);
        }
    }
}
