using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P_NutriTrack_Patricny_Reis.DataModels
{
    internal class NutriModels
    {
    }
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Aliment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public double Calories { get; set; }
        public double Proteines_g { get; set; }
        public double Glucides_g { get; set; }
        public double Lipides_g { get; set; }
        public double Fibres_g { get; set; }
        public List<string> Vitamines { get; set; }
    }

    public class Consommation
    {
        public int AlimentId { get; set; }
        public double Quantite_g { get; set; }
    }

    public class NutriData
    {
        public List<Category> Categories { get; set; }
        public List<Aliment> Aliments { get; set; }
        public List<Consommation> Consommation_journaliere { get; set; }
    }
}
