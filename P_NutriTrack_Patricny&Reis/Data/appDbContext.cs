using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P_NutriTrack_Patricny_Reis.Data
{
    public class appDbContext : DbContext
    {
        private string _databasePath;

        public appDbContext(string databasePath)
        {
            _databasePath = databasePath;
        }
    }
}
