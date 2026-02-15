using System.Data;
using System.Reflection;
using ecommerce.api.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ecommerce.api.Database;

public class ApplicationDbContext(DbContextOptions options, 
    IConfiguration configuration) : DbContext (options)
{
    #region Entities
    public DbSet<Users.User> Users { get; set; }
    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }

    public IDbConnection CreateConnection() => new NpgsqlConnection(configuration.GetConnectionString("EcommerceConnectionString"));

}