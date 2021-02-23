using ASPCORESimpleAppWithTests.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ASPCORESimpleAppWithTests
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrderContext context;
        public OrdersController(OrderContext context)
        {
            this.context = context;
        }
        // GET: api/<OrderController>
        [HttpGet]
        public IEnumerable<Order> Get()
        {
            return context.Set<Order>();
        }

        // GET api/<OrderController>/5
        [HttpGet("{id}")]
        public Order Get(int id)
        {
            return context.Set<Order>().FirstOrDefault(o => o.Id == id);
        }

        // POST api/<OrderController>
        [HttpPost]
        public ActionResult Post([FromBody] Order newOrder)
        {
            Product orderedProduct = context.Set<Product>().FirstOrDefault(p => p.Id == newOrder.ProductId);
            if (newOrder.Amount <= 0 || orderedProduct == null || orderedProduct.AmountInStock < newOrder.Amount)
            {
                return BadRequest();
            }
            orderedProduct.AmountInStock -= newOrder.Amount;
            context.Update(orderedProduct);
            Order order = context.Add(newOrder).Entity;
            context.SaveChanges();
            return Ok(order);
        }

        // DELETE api/<OrderController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            Order orderToDelete = context.Set<Order>().FirstOrDefault(o => o.Id == id);
            if(orderToDelete == null)
            {
                return NotFound();
            }
            context.Remove(orderToDelete);
            Product orderedProduct = context.Set<Product>().FirstOrDefault(p => p.Id == orderToDelete.ProductId);
            if(orderedProduct != null)
            {
                orderedProduct.AmountInStock += orderToDelete.Amount;
                context.Update(orderedProduct);
            }
            context.SaveChanges();
            return Ok(orderToDelete);
        }
    }
}
