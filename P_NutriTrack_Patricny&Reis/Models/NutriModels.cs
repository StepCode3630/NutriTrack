using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P_NutriTrack_Patricny_Reis.DataModels
{
    // ---------------- CATEGORY ----------------
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
    }

    // ---------------- ALIMENT ----------------
    public class Aliment
    {
        public int AlimentId { get; set; }
        public string Name { get; set; }

        public int Calories { get; set; }
        public double Proteines_g { get; set; }
        public double Glucides_g { get; set; }
        public double Lipides_g { get; set; }
        public double Fibres_g { get; set; }

        public int CategoryFk { get; set; }

        // Navigation (optionnel mais utile)
        public Category Category { get; set; }
        public List<string> Mineraux { get; set; }
        public List<string> Vitamines { get; set; }
    }

    // ---------------- MINERAU ----------------
    public class Minerau
    {
        public int MinerauxId { get; set; }
        public string Name { get; set; }

        public List<AlimentMinerau> Aliments { get; set; }
    }

    // ---------------- VITAMINE ----------------
    public class Vitamine
    {
        public int VitaminesId { get; set; }
        public string Name { get; set; }

        public List<AlimentVitamine> Aliments { get; set; }
    }

    // ---------------- CONSOMMATION ----------------
    public class Consommation
    {
        public int AlimentId { get; set; }
        public double Quantite_g { get; set; }

        // Champs internes utilisés par l'app, pas dans le JSON
        // [JsonIgnore] = "ne pas chercher cette propriété dans le JSON"
        [System.Text.Json.Serialization.JsonIgnore]
        public int ConsommationId { get; set; }

        //[System.Text.Json.Serialization.JsonIgnore]
        public DateTime DateConsommation { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public Aliment? Aliment { get; set; }
    }

    // ---------------- RELATION ALIMENT - MINERAUX ----------------
    public class AlimentMinerau
    {
        public int AlimentFk { get; set; }
        public int MinerauxFk { get; set; }

        public int Quantitee_mg { get; set; }

        // Navigation
        public Aliment Aliment { get; set; }
        public Minerau Minerau { get; set; }
    }

    // ---------------- RELATION ALIMENT - VITAMINES ----------------
    public class AlimentVitamine
    {
        public int AlimentFk { get; set; }
        public int VitaminesFk { get; set; }

        public int Quantitee_mg { get; set; }

        // Navigation
        public Aliment Aliment { get; set; }
        public Vitamine Vitamine { get; set; }
    }

    // ---------------- ROOT DATA pour JSON ----------------
    public class NutriData
    {
        public List<Category> Categories { get; set; }
        public List<Aliment> Aliments { get; set; }
        public List<Minerau> Mineraux { get; set; }
        public List<Vitamine> Vitamines { get; set; }
        public List<Consommation> Consommation_journaliere { get; set; }
        public List<AlimentMinerau> AlimentMineraux { get; set; }
        public List<AlimentVitamine> AlimentVitamines { get; set; }
    }
}
