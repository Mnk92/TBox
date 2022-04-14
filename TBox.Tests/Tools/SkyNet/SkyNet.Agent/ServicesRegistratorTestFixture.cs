using LightInject;
using Mnk.TBox.Tools.SkyNet.Agent;
using Mnk.TBox.Tools.SkyNet.Common;
using Mnk.TBox.Tools.SkyNet.Contracts;
using Moq;
using NUnit.Framework;

namespace Mnk.TBox.Tests.Tools.SkyNet.SkyNet.Agent
{
    [TestFixture]
    class ServicesRegistratorTestFixture
    {
        private Mock<ConfigProvider<AgentConfig>> provider;
        private IServiceContainer container;

        [SetUp]
        public void SetUp()
        {
            provider = new Mock<ConfigProvider<AgentConfig>>();
            provider.Setup(x => x.Config).Returns(new AgentConfig());
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
            var s = container.GetInstance<ISkyNetAgentService.ISkyNetAgentServiceClient>();

            //Assert
            Assert.IsNotNull(s);
        }
    }
}
