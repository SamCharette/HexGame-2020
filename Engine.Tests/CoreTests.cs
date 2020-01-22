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
        public void CheckHexShouldWork()
        {
            _engine.NewGame();
            Assert.IsTrue(_engine.CheckHex(1, 1));
            Assert.IsFalse(_engine.CheckHex(-1, 1));
            Assert.IsFalse(_engine.CheckHex(1, -1));
            Assert.IsFalse(_engine.CheckHex(_engine.Size + 1, _engine.Size - 1));
            Assert.IsFalse(_engine.CheckHex(_engine.Size - 1, _engine.Size + 1));
            Assert.IsFalse(_engine.CheckHex(1, _engine.Size));
            Assert.IsFalse(_engine.CheckHex(_engine.Size, 1));
        }

        [Test]
        public void ShouldNotAllowPlayerToPlayTwice()
        {
            _engine.NewGame();
            _engine.TakeTurn(HexOwner.Player1, 1, 1);
            Assert.Throws<Exception>(() => _engine.TakeTurn(HexOwner.Player1, 1, 2), "Cannot play twice in a row");
        }

        [Test]
        public void ShouldNotAllowNoPlayerToPlay()
        {
            _engine.NewGame();
            Exception ex = Assert.Throws<Exception>(() => _engine.TakeTurn(HexOwner.Empty, 1, 1));
            Assert.That(ex.Message, Is.EqualTo("Cannot take a turn as a non-player"));
        }

        [Test]
        public void SelectingProperHexShouldSucceed()
        {
            _engine.NewGame();
            _engine.TakeTurn(HexOwner.Player1, 1, 1);
            Assert.AreEqual(1, _engine.Board.Count(x => x.X == 1 && x.Y == 1 && x.Owner == HexOwner.Player1));
        }
    }
}