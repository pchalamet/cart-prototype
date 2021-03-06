﻿using System;
using NUnit.Framework;
using System.Linq;

namespace cart.grain.tests
{
    [TestFixture]
    public class CartServiceTests
    {
        [Test]
        public void Initial_state_is_empty()
        {
            var svc = new CartService();

            var op = Helpers.Build();
            var content = svc.Add(CartItems.Empty, op);

            Assert.IsEmpty(content.Items);
        }

        [Test]
        public void Add_new_id_can_be_observed()
        {
            var svc = new CartService();
            var content = svc.Add(CartItems.Empty, Helpers.Build(("A1", 5)));

            Assert.AreEqual(5, content.Items["A1"]);
            Assert.AreEqual(1, content.Items.Count);
        }

        [Test]
        public void Add_several_ids_can_be_observed()
        {
            var svc = new CartService();

            var content1 = svc.Add(CartItems.Empty, Helpers.Build(("A1", 5)));
            var content2 = svc.Add(content1, Helpers.Build(("A2", 20)));

            Assert.AreEqual(5, content2.Items["A1"]);
            Assert.AreEqual(20, content2.Items["A2"]);
            Assert.AreEqual(2, content2.Items.Count);
        }

        [Test]
        public void Zero_quantity_is_discarded()
        {
            var svc = new CartService();

            var content1 = svc.Add(CartItems.Empty, Helpers.Build(("A1", 5)));
            var content2 = svc.Add(content1, Helpers.Build(("A1", 5), ("A2", 20)));
            var content3 = svc.Remove(content2, Helpers.Build(("A1", 10)));

            Assert.AreEqual(20, content3.Items["A2"]);
            Assert.AreEqual(1, content3.Items.Count);
        }

        [Test]
        public void Zero_or_lower_quantity_is_discarded()
        {
            var svc = new CartService();

            var content1 = svc.Add(CartItems.Empty, Helpers.Build(("A1", 5)));
            var content2 = svc.Add(content1, Helpers.Build(("A1", 5), ("A2", 20)));
            var content3 = svc.Remove(content2, Helpers.Build(("A1", 20)));

            Assert.AreEqual(20, content3.Items["A2"]);
            Assert.AreEqual(1, content3.Items.Count);
        }

        [Test]
        public void Removing_unknown_article_is_not_error_and_article_is_not_in_the_basket()
        {
            var svc = new CartService();

            var content1 = svc.Add(CartItems.Empty, Helpers.Build(("A1", 5)));
            var content2 = svc.Remove(content1, Helpers.Build(("A2", 1)));

            Assert.AreEqual(5, content2.Items["A1"]);
            Assert.AreEqual(1, content2.Items.Count);
        }

        [Test]
        public void Quantity_does_not_overflow()
        {
            var svc = new CartService();

            var content1 = svc.Add(CartItems.Empty, Helpers.Build(("A1", 5)));
            var content2 = svc.Add(content1, Helpers.Build(("A1", int.MaxValue)));

            Assert.AreEqual(int.MaxValue, content2.Items["A1"]);
            Assert.AreEqual(1, content2.Items.Count);
        }

        [Test]
        public void Quantity_does_not_underflow()
        {
            var svc = new CartService();

            var content1 = svc.Add(CartItems.Empty, Helpers.Build(("A1", 1)));
            var content2 = svc.Remove(content1, Helpers.Build(("A1", int.MaxValue)));

            Assert.AreEqual(0, content2.Items.Count);
        }
    }
}
