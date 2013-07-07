using LightInject;
using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Plugins.SkyNet.Code;
using Mnk.TBox.Plugins.SkyNet.Code.Interfaces;
using NUnit.Framework;
using Rhino.Mocks;

namespace Mnk.TBox.Tests.Plugins.SkyNet
{
    [TestFixture]
    class When_using_services_registrator
    {
        private IServiceContainer container;
        
        [SetUp]
        public void SetUp()
        {
            container = ServicesRegistrar.Register(MockRepository.GenerateMock<IPluginContext>(), ()=>null);
        }

        [TearDown]
        public void TearDown()
        {
            container.Dispose();
        }

        [Test]
        public void Should_register_executor()
        {
            //Act
            var s = container.GetInstance<ITaskExecutor>();

            //Assert
            Assert.IsNotNull(s);
        }

        [Test]
        public void Should_register_configs_facade()
        {
            //Act
            var s = container.GetInstance<IConfigsFacade>();

            //Assert
            Assert.IsNotNull(s);
        }

    }
}
