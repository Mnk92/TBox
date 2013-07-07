using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mnk.TBox.Tools.SkyNet.Agent.Code;
using Mnk.TBox.Tools.SkyNet.Common;
using Mnk.TBox.Tools.SkyNet.Common.Modules;
using NUnit.Framework;
using Rhino.Mocks;

namespace Mnk.TBox.Tests.Tools.SkyNet.SkyNet.Agent
{
    [TestFixture]
    class When_using_file_downloader
    {
        [Test]
        public void Should_return_empty_string_if_nothing_to_do()
        {
            //Arrange
            var config = new AgentConfig();
            var packer = MockRepository.GenerateStub<IDataPacker>();
            var downloader = new FilesDownloader(config, packer);

            //Act
            var result = downloader.DownloadAndUnpackFiles(string.Empty);

            //Assert
            Assert.AreEqual(string.Empty, result);
        }
    }
}
