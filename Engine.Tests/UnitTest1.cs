using System;
using System.Linq;
using NUnit.Framework;

namespace Engine.Tests
{
    [TestFixture]
    public class EngineTests
    {

        private Engine _engine;
        [SetUp]
        public void Setup()
        {
            _engine = new Engine(11);
        }

        
        [Test]
        public void NewEngineBoardShouldBeEmpty()
        {
            int numberOfTakenHexes = _engine.Board.Count(x => x.Owner != HexOwner.Empty);
            Assert.AreEqual(0, numberOfTakenHexes);
        }

        [Test]
        public void SelectingOutOfRangeHexShouldFail()
        {
            Assert.That(() => _engine.TakeTurn(HexOwner.Player1, 1, -1), Throws.TypeOf<Exception>());
            Assert.That(() => _engine.TakeTurn(HexOwner.Player1, -1, 1), Throws.TypeOf<Exception>());
            Assert.That(() => _engine.TakeTurn(HexOwner.Player1, 1, 12), Throws.TypeOf<Exception>());
            Assert.That(() => _engine.TakeTurn(HexOwner.Player1, 12, 1), Throws.TypeOf<Exception>());
        }
    }
}