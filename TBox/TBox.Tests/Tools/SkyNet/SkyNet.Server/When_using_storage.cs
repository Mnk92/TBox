using Mnk.TBox.Tools.SkyNet.Server.Code;
using NUnit.Framework;

namespace Mnk.TBox.Tests.Tools.SkyNet.SkyNet.Server
{
    [TestFixture]
    class When_using_storage
    {
        [Test]
        public void Should_read_config()
        {
            //Arrange
            var storage = new ServerContext();

            //Act & Assert
            Assert.IsNotNull(storage.Config);
        }
    }
}
