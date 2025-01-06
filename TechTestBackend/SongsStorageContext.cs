using Microsoft.EntityFrameworkCore;

namespace TechTestBackend;

public class SongsStorageContext : DbContext
{
    public SongsStorageContext(DbContextOptions<SongsStorageContext> options)
        : base(options)
    {
    }

    public DbSet<SpotifySong> Songs { get; set; }
}