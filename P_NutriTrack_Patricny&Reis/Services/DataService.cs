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
            var path = Path.Combine(FileSystem.AppDataDirectory, "db.json");

            if (!File.Exists(path))
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync("db.json");
                using var reader = new StreamReader(stream);
                var json = await reader.ReadToEndAsync();

                await File.WriteAllTextAsync(path, json);
            }

            var finalJson = await File.ReadAllTextAsync(path);

            _data = JsonSerializer.Deserialize<NutriData>(finalJson);
        }

        private async Task SaveData()
        {
            var path = Path.Combine(FileSystem.AppDataDirectory, "db.json");

            var json = JsonSerializer.Serialize(_data, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await File.WriteAllTextAsync(path, json);
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
        public async Task addAliment(Aliment a)
        {
            // Ajoute un nouvel aliment
            _data.Aliments.Add(a);

            await SaveData();
        }

        public async Task removeAliment(int id)
        {
            // Cherche l'aliment correspondant
            var aliment = _data.Aliments.FirstOrDefault(a => a.AlimentId == id);

            //Supprime l'aliment si il existe sinon rien
            if (aliment != null)
                _data.Aliments.Remove(aliment);

            await SaveData();


        }

        public async Task updateAliment(Aliment alimentUpdated)
        {
            // Cherche l'aliment correspondant
            var aliment = _data.Aliments.FirstOrDefault(a => a.AlimentId == alimentUpdated.AlimentId);

            //Met à jour l'aliment si il existe sinon rien
            if (aliment != null)
            {
                aliment.Name = alimentUpdated.Name;
                aliment.Calories = alimentUpdated.Calories;
                aliment.Proteines_g = alimentUpdated.Proteines_g;
                aliment.Glucides_g = alimentUpdated.Glucides_g;
                aliment.Lipides_g = alimentUpdated.Lipides_g;
                aliment.Fibres_g = alimentUpdated.Fibres_g;
                aliment.Vitamines = alimentUpdated.Vitamines;
                aliment.Mineraux = alimentUpdated.Mineraux;
            }

            await SaveData();

        }
    }


}
