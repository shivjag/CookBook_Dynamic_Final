using Microsoft.EntityFrameworkCore;

namespace CookBook_CSharp.Data
{
    public partial class CookBookDbContext : DbContext
    {
        public CookBookDbContext(DbContextOptions<CookBookDbContext> options)
            : base(options) { }
    }
}