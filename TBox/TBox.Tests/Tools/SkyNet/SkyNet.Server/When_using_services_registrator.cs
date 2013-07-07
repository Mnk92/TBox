using LightInject;
using Mnk.TBox.Tools.SkyNet.Common;
using Mnk.TBox.Tools.SkyNet.Common.Modules;
using Mnk.TBox.Tools.SkyNet.Server;
using Mnk.TBox.Tools.SkyNet.Server.Code.Interfaces;
using NUnit.Framework;

namespace Mnk.TBox.Tests.Tools.SkyNet.SkyNet.Server
{
    [TestFixture]
    class When_using_services_registrator
    {
        private IServiceContainer container;
        
        [SetUp]
        public void SetUp()
        {
            container = ServicesRegistrar.Register(new ServerConfig());
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
            var s = container.GetInstance<ISkyNetCommon>();

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
