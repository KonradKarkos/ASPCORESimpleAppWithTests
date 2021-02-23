using ASPCORESimpleAppWithTests;
using ASPCORESimpleAppWithTests.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ASPCORESimpleTest
{
    public class OrderControllerTests
    {
        public OrderControllerTests()
        {
            ContextOptions = new DbContextOptionsBuilder<OrderContext>().UseSqlite("Filename=TestOrders.db").Options;
            FillDB();
        }
        protected DbContextOptions<OrderContext> ContextOptions { get; }
        private Product AddedTestProduct;
        private void FillDB()
        {
            using(var context = new OrderContext(ContextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                AddedTestProduct = context.Add(new Product()
                {
                    ProductName = "Chocolate",
                    Category = "Sweets",
                    AmountInStock = 50,
                    Price = 21.99,
                }).Entity;
                context.SaveChanges();
                context.Add(new Order()
                {
                    ClientName = "client1",
                    OrderTime = DateTime.Now,
                    Amount = 1,
                    DeliveryAddress = "Wiœniowa",
                    ProductId = AddedTestProduct.Id
                });
                context.Add(new Order()
                {
                    ClientName = "client2",
                    OrderTime = DateTime.Now,
                    Amount = 2,
                    DeliveryAddress = "Brzoskwiniowa",
                    ProductId = AddedTestProduct.Id
                });
                context.Add(new Order()
                {
                    ClientName = "client3",
                    OrderTime = DateTime.Now,
                    Amount = 3,
                    DeliveryAddress = "Czekoladowa",
                    ProductId = AddedTestProduct.Id
                });
                context.SaveChanges();
            }
        }
        [Fact]
        public void CanGetAllOrders()
        {
            List<Order> orders;


            using (var context = new OrderContext(ContextOptions))
            {
                var controller = new OrdersController(context);
                orders = ((DbSet<Order>)controller.Get()).ToList();
            }


            Assert.Equal(3, orders.Count);
            Assert.True(orders.FirstOrDefault(o => o.ClientName.Equals("client1")) != null);
            Assert.True(orders.FirstOrDefault(o => o.ClientName.Equals("client2")) != null);
            Assert.True(orders.FirstOrDefault(o => o.ClientName.Equals("client3")) != null);
        }
        [Theory]
        [InlineData(4)]
        [InlineData(8)]
        [InlineData(10)]
        [InlineData(35)]
        public void CanPostOrderAndTakeProducts(int orderedAmount)
        {
            Order newOrder = new Order()
            {
                ClientName = "client4",
                OrderTime = DateTime.Now,
                Amount = orderedAmount,
                DeliveryAddress = "Morelowa",
                ProductId = AddedTestProduct.Id
            };
            Order gottenOrder;
            OkObjectResult postResult;
            int expectedInStockAmount;
            int finalInStockAmount;
            using (var context = new OrderContext(ContextOptions))
            {
                expectedInStockAmount = context.Set<Product>().First(p => p.Id == AddedTestProduct.Id).AmountInStock - orderedAmount;



                var controller = new OrdersController(context);
                postResult = controller.Post(newOrder) as OkObjectResult;
            }
            using (var context = new OrderContext(ContextOptions))
            {
                gottenOrder = context.Set<Order>().Single(e => e.ClientName.Equals("client4"));
                finalInStockAmount = context.Set<Product>().First(p => p.Id == AddedTestProduct.Id).AmountInStock;
            }



            if (expectedInStockAmount < 0)
            {
                Assert.Null(postResult);
            }
            else
            {
                Assert.NotNull(postResult);
                Assert.Equal(newOrder, postResult.Value);
                Assert.Equal(newOrder, gottenOrder);
                Assert.Equal(finalInStockAmount, expectedInStockAmount);
            }
        }
        [Fact]
        public void CanDeleteOrderAndReturnProducts()
        {
            Order toDeleteOrder;
            int expectedInStockAmount;
            int finalInStockAmount;
            using (var context = new OrderContext(ContextOptions))
            {
                toDeleteOrder = context.Set<Order>().First();
                expectedInStockAmount = context.Set<Product>().First(p => p.Id == toDeleteOrder.ProductId).AmountInStock + toDeleteOrder.Amount;
            }
            OkObjectResult orderDeleteResult = null;



            using (var context = new OrderContext(ContextOptions))
            {
                OrdersController ordersController = new OrdersController(context);
                orderDeleteResult = ordersController.Delete(toDeleteOrder.Id) as OkObjectResult;
                finalInStockAmount = context.Set<Product>().First(p => p.Id == toDeleteOrder.Id).AmountInStock;
            }



            Assert.NotNull(orderDeleteResult);
            Assert.Equal(toDeleteOrder, orderDeleteResult.Value);
            Assert.Equal(expectedInStockAmount, finalInStockAmount);
        }
    }
}
