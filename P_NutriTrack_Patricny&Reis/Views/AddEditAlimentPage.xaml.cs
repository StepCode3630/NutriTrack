using P_NutriTrack_Patricny_Reis.DataModels;
using P_NutriTrack_Patricny_Reis.Services;
namespace P_NutriTrack_Patricny_Reis.Views;
public partial class AddEditAlimentPage : ContentPage
{
    private DataService dataService;
    // choix categorie pour add aliment
    private Category categorie;
    // aliment à modifier  devient null si c est un ajout
    private Aliment alimentAModifier;
    // constructeur pour add aliment / pas d'aliment qui existe deja
    public AddEditAlimentPage(Category categorieRecue)
    {
        InitializeComponent();
        categorie = categorieRecue;
        dataService = new DataService();
        alimentAModifier = null;
        TitreLabel.Text = "Ajouter un aliment";
    }
    // constructeur pour modif aliment existant
    public AddEditAlimentPage(Category categorieRecue, Aliment alimentRecu)
    {
        InitializeComponent();
        categorie = categorieRecue;
        dataService = new DataService();
        alimentAModifier = alimentRecu;
        TitreLabel.Text = "Modifier un aliment";
        // pré-remplir form avec valeurs de l aliment
        NomEntry.Text = alimentRecu.Name;
        CaloriesEntry.Text = alimentRecu.Calories.ToString();
        ProteinesEntry.Text = alimentRecu.Proteines_g.ToString();
        GlucidesEntry.Text = alimentRecu.Glucides_g.ToString();
        LipidesEntry.Text = alimentRecu.Lipides_g.ToString();
        FibresEntry.Text = alimentRecu.Fibres_g.ToString();
    }
    private async void OnSaveClicked(object sender, EventArgs eventArgs)
    {
        // vérifie que nom n est pas vide
        if (string.IsNullOrWhiteSpace(NomEntry.Text))
        {
            await DisplayAlert("Erreur", "Le nom est obligatoire", "OK");
            return;
        }
        // charger les données
        await dataService.LoadData();
        List<Aliment> tousLesAliments = dataService.GetAliments();
        if (alimentAModifier == null)
        {
            // pour quand on ajoute
            Aliment nouvelAliment = new Aliment();
            // calculer new Id unique
            if (tousLesAliments.Count == 0)
                nouvelAliment.AlimentId = 1;
            else
                nouvelAliment.AlimentId = tousLesAliments.Max(aliment => aliment.AlimentId) + 1;
            // remplir valeurs avec ce qui a été saisi
            nouvelAliment.Name = NomEntry.Text;
            nouvelAliment.Calories = ConvertirEnInt(CaloriesEntry.Text);
            nouvelAliment.Proteines_g = ConvertirEnDouble(ProteinesEntry.Text);
            nouvelAliment.Glucides_g = ConvertirEnDouble(GlucidesEntry.Text);
            nouvelAliment.Lipides_g = ConvertirEnDouble(LipidesEntry.Text);
            nouvelAliment.Fibres_g = ConvertirEnDouble(FibresEntry.Text);
            nouvelAliment.CategoryFk = categorie.CategoryId;
            dataService.addAliment(nouvelAliment);
        }
        else
        {
            // pour les modif
            Aliment alimentModifie = new Aliment();
            alimentModifie.AlimentId = alimentAModifier.AlimentId;
            alimentModifie.Name = NomEntry.Text;
            alimentModifie.Calories = ConvertirEnInt(CaloriesEntry.Text);
            alimentModifie.Proteines_g = ConvertirEnDouble(ProteinesEntry.Text);
            alimentModifie.Glucides_g = ConvertirEnDouble(GlucidesEntry.Text);
            alimentModifie.Lipides_g = ConvertirEnDouble(LipidesEntry.Text);
            alimentModifie.Fibres_g = ConvertirEnDouble(FibresEntry.Text);
            dataService.updateAliment(alimentModifie);
        }
        // sauvegarde dans le JSON
        // A FAIRE PLUS TARD activer quand SaveData sera dispo dans DataService
        // retour page d'avant
        await Navigation.PopAsync();
    }
    // méthode pour convertir le texte en nbr
    private int ConvertirEnInt(string texte)
    {
        if (int.TryParse(texte, out int resultat))
            return resultat;
        return 0;
    }
    private double ConvertirEnDouble(string texte)
    {
        if (double.TryParse(texte, out double resultat))
            return resultat;
        return 0;
    }
}