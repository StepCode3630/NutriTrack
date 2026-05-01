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

            if (_data != null)
            {
                _data.Categories ??= new List<Category>();
                _data.Aliments ??= new List<Aliment>();
                _data.Consommations ??= new List<Consommation>();
                _data.Mineraux ??= new List<Minerau>();
                _data.Vitamines ??= new List<Vitamine>();
                _data.AlimentMineraux ??= new List<AlimentMinerau>();
                _data.AlimentVitamines ??= new List<AlimentVitamine>();
            }
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

        // Bilan CONSO QUOTIDIENNE DE L'UTILISATEUR

        // Recup les conso du jour que aujourd'hui pour le moment
        public List<Consommation> GetConsommationsDuJour()
        {
            DateTime aujourdhui = DateTime.Today;
            return _data.Consommation_journaliere
                .Where(c => c.DateConsommation.Date == aujourdhui)
                .ToList();
        }

        // add une nouvelle consommation
        public void addConso(Consommation nouvelleConso)
        {
            // Calcul du nouvel ID
            if (_data..Count == 0)
                nouvelleConso.ConsommationId = 1;
            else
                nouvelleConso.ConsommationId = _data.Consommations.Max(c => c.ConsommationId) + 1;

            _data.Consommations.Add(nouvelleConso);
        }

        // maj une conso existante on peut modifier la quantité
        public void updateConso(Consommation consoModifiee)
        {
            Consommation? existante = _data.Consommations
                .FirstOrDefault(c => c.ConsommationId == consoModifiee.ConsommationId);

            if (existante != null)
            {
                existante.Quantite_g = consoModifiee.Quantite_g;
            }
        }

        // delete une consommation
        public void removeConso(int consommationId)
        {
            Consommation? aSupprimer = _data.Consommations
                .FirstOrDefault(c => c.ConsommationId == consommationId);

            if (aSupprimer != null)
                _data.Consommations.Remove(aSupprimer);
        }

        // Récup un aliment par son ID utile pour afficher les détails dans la liste de conso
        public Aliment? GetAlimentById(int alimentId)
        {
            return _data.Aliments.FirstOrDefault(a => a.AlimentId == alimentId);
        }
    }


}
