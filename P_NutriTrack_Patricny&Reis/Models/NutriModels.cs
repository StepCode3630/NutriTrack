using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using SQLitePCL;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace P_NutriTrack_Patricny_Reis.DataModels
{
    // ---------------- CATEGORY ----------------
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        public string Name { get; set; }
    }

    // ---------------- ALIMENT ----------------
    public class Aliment
    {
        [Key]
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
        [Key]
        public int MinerauxId { get; set; }
        public string Name { get; set; }

        public List<AlimentMinerau> Aliments { get; set; }
    }

    // ---------------- VITAMINE ----------------
    public class Vitamine
    {
        [Key]
        public int VitaminesId { get; set; }
        public string Name { get; set; }

        public List<AlimentVitamine> Aliments { get; set; }
    }

    // ---------------- CONSOMMATION ----------------
    public class Consommation
    {
        [Key]
        public int ConsommationId { get; set; }
        public double Quantite_g { get; set; }
        public DateTime DateConsommation { get; set; }

        public int AlimentFk { get; set; }

        // Navigation
        public Aliment Aliment { get; set; }
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

    // ---------------- ROOT DATA (pour JSON) ----------------
    public class NutriData
    {
        public List<Category> Categories { get; set; }
        public List<Aliment> Aliments { get; set; }
        public List<Minerau> Mineraux { get; set; }
        public List<Vitamine> Vitamines { get; set; }
        public List<Consommation> Consommations { get; set; }
        public List<AlimentMinerau> AlimentMineraux { get; set; }
        public List<AlimentVitamine> AlimentVitamines { get; set; }
    }
}
