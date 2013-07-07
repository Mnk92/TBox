using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LightInject;
using Mnk.TBox.Tools.SkyNet.Agent;
using Mnk.TBox.Tools.SkyNet.Common;
using NUnit.Framework;

namespace Mnk.TBox.Tests.Tools.SkyNet.SkyNet.Agent
{
    [TestFixture]
    class When_using_services_registrator
    {
        private IServiceContainer container;
        
        [SetUp]
        public void SetUp()
        {
            container = ServicesRegistrar.Register(new AgentConfig());
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
            var s = container.GetInstance<ISkyNetAgentService>();

            //Assert
            Assert.IsNotNull(s);
        }
    }
}
