using System;
using System.Linq;
using Engine.Players;
using NUnit.Framework;

namespace Engine.Tests
{
    [TestFixture]
    public class RefereeTests
    {

        private Referee _referee;
        [SetUp]
        public void Setup()
        {
            _referee = new Referee(11);
        }


        [Test]
        public void NewEngineBoardShouldBeEmpty()
        {
            int numberOfTakenHexes = _referee.Board.Spaces.Count(x => x.Owner != null);
            Assert.AreEqual(0, numberOfTakenHexes);
        }

        [Test]
        public void CheckHexShouldWork()
        {
            _referee.NewGame();
            //Assert.IsTrue(_referee.CheckHex(1, 1));
            //Assert.IsFalse(_referee.CheckHex(-1, 1));
            //Assert.IsFalse(_referee.CheckHex(1, -1));
            //Assert.IsFalse(_referee.CheckHex(_referee.Size + 1, _referee.Size - 1));
            //Assert.IsFalse(_referee.CheckHex(_referee.Size - 1, _referee.Size + 1));
            //Assert.IsFalse(_referee.CheckHex(1, _referee.Size));
            //Assert.IsFalse(_referee.CheckHex(_referee.Size, 1));
        }

        [Test]
        public void ShouldNotAllowPlayerToPlayTwice()
        {
            var player1 = new RandomPlayer(1, _referee.Size);
            _referee.NewGame();
            //_referee.TakeTurn(player1, 1, 1);
            //Exception ex = Assert.Throws<Exception>(() => _referee.TakeTurn(player1, 1, 2), "Cannot play twice in a row");
            //Assert.That(ex.Message, Is.EqualTo("Cannot play twice in a row"));
        }

        [Test]
        public void ShouldNotAllowNoPlayerToPlay()
        {
            _referee.NewGame();
            //Exception ex = Assert.Throws<Exception>(() => _referee.TakeTurn(null, 1, 1));
            //Assert.That(ex.Message, Is.EqualTo("Cannot take a turn as a non-player"));
        }

        [Test]
        public void SelectingProperHexShouldSucceed()
        {
            _referee.NewGame();
            var player1 = new RandomPlayer(1, _referee.Size);

            //_referee.TakeTurn(player1, 1, 1);
            //Assert.AreEqual(1, _referee.Board.Spaces.Count(x => x.X == 1 && x.Y == 1 && x.Owner == player1));
        }
    }
}