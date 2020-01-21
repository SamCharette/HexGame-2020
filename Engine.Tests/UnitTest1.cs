using System.Linq;
using NUnit.Framework;

namespace Engine.Tests
{
    public class EngineTests
    {

        private Engine _engine;
        [SetUp]
        public void Setup()
        {
            _engine = new global::Engine.Engine(11);
        }


        [Test]
        public void NewEngineBoardShouldBeEmpty()
        {
            int numberOfTakenHexes = _engine.Board.Count(x => x.Owner != HexOwner.Empty);
            Assert.AreEqual(0, numberOfTakenHexes);
        }
    }
}