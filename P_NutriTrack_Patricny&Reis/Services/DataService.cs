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
using P_NutriTrack_Patricny_Reis.Data;

namespace P_NutriTrack_Patricny_Reis.Services
{
    public class DataService
    {
        // Instance statique directement créée
        private static DataService? _instance;

        public static DataService Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new DataService();

                return _instance;
            }
        }

        // CONTEXTE SQLITE
        private readonly AppDbContext _db;

        // CONSTRUCTEUR
        private DataService()
        {
            string dbPath = Path.Combine(
                FileSystem.AppDataDirectory,
                "nutri.db");

            _db = new AppDbContext(dbPath);

            // crée automatiquement la DB et les tables
            _db.Database.EnsureCreated();
        }

        // CATEGORIES

        // retourne toutes les catégories
        public async Task<List<Category>> GetCategories()
        {
            return await _db.Categories.ToListAsync();
        }

        // ALIMENTS

        // retourne tous les aliments
        public async Task<List<Aliment>> GetAliments()
        {
            return await _db.Aliments.ToListAsync();
        }

        // retourne les aliments d'une catégorie
        public async Task<List<Aliment>> GetAlimentsByCategorie(int categorieId)
        {
            return await _db.Aliments
                .Where(aliment => aliment.CategoryFk == categorieId)
                .ToListAsync();
        }

        // retourne un aliment par son ID
        public async Task<Aliment?> GetAlimentById(int alimentId)
        {
            return await _db.Aliments
                .FirstOrDefaultAsync(aliment => aliment.AlimentId == alimentId);
        }

        // ajoute un nouvel aliment
        public async Task AddAliment(Aliment nouvelAliment)
        {
            _db.Aliments.Add(nouvelAliment);

            await _db.SaveChangesAsync();
        }

        // supprime un aliment
        public async Task RemoveAliment(int alimentId)
        {
            Aliment? aliment = await _db.Aliments
                .FirstOrDefaultAsync(item => item.AlimentId == alimentId);

            if (aliment != null)
            {
                _db.Aliments.Remove(aliment);

                await _db.SaveChangesAsync();
            }
        }

        // met à jour un aliment
        public async Task UpdateAliment(Aliment alimentModifie)
        {
            Aliment? aliment = await _db.Aliments
                .FirstOrDefaultAsync(item => item.AlimentId == alimentModifie.AlimentId);

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

                await _db.SaveChangesAsync();
            }
        }

        // CONSOMMATIONS JOURNALIERES

        // retourne toutes les consommations du jour
        public async Task<List<Consommation>> GetConsommationsDuJour()
        {
            DateTime aujourdhui = DateTime.Today;

            return await _db.Consommations
                .Where(conso => conso.DateConsommation.Date == aujourdhui)
                .ToListAsync();
        }

        // ajoute une consommation
        public async Task AddConso(Consommation nouvelleConso)
        {
            _db.Consommations.Add(nouvelleConso);

            await _db.SaveChangesAsync();
        }

        // met à jour une consommation
        public async Task UpdateConso(Consommation consoModifiee)
        {
            Consommation? consoExistante = await _db.Consommations
                .FirstOrDefaultAsync(conso =>
                    conso.ConsommationId == consoModifiee.ConsommationId);

            if (consoExistante != null)
            {
                consoExistante.Quantite_g = consoModifiee.Quantite_g;

                await _db.SaveChangesAsync();
            }
        }

        // supprime une consommation
        public async Task RemoveConso(int consommationId)
        {
            Consommation? consoASupprimer = await _db.Consommations
                .FirstOrDefaultAsync(conso =>
                    conso.ConsommationId == consommationId);

            if (consoASupprimer != null)
            {
                _db.Consommations.Remove(consoASupprimer);

                await _db.SaveChangesAsync();
            }
        }

        // BILAN JOURNALIER

        // calcule le bilan nutritionnel total de la journée
        public async Task<BilanJournalier> GetBilanDuJour()
        {
            BilanJournalier bilan = new BilanJournalier();

            List<Consommation> consoDuJour = await GetConsommationsDuJour();

            foreach (Consommation conso in consoDuJour)
            {
                Aliment? aliment = await GetAlimentById(conso.AlimentId);

                if (aliment == null)
                    continue;

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

    // classe pour transporter les totaux du jour
    public class BilanJournalier
    {
        public double TotalCalories { get; set; }
        public double TotalProteines { get; set; }
        public double TotalGlucides { get; set; }
        public double TotalLipides { get; set; }
        public double TotalFibres { get; set; }
    }
}