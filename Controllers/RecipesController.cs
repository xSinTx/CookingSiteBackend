using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Temalabor.Models;
using AuthorizeAttribute = Microsoft.AspNetCore.Authorization.AuthorizeAttribute;
using FromBodyAttribute = Microsoft.AspNetCore.Mvc.FromBodyAttribute;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace Temalabor.Controllers
{
    [Route("api/")]
    [ApiController]
    public class RecipesController : ControllerBase
    {
        private readonly RecipeContext _context;

        public RecipesController(RecipeContext context)
        {
            _context = context;
        }

        // GET: api/Recipes
        [HttpGet("Recipes")]
        public async Task<ActionResult<IEnumerable<Recipe>>> GetRecipes([FromQuery] string? search = null, [FromQuery] int from = 0)
        {
            return
                await
                _context
                .Recipes.Include(r => r.Ingredients)
                .Where(r => string.IsNullOrEmpty(search) ? true : r.Title.Contains(search))
                .Skip(from)
                .Take(2)
                .ToListAsync();
        }

        // GET: api/Recipes/5
        [HttpGet("Recipes/{id}")]
        public async Task<ActionResult<Recipe>> GetRecipe(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);

            if (recipe == null)
            {
                return NotFound();
            }

            return recipe;
        }


        // POST: api/Recipes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /*[HttpPost("Recipes")]
        public async Task<ActionResult<Recipe>> PostRecipe(Recipe recipe)
        {
            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRecipe), new { id = recipe.Id }, recipe);
        }
        */
        [HttpPost("Recipes")]
        public async Task<ActionResult> CreateRecipe([FromBody] DTO.Recipe newRecipe)
        {
            if (newRecipe.Ingredients.Length == 0)
                return BadRequest("No ingredients given");

            ICollection<Ingredient> dbIngredients = new Collection<Ingredient>();

            var dbIngredient = await _context.Ingredients.ToListAsync();
            foreach (var ingredient in newRecipe.Ingredients)
            {
                Ingredient? ingredientAlreadyExisted = dbIngredient.FirstOrDefault(i => i.Name == ingredient.Name);
                if (ingredientAlreadyExisted is null)
                    dbIngredients.Add(new Ingredient() { Name = ingredient.Name });
                else 
                    dbIngredients.Add(ingredientAlreadyExisted);
            }
            var dbUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == newRecipe.User.Email);
            if (dbUser is null)
                dbUser = new User() 
                { 
                    UserName = newRecipe.User.UserName,
                    Password = newRecipe.User.Password,
                    Email = newRecipe.User.Email
                };

            var dbRecipe = new Recipe()
            {
                Title = newRecipe.Title,
                Time = newRecipe.Time,
                Description = newRecipe.Description,
                Ingredients = dbIngredients,
                User = dbUser
            };

            _context.Recipes.Add(dbRecipe);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRecipes), new { id = dbRecipe.Id },
                new DTO.Recipe(
                    dbRecipe.Id, 
                    dbRecipe.Title, 
                    dbRecipe.Time,
                    dbRecipe.Description)); 
        }

        //// DELETE: api/Recipes/5
        //[HttpDelete("Recipes/{id}")]
        //public async Task<IActionResult> DeleteRecipe(int id)
        //{
        //    var recipe = await _context.Recipes.FindAsync(id);
        //    if (recipe == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Recipes.Remove(recipe);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        private bool RecipeExists(int id)
        {
            return _context.Recipes.Any(e => e.Id == id);
        }

        // GET: api/Ingredients 

        [HttpGet("Ingredients")]
        public async Task<ActionResult<IEnumerable<Ingredient>>> GetIngredients()
        {
            return await _context.Ingredients.ToListAsync();
        }

        // GET: api/RecipesOfUser
        [Authorize]
        [HttpGet("RecipesOfUser")]
        public async Task<ActionResult<IEnumerable<Recipe>>> GetRecipesofUser()
        {
            Console.WriteLine("Itt");
            int id = int.Parse(
                User.Claims.First(i => i.Type == "UserId").Value);
            return await _context.Recipes.Where(r => r.User.Id == id).Include(r => r.Ingredients).ToListAsync();
        }

        // GET: api/User 
        [Authorize]
        [HttpGet("User")]
        public async Task<ActionResult<User>> GetUser()
        {
            string email = User.Claims.First(i => i.Type == "Email").Value;
            if (email == null)
                return BadRequest();
            else
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
                if (user == null)
                    return NotFound();
                else
                    return user;
            }
        }

        [HttpPost("SignUp")]
        public async Task<ActionResult<User>> NewUser(User user)
        {
            if (user == null)
                return BadRequest();

           
            var exsisting = await _context.Users.Where(u => u.Email == user.Email || u.UserName == user.UserName).ToListAsync();
            if (exsisting.Any()) {
                return Forbid();

            } else {

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(NewUser), new { id = user.Id }, user);

            }

            
        }
    }
}
