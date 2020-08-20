using Microsoft.EntityFrameworkCore;

namespace loginReg.Models {

    public class MyContext : DbContext {
        public MyContext (DbContextOptions options) : base (options) { }
        public DbSet<NewUser> Users { get; set; }
    }
}