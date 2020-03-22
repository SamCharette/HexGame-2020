using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Players.Common;
using Players.Minimax.List;

namespace Tests.Players.Minimax.List
{
    [TestFixture]
    public class MinimaxListTests
    {
        [SetUp]
        public void Setup()
        {

        }

        [TearDown]
        public void TearDown()
        {

        }


        [Test]
        public void Neighbours_ShouldBeCreatedAppropriately_WhenAHexIsCreated()
        {
            var hex = new ListHex(11, 5, 5);
            Assert.AreEqual(6, hex.Neighbours.Count);
            var hex2 = new ListHex(11, 0, 1);
            Assert.AreEqual(4, hex2.Neighbours.Count);
        }

        [Test]
        public void Attaching_ShouldWork_WhenCreatingAHexNearAnother()
        {
            var hex = new ListHex(11, 5, 5);
            var hex2 = new ListHex(11, 5, 4);
            hex.AttachTo(hex2);

            Assert.AreEqual(1.0, hex.Attached.At(5,4));
            Assert.AreEqual(1.0, hex.Attached.At(5,5));
            Assert.AreEqual(1.0, hex2.Attached.At(5, 4));
            Assert.AreNotEqual(1.0, hex2.Attached.At(5, 5));

            hex2.AttachTo(hex);
            Assert.AreEqual(1.0, hex2.Attached.At(5, 5));

            var hex3 = new ListHex(11, 0, 0);
            Assert.IsTrue(hex3.AttachedToTop);
            Assert.IsTrue(hex3.AttachedToLeft);
            Assert.IsFalse(hex3.AttachedToRight);
            Assert.IsFalse(hex3.AttachedToBottom);

            hex3.AttachTo(new ListHex(11, 0, 1));
            hex3.AttachTo(new ListHex(11, 0, 2));
            hex3.AttachTo(new ListHex(11, 0, 3));
            hex3.AttachTo(new ListHex(11, 0, 4));
            hex3.AttachTo(new ListHex(11, 0, 5));
            hex3.AttachTo(new ListHex(11, 0, 6));
            hex3.AttachTo(new ListHex(11, 0, 7));
            hex3.AttachTo(new ListHex(11, 0,8));
            hex3.AttachTo(new ListHex(11, 0, 9));
            hex3.AttachTo(new ListHex(11, 0, 10));

            Assert.IsTrue(hex3.AttachedToRight);
            Assert.IsFalse(hex3.AttachedToBottom);
            Assert.AreEqual(1.0, hex3.Attached.At(0,5));
            Assert.IsTrue(hex3.IsAttachedTo(0,5));
            Assert.IsFalse(hex3.IsAttachedTo(3,3));
            Assert.IsTrue(hex3.IsAttachedToBothEnds(PlayerType.Red));
            Assert.IsFalse(hex3.IsAttachedToBothEnds(PlayerType.Blue));

            hex3.DetachFrom(new ListHex(11, 0, 5));
            Assert.AreNotEqual(1.0, hex3.Attached.At(0,5));
            Assert.IsFalse(hex3.AttachedToRight);
        }
    }
}
