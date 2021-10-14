using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyTodo.Data;
using MyTodo.Models;
using MyTodo.ViewModels;

namespace MyTodo.Controllers
{
    [ApiController]
    [Route("v1")]
    public class TodoController : ControllerBase
    {
        [HttpGet]
        [Route("todos")]
        public async Task<IActionResult> GetAsync([FromServices] AppDbContext context)
        {
            var todos = await context.Todos.AsNoTracking().ToListAsync();
            return Ok(todos);
        }
        
        [HttpGet]
        [Route("todos/{id}")]
        public async Task<IActionResult> GetByIdAsync([FromServices] AppDbContext context,
            [FromRoute] int id
            )
        {
            var todo = await context.Todos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (todo == null)
            {
                return NotFound();
                
            }
                
            return Ok(todo);
        }
        
        [HttpPost("todos")]
        public async Task<IActionResult> CreateAsync([FromServices] AppDbContext context,
            [FromBody] CreateTodoViewModel model
        )
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var todo = new Todo
            {
                Date = DateTime.Now,
                Done = false,
                Title = model.Title
            };
            try
            {
                await context.Todos.AddAsync(todo);
                await context.SaveChangesAsync();
                return Created($"api/todos/{todo.Id}", todo);
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }
        
        [HttpPut("todos/{id}")]
        public async Task<IActionResult> UpdateAsync([FromServices] AppDbContext context,
            [FromBody] CreateTodoViewModel model,
            [FromRoute] int id
        )
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var todo = await context.Todos.FirstOrDefaultAsync(x => x.Id == id);
            if (todo == null)
            {
                return NotFound();
            }

            try
            {
                todo.Title = model.Title;
                context.Todos.Update(todo);
                await context.SaveChangesAsync();
                return Ok(todo);
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpDelete("todos/{id}")]
        public async Task<IActionResult> RemoveAsync(
            [FromServices] AppDbContext context, 
            [FromRoute] int id)
        {
            var todo = await context.Todos.FirstOrDefaultAsync(x => x.Id == id);
            try
            {
                context.Todos.Remove(todo);
                await context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }
    }
}