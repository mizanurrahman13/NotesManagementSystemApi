using Microsoft.EntityFrameworkCore;
using NOTEKEEPER.Api.Entities;

namespace NOTEKEEPER.Api.Contexts;

public class ApplicationContext: DbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options) 
        : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Note> Notes { get; set; }
}
