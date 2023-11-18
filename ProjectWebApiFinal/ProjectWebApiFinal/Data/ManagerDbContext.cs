using ProjectWebApiFinal.Models;
using Microsoft.EntityFrameworkCore;

namespace ProjectWebApiFinal.Data {
    public class ManagerDbContext : DbContext{
        public ManagerDbContext(DbContextOptions<ManagerDbContext> options) : base (options) { }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<Movie> Movies { get; set; }
    }
}
