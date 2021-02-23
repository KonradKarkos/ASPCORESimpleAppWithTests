using ASPCORESimpleAppWithTests.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ASPCORESimpleAppWithTests
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly OrderContext context;
        public ProductsController(OrderContext context)
        {
            this.context = context;
        }
        // GET: api/<ProductsController>
        [HttpGet]
        public IEnumerable<Product> Get()
        {
            return context.Set<Product>();
        }

        // GET api/<ProductsController>/5
        [HttpGet("{id}")]
        public Product Get(int id)
        {
            return context.Set<Product>().FirstOrDefault(p => p.Id == id);
        }

        // POST api/<ProductsController>
        [HttpPost]
        public ActionResult Post([FromBody] Product newProduct)
        {
            Product result = context.Add(newProduct).Entity;
            context.SaveChanges();
            return Ok(result);
        }

        // DELETE api/<ProductsController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            Product toDelete = context.Set<Product>().SingleOrDefault(p => p.Id == id);
            if(toDelete == null)
            {
                return BadRequest();
            }
            toDelete = context.Remove(toDelete).Entity;
            context.SaveChanges();
            return Ok(toDelete);
        }
    }
}
