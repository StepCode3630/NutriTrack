using Microsoft.EntityFrameworkCore;
using P_NutriTrack_Patricny_Reis.Data;
using P_NutriTrack_Patricny_Reis.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace P_NutriTrack_Patricny_Reis.Services
{
    public class DataService
    {
        private readonly appDbContext _db = new();

        public async Task Init()
        {
            await _db.Database.EnsureCreatedAsync();
        }

        public Task<List<Category>> GetCategories()
        {
            return _db.Categories.ToListAsync();
        }

        public Task<List<Aliment>> GetAliments()
        {
            return _db.Aliments.ToListAsync(); 
        }

        public Task<List<Aliment>> GetAlimentsById(int id)
        {
            return _db.Aliments.Where(a => a.CategoryFk == id).ToListAsync();
        }
        public async Task addAliment(Aliment a)
        {
            // Ajoute un nouvel aliment
            _db.Aliments.Add(a);
            await _db.SaveChangesAsync();

        }

        public async Task removeAliment(int id)
        {
            // Cherche l'aliment correspondant
            var aliment = await _db.Aliments.FindAsync(id);

            //Supprime l'aliment si il existe sinon rien
            if (aliment != null)
                _db.Aliments.Remove(aliment);

            await _db.SaveChangesAsync();


        }

        public async Task updateAliment(Aliment alimentUpdated)
        {
            // Cherche l'aliment correspondant
            var aliment = _db.Aliments.FindAsync(alimentUpdated.AlimentId);

            //Met à jour l'aliment si il existe sinon rien
            if (aliment != null)
            {
                _db.Entry(aliment).CurrentValues.SetValues(alimentUpdated);
                await _db.SaveChangesAsync();
            }

        }
        public async Task<int> Count()
        {
            return await _db.Aliments.CountAsync();
        }
    }


}
