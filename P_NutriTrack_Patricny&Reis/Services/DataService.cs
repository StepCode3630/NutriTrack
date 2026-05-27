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
    /// <summary>
    /// class pour crud des données
    /// </summary>
    public class DataService
    {
        // données globales chargées depuis le JSON
        private NutriData donnees;

        // CHARGEMENT & SAUVEGARDE DU JSON

        /// <summary>
        /// charge le JSON depuis le stockage local sinon le copie depuis l app package
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        // sauvegarde toutes les données dans le fichier JSON
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// retourne toutes les catégories
        /// </summary>
        /// <returns></returns>
        public List<Category> GetCategories()
        {
            return donnees.Categories;
        }

        // ALIMENTS

        /// <summary>
        /// retourne tous les aliments
        /// </summary>
        /// <returns></returns>
        public List<Aliment> GetAliments()
        {
            return donnees.Aliments;
        }

        /// <summary>
        /// retourne les aliments d'une catégorie
        /// </summary>
        /// <param name="categorieId"></param>
        /// <returns></returns>
        public List<Aliment> GetAlimentsByCategorie(int categorieId)
        {
            return donnees.Aliments
                .Where(aliment => aliment.CategoryFk == categorieId)
                .ToList();
        }

        /// <summary>
        /// retourne un aliment par son ID
        /// </summary>
        /// <param name="alimentId"></param>
        /// <returns></returns>
        public Aliment? GetAlimentById(int alimentId)
        {
            return donnees.Aliments.FirstOrDefault(aliment => aliment.AlimentId == alimentId);
        }

        /// <summary>
        /// ajoute un nouvel aliment
        /// </summary>
        /// <param name="nouvelAliment"></param>
        /// <returns></returns>
        public async Task AddAliment(Aliment nouvelAliment)
        {
            donnees.Aliments.Add(nouvelAliment);
            await SaveData();
        }

        /// <summary>
        /// supprime un aliment par son ID
        /// </summary>
        /// <param name="alimentId"></param>
        /// <returns></returns>
        public async Task RemoveAliment(int alimentId)
        {
            Aliment? aliment = donnees.Aliments
                .FirstOrDefault(item => item.AlimentId == alimentId);

            if (aliment != null)
                donnees.Aliments.Remove(aliment);

            await SaveData();
        }

        /// <summary>
        /// met à jour un aliment existant
        /// </summary>
        /// <param name="alimentModifie"></param>
        /// <returns></returns>
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


        /// <summary>
        /// retourne toutes les consommations du jour
        /// </summary>
        /// <returns>La liste de la consommation du jour</returns>
        public List<Consommation> GetConsommationsDuJour()
        {
            DateTime aujourdhui = DateTime.Today;
            return donnees.Consommation_journaliere
                .Where(conso => conso.DateConsommation.Date == aujourdhui)
                .ToList();
        }

        /// <summary>
        /// ajoute une nouvelle consommation pour la journée
        /// </summary>
        /// <param name="nouvelleConso"></param>
        /// <returns>nouvel aliment</returns>
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

        /// <summary>
        /// met à jour une consommation existante (modification de la quantité)
        /// </summary>
        /// <param name="consoModifiee"></param>
        /// <returns>maj une consommation</returns>
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

        /// <summary>
        /// supprime une consommation par son ID
        /// </summary>
        /// <param name="consommationId"></param>
        /// <returns>Supprime aliment de la consomation Journalière</returns>
        public async Task RemoveConso(int consommationId)
        {
            Consommation? consoASupprimer = donnees.Consommation_journaliere
                .FirstOrDefault(conso => conso.ConsommationId == consommationId);

            if (consoASupprimer != null)
                donnees.Consommation_journaliere.Remove(consoASupprimer);

            await SaveData();
        }

        // BILAN JOURNALIER

        /// <summary>
        /// calcule le bilan nutritionnel total de la journée
        /// utile pour afficher les totaux en haut de page conso jour
        /// </summary>
        /// <returns>objet bilan</returns>
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

    /// <summary>
    /// classe simple pour transporter les totaux du jour
    /// </summary>
    public class BilanJournalier
    {
        public double TotalCalories { get; set; }
        public double TotalProteines { get; set; }
        public double TotalGlucides { get; set; }
        public double TotalLipides { get; set; }
        public double TotalFibres { get; set; }
    }
}