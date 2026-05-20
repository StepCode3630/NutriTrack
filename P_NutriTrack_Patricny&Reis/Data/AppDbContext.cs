using Microsoft.EntityFrameworkCore;
using P_NutriTrack_Patricny_Reis.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P_NutriTrack_Patricny_Reis.Data
{
    public class AppDbContext : DbContext
    {
        private  string _databasePath;

        public AppDbContext(string databasePath)
        { _databasePath = databasePath; }

        public DbSet<Aliment> Aliments { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Consommation> Consommations { get; set; }



        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(
                $"Filename={_databasePath}");
        }
    }
}
