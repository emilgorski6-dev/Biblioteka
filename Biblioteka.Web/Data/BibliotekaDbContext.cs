using Microsoft.EntityFrameworkCore;
using Biblioteka.Web.Data.Entities;

namespace Biblioteka.Web.Data
{
    public class BibliotekaDbContext : DbContext
    {
        public BibliotekaDbContext(DbContextOptions<BibliotekaDbContext> options)
            : base(options)
        {
        }

        public DbSet<Uzytkownik> Uzytkownicy { get; set; }
    }
}