using LightInject;
using Mnk.TBox.Tools.SkyNet.Common;
using Mnk.TBox.Tools.SkyNet.Common.Modules;
using Mnk.TBox.Tools.SkyNet.Contracts;
using Mnk.TBox.Tools.SkyNet.Server;
using Moq;
using NUnit.Framework;

namespace Mnk.TBox.Tests.Tools.SkyNet.SkyNet.Server
{
    [TestFixture]
    class ServicesRegistratorTestFixture
    {
        private Mock<ConfigProvider<ServerConfig>> provider;
        private IServiceContainer container;

        [SetUp]
        public void SetUp()
        {
            provider = new Mock<ConfigProvider<ServerConfig>>();
            provider.Setup(x => x.Config).Returns(new ServerConfig());
            container = ServicesRegistrar.Register(provider.Object);
        }

        [TearDown]
        public void TearDown()
        {
            container.Dispose();
        }

        [Test]
        public void Should_register_service()
        {
            //Act
            var s = container.GetInstance<ISkyNetFileService.ISkyNetFileServiceClient>();

            //Assert
            Assert.IsNotNull(s);
        }

        [Test]
        public void Should_register_modules_runner()
        {
            //Act
            var s = container.GetInstance<IModulesRunner>();

            //Assert
            Assert.IsNotNull(s);
        }
    }
}
