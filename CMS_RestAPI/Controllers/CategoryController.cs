using CMS_RestAPI.Models;
using CMS_RestAPI.Models.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMS_RestAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public CategoryController(ApplicationDbContext applicationDbContext) => _applicationDbContext = applicationDbContext;

        [HttpGet]
        public async Task<IEnumerable<Category>> GetCategories()
        {
            return await _applicationDbContext.Categories.Where(x => x.Status != Status.Passive).OrderBy(x => x.Id).ToListAsync();

        }

        [HttpGet("{id:int}",Name ="GetCategory")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            Category category = await _applicationDbContext.Categories.FindAsync(id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        [HttpGet("{categorySlug}", Name = "GetCategoryBySlug")]
        public async Task<ActionResult<Category>> GetCategory(string categorySlug)
        {
            Category category = await _applicationDbContext.Categories.FirstOrDefaultAsync(x => x.Slug == categorySlug);
            if (category == null) return NotFound();
            return Ok(category);
        }

        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory(Category category)
        {
            _applicationDbContext.Categories.Add(category);
            await _applicationDbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCategories), category);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Category>> PutCategory(int id, Category category)
        {
            if (id != category.Id) return BadRequest();

            _applicationDbContext.Entry(category).State = EntityState.Modified;
            await _applicationDbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCategories), category);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Category>> DeleteCategory(int id){

            Category category = await _applicationDbContext.Categories.FindAsync(id);

            if (category == null) return NotFound();

            _applicationDbContext.Categories.Remove(category);
            await _applicationDbContext.SaveChangesAsync();
            return NoContent(); //=>204 status code

            }

    }
}
