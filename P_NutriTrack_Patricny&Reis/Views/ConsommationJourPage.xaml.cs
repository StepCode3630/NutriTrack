/******************************************************************************
** PROGRAMME  ConsommationJourPage.xaml.cs                                   **
**                                                                           **
** Lieu      : ETML - section informatique                                   **
** Auteur    : Camille Rais                                                  **
** Date      : 01.04.2026                                                    **
**                                                                           **
******************************************************************************/

/******************************************************************************
** DESCRIPTION                                                               **
**                                                                           **
** Page qui affiche le bilan journalier de l'utilisateur                     **
** Liste les aliments consommés et calcule les totaux nutritionnels          **
**                                                                           **
******************************************************************************/

using P_NutriTrack_Patricny_Reis.DataModels;
using P_NutriTrack_Patricny_Reis.Services;

namespace P_NutriTrack_Patricny_Reis.Views;

public partial class ConsommationJourPage : ContentPage
{

    // retour à la page d'accueil depuis la maison
    private async void OnHomeClicked(object sender, EventArgs eventArgs)
    {
        await Navigation.PopToRootAsync();
    }


    private DataService dataService;
    // liste affichée à l'écran avec les infos enrichies
    private List<ConsoAffichage> consoAffichees = null!;

    public ConsommationJourPage()
    {
        InitializeComponent();
        //Sqlite
        dataService = DataService.Instance;
        DateLabel.Text = DateTime.Today.ToString("dddd dd MMMM");
    }

    // refresh les données à chaque retour sur la page
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        AfficherConsommations();
    }

    // construit la liste affichée et calcule les totaux du jour
    private async void AfficherConsommations()
    {
        List<Consommation> consoDuJour = await dataService.GetConsommationsDuJour();

        consoAffichees = new List<ConsoAffichage>();

        foreach (Consommation conso in consoDuJour)
        {
            Aliment? aliment = await dataService.GetAlimentById(conso.AlimentId);
            if (aliment == null) continue;

            // calcul nutritionnel selon la quantité (valeurs JSON pour 100g)
            double facteur = conso.Quantite_g / 100.0;
            double calories = aliment.Calories * facteur;

            consoAffichees.Add(new ConsoAffichage
            {
                ConsommationId = conso.ConsommationId,
                NomAliment = aliment.Name,
                InfoLigne = $"{conso.Quantite_g}g - {calories:F0} Kcal"
            });
        }

        ConsoListView.ItemsSource = consoAffichees;

        // récupère le bilan global du jour calculé par le DataService
        BilanJournalier bilan = await dataService.GetBilanDuJour();
        TotalCaloriesLabel.Text = $"{bilan.TotalCalories:F0}";
        TotalProteinesLabel.Text = $"{bilan.TotalProteines:F1}";
        TotalGlucidesLabel.Text = $"{bilan.TotalGlucides:F1}";
        TotalLipidesLabel.Text = $"{bilan.TotalLipides:F1}";
    }

    // bouton ajouter une consommation
    private async void OnAddClicked(object sender, EventArgs eventArgs)
    {
        await Navigation.PushAsync(new SelectionAlimentPage());
    }

    // bouton modifier une consommation existante
    private async void OnEditClicked(object sender, EventArgs eventArgs)
    {
        Button bouton = (Button)sender;
        int consoId = (int)bouton.CommandParameter;

        // retrouve la conso et l'aliment correspondant
        List<Consommation> consommations =
            await dataService.GetConsommationsDuJour();

        Consommation? conso = consommations
            .FirstOrDefault(item => item.ConsommationId == consoId);

        if (conso == null)
            return;

        Aliment? aliment = await dataService.GetAlimentById(conso.AlimentId);
        if (aliment == null) return;

        // ouvre la page d'ajout en mode édition
        await Navigation.PushAsync(new AjoutConsoPage(aliment, conso));
    }

    // bouton supprimer une consommation
    private async void OnDeleteClicked(object sender, EventArgs eventArgs)
    {
        Button bouton = (Button)sender;
        int consoId = (int)bouton.CommandParameter;

        bool confirmation = await DisplayAlert(
            "Supprimer",
            "Retirer cet aliment de ta journée ?",
            "Oui", "Non");

        if (!confirmation) return;

        await dataService.RemoveConso(consoId);
        AfficherConsommations();
    }
}

// classe utilitaire pour l'affichage de la liste
public class ConsoAffichage
{
    public int ConsommationId { get; set; }
    public string NomAliment { get; set; } = "";
    public string InfoLigne { get; set; } = "";
}