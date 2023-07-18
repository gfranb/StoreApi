using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreApi.DAL;
using StoreApi.Data;
using StoreApi.DTO;
using StoreApi.Models;

namespace StoreApi.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductController : Controller
    {
        private readonly StoreDbContext context;
        public ProductController(StoreDbContext dbContext)
        {
            this.context = dbContext;
        }
        //Get endpoint to fetch all the products from the db
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            using (var productRepository = new ProductRepository(context)) {
                var products = await productRepository.GetAll();
                return Ok(products);
            }
        }
        //Get endpoint to fetch a unique product by id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            using (var productRepository = new ProductRepository(context))
            {
                Product? product = await productRepository.GetById(id);
                if (product != null)
                {
                    return Ok(product);
                }
                else
                {
                    return NotFound("Product not found");
                }
            }
        }
        //Post endpoint to Add a product into the DB
        [HttpPost]
        public async Task<IActionResult> PostProduct(ProductDTO request)
        {
            using (var productRepository = new ProductRepository(context))
            {
                var newProduct = await productRepository.Insert(request);
                if (newProduct != null)
                {
                    return Created("Added", request);
                }
                else
                {
                    return BadRequest("Error creating the product, please verify the information");
                }
            }
        }
        //Put endpoint to update the data stored from a product
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, ProductDTO request)
        {
            using(var productRepository = new ProductRepository(context))
            {
                Product? product = await productRepository.Update(id, request);
                if (product != null)
                {
                    return Ok(product);
                }
                else
                {
                    return BadRequest("Error editing the product");
                }
            }
        }
        //Put endpoint to update the data stored from a product
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            using (var productRepository = new ProductRepository(context))
            {
                if (await productRepository.Delete(id))
                {
                    return Ok();
                }
                else
                {
                    return NotFound("Product not found");
                }
            }
        }
    }
}
