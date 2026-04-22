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
        private NutriData _data;
    
        public async Task LoadData()
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("db.json");
            using var reader = new StreamReader(stream);

            string json = await reader.ReadToEndAsync();

            _data = JsonSerializer.Deserialize<NutriData>(json);
        }

        public List<Category> GetCategories()
        {
            return _data.Categories;
        }

        public List<Aliment> GetAliments()
        {
            return _data.Aliments; 
        }

        public List<Aliment> GetAlimentsById(int id)
        {
            return _data.Aliments.Where(a => a.CategoryFk == id).ToList();
        }
    }

}
