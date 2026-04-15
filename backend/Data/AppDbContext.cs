using BoschPizza.Models;
using Microsoft.EntityFrameworkCore;

namespace BoschPizza.Data;

public class AppDbContext : DbContext
{
    // Construtor que recebe as opcaoes do contexto pela injecao de dependencias
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}


    //Representa a tabela Pizza no banco de dados
    public DbSet<Pizza> Pizzas { get; set; }


    public DbSet <UserLogin> UserLogins { get; set; }

     public DbSet <Cliente> Clientes { get; set; }

     public DbSet<User> Users { get; set; }
}
