/******************************************************************************
** PROGRAMME  AlimentDetailPage.xaml.cs                                      **
**                                                                           **
** Lieu      : ETML - section informatique                                   **
** Auteur    : Camille Rais                                                  **
** Date      : 01.04.2026                                                    **
**                                                                           **
******************************************************************************/

/******************************************************************************
** DESCRIPTION                                                               **
**                                                                           **
** Page qui affiche le détail nutritionnel d'un aliment                      **
** Permet de modifier ou supprimer l'aliment                                 **
**                                                                           **
******************************************************************************/

using P_NutriTrack_Patricny_Reis.DataModels;
using P_NutriTrack_Patricny_Reis.Services;

namespace P_NutriTrack_Patricny_Reis.Views;

public partial class AlimentDetailPage : ContentPage
{
    private DataService dataService;
    // catégorie de l'aliment comme ca on peut modifier
    private Category categorie;
    // l'aliment affiché
    private Aliment aliment;

    // constructeur recoit catégorie et aliment à afficher
    public AlimentDetailPage(Category categorieRecue, Aliment alimentRecu)
    {
        InitializeComponent();
        categorie = categorieRecue;
        aliment = alimentRecu;
        dataService = new DataService();

        // remplit les labels avec les valeurs de l'aliment
        AfficherDetails();
    }

    // refresh quand on revient sur la page apres qu on ait modifier
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // recharge données pour avoir les valeurs à jour
        await dataService.LoadData();
        // refind l aliment pour avoir la version maj
        List<Aliment> tousLesAliments = dataService.GetAliments();
        Aliment? alimentMaj = tousLesAliments.FirstOrDefault(a => a.AlimentId == aliment.AlimentId);
        if (alimentMaj != null)
        {
            aliment = alimentMaj;
            AfficherDetails();
        }
    }

    // affiche les valeurs nutritionnelles de l aliment dans labels
    private void AfficherDetails()
    {
        NomLabel.Text = aliment.Name;
        CaloriesLabel.Text = aliment.Calories.ToString();
        ProteinesLabel.Text = aliment.Proteines_g.ToString();
        GlucidesLabel.Text = aliment.Glucides_g.ToString();
        LipidesLabel.Text = aliment.Lipides_g.ToString();
        FibresLabel.Text = aliment.Fibres_g.ToString();
    }

    // btn modifier
    private async void OnEditClicked(object sender, EventArgs eventArgs)
    {
        // ouvre la page d edit avec l aliment en paramètre
        await Navigation.PushAsync(new AddEditAlimentPage(categorie, aliment));
    }

    // bouton supprimer
    private async void OnDeleteClicked(object sender, EventArgs eventArgs)
    {
        // demande a l user de confirmer
        string message = $"Supprimer {aliment.Name} ?";
        bool confirmation = await DisplayAlert(
            "Supprimer",
            message,
            "Oui", "Non");

        if (!confirmation)
            return;

        // supprime en passant par le DataService
        dataService.removeAliment(aliment.AlimentId);

        // retour à la page d'avant
        await Navigation.PopAsync();
    }
}