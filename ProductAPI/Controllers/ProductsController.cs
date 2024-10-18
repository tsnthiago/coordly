using Microsoft.AspNetCore.Mvc;
using ProductAPI.Models;
using ProductAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ProductAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _repository;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductRepository repository, ILogger<ProductsController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<ProductsResponse>> GetProducts(int pageNumber = 1, int pageSize = 10)
        {
            if (pageSize > 50)
                pageSize = 50;

            try
            {
                var (products, totalCount) = await _repository.GetAllAsync(pageNumber, pageSize);
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                var response = new ProductsResponse
                {
                    Products = products,
                    CurrentPage = pageNumber,
                    PageSize = pageSize,
                    TotalPages = totalPages,
                    TotalCount = totalCount
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting products");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            try
            {
                var product = await _repository.GetByIdAsync(id);
                return Ok(product);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/Products
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdProduct = await _repository.CreateAsync(product);
            return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.ProductID }, createdProduct);
        }

        // PUT: api/Products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, Product product)
        {
            if (id != product.ProductID)
            {
                return BadRequest("ID mismatch");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _repository.UpdateAsync(id, product);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product");
                return StatusCode(500, "An error occurred while updating the product.");
            }
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                await _repository.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
