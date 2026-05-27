/******************************************************************************
** PROGRAMME  DataService.cs                                                 **
**                                                                           **
** Lieu      : ETML - section informatique                                   **
** Auteur    : Camille Rais                                                  **
** Date      : 01.04.2026                                                    **
**                                                                           **
******************************************************************************/

/******************************************************************************
** DESCRIPTION                                                               **
**                                                                           **
** Service de gestion des données de l'application NutriTrack                **
** Charge / sauvegarde le fichier JSON et fournit les méthodes CRUD          **
** pour les aliments et les consommations journalières                       **
**                                                                           **
******************************************************************************/

using P_NutriTrack_Patricny_Reis.DataModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace P_NutriTrack_Patricny_Reis.Services
{
    public class DataService
    {
        // données globales chargées depuis le JSON
        private NutriData donnees;

        // CHARGEMENT & SAUVEGARDE DU JSON

        // charge le JSON depuis le stockage local sinon le copie depuis l app package
        public async Task LoadData()
        {
            string chemin = Path.Combine(FileSystem.AppDataDirectory, "db.json");

            // 1ere utilisation alors on copie le JSON inclus dans l'app
            if (!File.Exists(chemin))
            {
                using Stream stream = await FileSystem.OpenAppPackageFileAsync("db.json");
                using StreamReader reader = new StreamReader(stream);
                string jsonInitial = await reader.ReadToEndAsync();
                await File.WriteAllTextAsync(chemin, jsonInitial);
            }

            // lit le contenu du JSON puis le désérialise
            string jsonFinal = await File.ReadAllTextAsync(chemin);
            donnees = JsonSerializer.Deserialize<NutriData>(jsonFinal);

            // au cas où certaines listes seraient null on les initialise vides
            if (donnees != null)
            {
                donnees.Categories ??= new List<Category>();
                donnees.Aliments ??= new List<Aliment>();
                donnees.Consommation_journaliere ??= new List<Consommation>();
                donnees.Mineraux ??= new List<Minerau>();
                donnees.Vitamines ??= new List<Vitamine>();
                donnees.AlimentMineraux ??= new List<AlimentMinerau>();
                donnees.AlimentVitamines ??= new List<AlimentVitamine>();
            }

            // si une conso n'a pas de date, on lui met la date d'aujourd'hui comme ca les données s'affichent
            foreach (Consommation conso in donnees.Consommation_journaliere)
            {
                if (conso.DateConsommation == DateTime.MinValue)
                {
                    conso.DateConsommation = DateTime.Today;
                }
            }
        }

        // sauvegarde toutes les données dans le fichier JSON
        private async Task SaveData()
        {
            string chemin = Path.Combine(FileSystem.AppDataDirectory, "db.json");

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string json = JsonSerializer.Serialize(donnees, options);
            await File.WriteAllTextAsync(chemin, json);
        }

        // CATEGORIES

        // retourne toutes les catégories
        public List<Category> GetCategories()
        {
            return donnees.Categories;
        }

        // ALIMENTS

        // retourne tous les aliments
        public List<Aliment> GetAliments()
        {
            return donnees.Aliments;
        }

        // retourne les aliments d'une catégorie
        public List<Aliment> GetAlimentsByCategorie(int categorieId)
        {
            return donnees.Aliments
                .Where(aliment => aliment.CategoryFk == categorieId)
                .ToList();
        }

        // retourne un aliment par son ID
        public Aliment? GetAlimentById(int alimentId)
        {
            return donnees.Aliments.FirstOrDefault(aliment => aliment.AlimentId == alimentId);
        }

        // ajoute un nouvel aliment
        public async Task AddAliment(Aliment nouvelAliment)
        {
            donnees.Aliments.Add(nouvelAliment);
            await SaveData();
        }

        // supprime un aliment par son ID
        public async Task RemoveAliment(int alimentId)
        {
            Aliment? aliment = donnees.Aliments
                .FirstOrDefault(item => item.AlimentId == alimentId);

            if (aliment != null)
                donnees.Aliments.Remove(aliment);

            await SaveData();
        }

        // met à jour un aliment existant
        public async Task UpdateAliment(Aliment alimentModifie)
        {
            Aliment? aliment = donnees.Aliments
                .FirstOrDefault(item => item.AlimentId == alimentModifie.AlimentId);

            if (aliment != null)
            {
                aliment.Name = alimentModifie.Name;
                aliment.Calories = alimentModifie.Calories;
                aliment.Proteines_g = alimentModifie.Proteines_g;
                aliment.Glucides_g = alimentModifie.Glucides_g;
                aliment.Lipides_g = alimentModifie.Lipides_g;
                aliment.Fibres_g = alimentModifie.Fibres_g;
                aliment.Vitamines = alimentModifie.Vitamines;
                aliment.Mineraux = alimentModifie.Mineraux;
            }

            await SaveData();
        }

        // CONSOMMATIONS JOURNALIERES
        

        // retourne toutes les consommations du jour
        public List<Consommation> GetConsommationsDuJour()
        {
            DateTime aujourdhui = DateTime.Today;
            return donnees.Consommation_journaliere
                .Where(conso => conso.DateConsommation.Date == aujourdhui)
                .ToList();
        }

        // ajoute une nouvelle consommation pour la journée
        public async Task AddConso(Consommation nouvelleConso)
        {
            // calcul d'un nouvel ID unique
            if (donnees.Consommation_journaliere.Count == 0)
                nouvelleConso.ConsommationId = 1;
            else
                nouvelleConso.ConsommationId = donnees.Consommation_journaliere
                    .Max(conso => conso.ConsommationId) + 1;

            donnees.Consommation_journaliere.Add(nouvelleConso);
            await SaveData();
        }

        // met à jour une consommation existante (modification de la quantité)
        public async Task UpdateConso(Consommation consoModifiee)
        {
            Consommation? consoExistante = donnees.Consommation_journaliere
                .FirstOrDefault(conso => conso.ConsommationId == consoModifiee.ConsommationId);

            if (consoExistante != null)
            {
                consoExistante.Quantite_g = consoModifiee.Quantite_g;
            }

            await SaveData();
        }

        // supprime une consommation par son ID
        public async Task RemoveConso(int consommationId)
        {
            Consommation? consoASupprimer = donnees.Consommation_journaliere
                .FirstOrDefault(conso => conso.ConsommationId == consommationId);

            if (consoASupprimer != null)
                donnees.Consommation_journaliere.Remove(consoASupprimer);

            await SaveData();
        }

        // BILAN JOURNALIER

        // calcule le bilan nutritionnel total de la journée
        // utile pour afficher les totaux en haut de page conso jour
        public BilanJournalier GetBilanDuJour()
        {
            BilanJournalier bilan = new BilanJournalier();
            List<Consommation> consoDuJour = GetConsommationsDuJour();

            foreach (Consommation conso in consoDuJour)
            {
                Aliment? aliment = GetAlimentById(conso.AlimentId);
                if (aliment == null) continue;

                // les valeurs nutritionnelles sont pour 100g
                double facteur = conso.Quantite_g / 100.0;

                bilan.TotalCalories += aliment.Calories * facteur;
                bilan.TotalProteines += aliment.Proteines_g * facteur;
                bilan.TotalGlucides += aliment.Glucides_g * facteur;
                bilan.TotalLipides += aliment.Lipides_g * facteur;
                bilan.TotalFibres += aliment.Fibres_g * facteur;
            }

            return bilan;
        }
    }

    // classe simple pour transporter les totaux du jour
    public class BilanJournalier
    {
        public double TotalCalories { get; set; }
        public double TotalProteines { get; set; }
        public double TotalGlucides { get; set; }
        public double TotalLipides { get; set; }
        public double TotalFibres { get; set; }
    }
}