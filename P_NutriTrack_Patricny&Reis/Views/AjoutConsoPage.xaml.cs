/******************************************************************************
** PROGRAMME  AjoutConsoPage.xaml.cs                                         **
**                                                                           **
** Lieu      : ETML - section informatique                                   **
** Auteur    : Camille Rais                                                  **
** Date      : 01.04.2026                                                    **
**                                                                           **
******************************************************************************/

/******************************************************************************
** DESCRIPTION                                                               **
**                                                                           **
** Page d'ajout / modification d'une consommation pour la journée            **
** L'utilisateur saisit une quantité en g et voit le calcul nutritionnel     **
**                                                                           **
******************************************************************************/

using P_NutriTrack_Patricny_Reis.DataModels;
using P_NutriTrack_Patricny_Reis.Services;

namespace P_NutriTrack_Patricny_Reis.Views;

public partial class AjoutConsoPage : ContentPage
{
    private DataService dataService;
    private Aliment alimentChoisi;

    // si != null alors on est en mode modification
    private Consommation? consoExistante;

    // constructeur pour ajout
    public AjoutConsoPage(Aliment alimentRecu)
    {
        InitializeComponent();
        dataService = new DataService();
        alimentChoisi = alimentRecu;
        consoExistante = null;

        NomAlimentLabel.Text = alimentChoisi.Name;
    }

    // constructeur pour modification
    public AjoutConsoPage(Aliment alimentRecu, Consommation consoAModifier)
    {
        InitializeComponent();
        dataService = new DataService();
        alimentChoisi = alimentRecu;
        consoExistante = consoAModifier;

        NomAlimentLabel.Text = alimentChoisi.Name;
        QuantiteEntry.Text = consoAModifier.Quantite_g.ToString();
        // le TextChanged se déclenche tout seul et fait les calculs
    }

    // calcule en temps réel les valeurs nutritionnelles selon la quantité saisie
    private void OnQuantiteChanged(object sender, TextChangedEventArgs eventArgs)
    {
        double quantite = ConvertirEnDouble(eventArgs.NewTextValue);
        // les valeurs nutritionnelles dans le JSON sont pour 100g
        double facteur = quantite / 100.0;

        CalCalcLabel.Text = $"{alimentChoisi.Calories * facteur:F2}";
        ProtCalcLabel.Text = $"{alimentChoisi.Proteines_g * facteur:F2}";
        GlucCalcLabel.Text = $"{alimentChoisi.Glucides_g * facteur:F2}";
    }

    // valide et enregistre la consommation
    private async void OnAjouterClicked(object sender, EventArgs eventArgs)
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
            // mode ajout
            Consommation nouvelleConso = new Consommation
            {
                AlimentId = alimentChoisi.AlimentId,
                Quantite_g = quantite,
                DateConsommation = DateTime.Now
            };
            await dataService.AddConso(nouvelleConso);
        }
        else
        {
            // mode modification
            consoExistante.Quantite_g = quantite;
            await dataService.UpdateConso(consoExistante);
        }

        // si on était en mode ajout on saute la page de selection d'aliment
        // pour revenir directement sur la page conso du jour
        if (consoExistante == null && Navigation.NavigationStack.Count >= 2)
        {
            Page pageSelection = Navigation.NavigationStack[Navigation.NavigationStack.Count - 2];
            Navigation.RemovePage(pageSelection);
        }

        await Navigation.PopAsync();
    }

    // bouton annuler
    private async void OnAnnulerClicked(object sender, EventArgs eventArgs)
    {
        await Navigation.PopAsync();
    }

    // méthode utilitaire pour convertir un texte en nombre
    private double ConvertirEnDouble(string? texte)
    {
        if (double.TryParse(texte, out double resultat))
            return resultat;
        return 0;
    }
}