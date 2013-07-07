using Mnk.TBox.Tools.SkyNet.Common;
using Mnk.TBox.Tools.SkyNet.Server.Code;
using Mnk.TBox.Tools.SkyNet.Server.Code.Interfaces;
using Mnk.TBox.Tools.SkyNet.Server.Code.Processing;
using NUnit.Framework;

namespace Mnk.TBox.Tests.Tools.SkyNet.SkyNet.Server
{
    [TestFixture]
    class When_using_agents_cache
    {
        private IAgentsCache agentsCache;
        private ServerAgent agent;
        [SetUp]
        public void SetUp()
        {
            agent = new ServerAgent {Endpoint = "http://localost"};
            agentsCache = new AgentsCache();
        }

        [TearDown]
        public void TearDown()
        {
            agentsCache.Dispose();
        }

        [Test]
        public void Shouldnt_be_exception_if_clear_empty()
        {
            //Assert
            agentsCache.Clear();
        }

        [Test]
        public void Shouldnt_be_exception_if_get_and_clear_empty()
        {
            //Assert
            agentsCache.Get(agent);
            agentsCache.Clear();
        }

        [Test]
        public void Should_return_service_on_get()
        {
            //Act
            var service = agentsCache.Get(agent);

            //Assert
            Assert.IsNotNull(service);
        }
    }
}
