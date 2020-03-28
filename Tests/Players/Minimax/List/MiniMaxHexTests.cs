using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Players.Common;
using Players.Minimax.List;

namespace Tests.Players.Minimax.List
{
    [TestFixture]
    public class ListHexTests
    {
        private List<ListHex> hexes;
        private int size;

        [SetUp]
        public void Setup()
        {
            size = 11;
            hexes = new List<ListHex>(size * size);
            for (var row = 0; row < size; row++)
            {
                for (var column = 0; column < size; column++)
                {
                    var hex = new ListHex(size, row, column);
                    hexes.Add(hex);
                }
            }
        }

        [TearDown]
        public void TearDown()
        {
            size = 0;
            hexes = null;
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
            Assert.IsTrue(hex3.IsAttachedToTop);
            Assert.IsTrue(hex3.IsAttachedToLeft);
            Assert.IsFalse(hex3.IsAttachedToRight);
            Assert.IsFalse(hex3.IsAttachedToBottom);

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

            Assert.IsTrue(hex3.IsAttachedToRight);
            Assert.IsFalse(hex3.IsAttachedToBottom);
            Assert.AreEqual(1.0, hex3.Attached.At(0,5));
            Assert.IsTrue(hex3.IsAttachedTo(0,5));
            Assert.IsFalse(hex3.IsAttachedTo(3,3));


   
        }

        [Test]
        public void IsAttachedTo_ShouldWork_WhenAttachedOrNot()
        {
            var hex1 = new ListHex(11, 1, 1);
            var hex2 = new ListHex(11, 2, 2);
            Assert.IsFalse(hex1.IsAttachedTo(hex2));
            Assert.IsFalse(hex2.IsAttachedTo(null));
            hex1.AttachTo(hex2);
            Assert.IsTrue(hex1.IsAttachedTo(hex2));

            // Just because hex1 is considered to be attached to hex 2
            // that doesn't mean that hex2 is considered to be attached
            // to hex1.  That is not something that should be done at the 
            // hex level I Don't think

            Assert.IsFalse(hex2.IsAttachedTo(hex1));
        }

        [Test]
        public void ToString_ShouldOutput_AppropriateText()
        {
            var hex = new ListHex(11, 2, 10);
            Assert.AreEqual("(2, 10)", hex.ToString());

        }

        [Test]
        public void F_ShouldWork_Period()
        {
            var hex = new ListHex(11, 1, 1);
            hex.G = 5;
            hex.H = 10;
            Assert.AreEqual(15, hex.F());
            hex.ClearPathingVariables();
            Assert.AreEqual(0, hex.F());
        }

        [Test]
        public void Detaching_ShouldWork_WhenDoneProperly()
        {
            Assert.AreEqual(size * size, hexes.Count);
            OutputHex(hexes.ElementAt(0), "before attaching");
            OutputHex(hexes.ElementAt(3), "before attaching");
            
            AttachHexes(hexes.ElementAt(0), hexes.ElementAt(1));
            OutputHex(hexes.ElementAt(0), "After attaching " + hexes.ElementAt(1).HexName);

            AttachHexes(hexes.ElementAt(3), hexes.ElementAt(4));
            OutputHex(hexes.ElementAt(3), "Attached to " + hexes.ElementAt(4).HexName);

            // so 0,0 and 0,1 should be connected. 0,3 and 0,4 should be connected.
            // Adding 0,2 should connect them all.

            Assert.AreEqual(0.0, hexes.ElementAt(1).Attached.At(0,3));
            AttachHexes(hexes.ElementAt(1), hexes.ElementAt(2));
            AttachHexes(hexes.ElementAt(3), hexes.ElementAt(2));

            OutputHex(hexes.ElementAt(0), "After attaching " + hexes.ElementAt(1).HexName + " to " + hexes.ElementAt(2).HexName);
            OutputHex(hexes.ElementAt(1), "After attaching " + hexes.ElementAt(1).HexName + " to " + hexes.ElementAt(2).HexName);
            OutputHex(hexes.ElementAt(2), "After attaching " + hexes.ElementAt(1).HexName + " to " + hexes.ElementAt(2).HexName);
            OutputHex(hexes.ElementAt(3), "After attaching " + hexes.ElementAt(1).HexName + " to " + hexes.ElementAt(2).HexName);
        }

        [Test]
        public void Equals_ShouldBeTrue_WhileOnlyCaringAboutTheCoordinates()
        {
            var hex1 = new ListHex(11, 2, 2);
            var hex2 = new ListHex(11, 2, 2);
            Assert.IsTrue(hex1.Equals(hex2));
            hex1.Owner = PlayerType.Blue;
            hex2.Owner = PlayerType.Red;
            hex1.G = 15;
            hex1.H = 12;
            Assert.IsTrue(hex1.Equals(hex2));
        }

        private void OutputHex(ListHex hex, string message)
        {
            TestContext.WriteLine(hex.HexName + " " + message);
            TestContext.WriteLine(hex.Attached.ToString());
        }

        private void AttachHexes(ListHex hex1, ListHex hex2)
        {
            hex1.AttachTo(hex2);
            hex2.AttachTo(hex1);
        }
    }
}
