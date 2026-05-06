using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P_NutriTrack_Patricny_Reis.DataModels;

namespace P_NutriTrack_Patricny_Reis.Data
{
    public class appDbContext : DbContext
    {
        public DbSet<Aliment> Aliments { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "nutri.db");
            options.UseSqlite($"Filename={dbPath}");
        }
    }
}
