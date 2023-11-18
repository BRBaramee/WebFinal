using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectWebApiFinal.Data;
using ProjectWebApiFinal.Models;

namespace ProjectWebApiFinal.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class ManagerController : ControllerBase {
        private readonly ManagerDbContext _context;
        public ManagerController(ManagerDbContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<Manager>> Get() {
            return await _context.Managers.ToListAsync();
        }

        [HttpGet("id")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id) {
            Manager? manager = await _context.Managers.FindAsync(id);
            return manager == null ? NotFound() : Ok(manager);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(Manager manager) {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(manager.Password);
            Manager data = new Manager {
                Id = manager.Id,
                FirstName = manager.FirstName,
                LastName = manager.LastName,
                Email = manager.Email,
                Password = passwordHash
            };

            await _context.Managers.AddAsync(data);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = manager.Id }, manager);
        }
        
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, Manager manager) {
            if (id != manager.Id) return BadRequest();
            _context.Entry(manager).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id) {
            var manager = await _context.Managers.FindAsync(id);
            if (manager == null) return NotFound();

            _context.Managers.Remove(manager);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
