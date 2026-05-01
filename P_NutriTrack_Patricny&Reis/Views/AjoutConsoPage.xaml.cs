using P_NutriTrack_Patricny_Reis.DataModels;
using P_NutriTrack_Patricny_Reis.Services;

namespace P_NutriTrack_Patricny_Reis.Views;

public partial class AjoutConsoPage : ContentPage
{
    private DataService dataService;
    private Aliment aliment;
    // si != null alors on est en mode modification
    private Consommation? consoExistante;

    // Constructeur ADD
    public AjoutConsoPage(Aliment alimentChoisi)
    {
        InitializeComponent();
        dataService = new DataService();
        aliment = alimentChoisi;
        consoExistante = null;

        NomAlimentLabel.Text = aliment.Name;
    }

    // Constructeur pour UPDATE
    public AjoutConsoPage(Aliment alimentChoisi, Consommation consoAModifier)
    {
        InitializeComponent();
        dataService = new DataService();
        aliment = alimentChoisi;
        consoExistante = consoAModifier;

        NomAlimentLabel.Text = aliment.Name;
        QuantiteEntry.Text = consoAModifier.Quantite_g.ToString();
        // Le TextChanged se déclenche automatiquement et fait les calculs
    }

    private void OnQuantiteChanged(object sender, TextChangedEventArgs e)
    {
        double quantite = ConvertirEnDouble(e.NewTextValue);
        double facteur = quantite / 100.0;

        CalCalcLabel.Text = $"{aliment.Calories * facteur:F2}";
        ProtCalcLabel.Text = $"{aliment.Proteines_g * facteur:F2}";
        GlucCalcLabel.Text = $"{aliment.Glucides_g * facteur:F2}";
    }

    private async void OnAjouterClicked(object sender, EventArgs e)
    {
        double quantite = ConvertirEnDouble(QuantiteEntry.Text);
        if (quantite <= 0)
        {
            await DisplayAlert("Erreur", "Saisis une quantité valide", "OK");
            return;
        }

        await dataService.LoadData();

        if (consoExistante == null)
        {
            // CREATE
            Consommation nouvelle = new Consommation
            {
                AlimentFk = aliment.AlimentId,
                Quantite_g = quantite,
                DateConsommation = DateTime.Now
            };
            dataService.addConso(nouvelle);
        }
        else
        {
            // UPDATE
            consoExistante.Quantite_g = quantite;
            dataService.updateConso(consoExistante);
        }

        // retourn direct a la page conso jour
        if (consoExistante == null)
        {
            // fais 2x le retour pour aller a la bonne page
            Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 2]);
        }
        await Navigation.PopAsync();
    }

    private async void OnAnnulerClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private double ConvertirEnDouble(string? texte)
    {
        if (double.TryParse(texte, out double resultat))
            return resultat;
        return 0;
    }
}