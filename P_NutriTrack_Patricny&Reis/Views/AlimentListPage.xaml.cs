/******************************************************************************
** PROGRAMME  AlimentListPage.xaml.cs                                        **
**                                                                           **
** Lieu      : ETML - section informatique                                   **
** Auteur    : Camille Rais                                                  **
** Date      : 01.04.2026                                                    **
**                                                                           **
******************************************************************************/

/******************************************************************************
** DESCRIPTION                                                               **
**                                                                           **
** Page qui affiche la liste des aliments d'une catégorie                    **
** Permet d'ajouter, modifier ou supprimer un aliment                        **
**                                                                           **
******************************************************************************/

using P_NutriTrack_Patricny_Reis.DataModels;
using P_NutriTrack_Patricny_Reis.Services;

namespace P_NutriTrack_Patricny_Reis.Views;

public partial class AlimentListPage : ContentPage
{
    private DataService dataService;
    private List<Aliment> aliments = null!;
    private Category categorie;

    // constructeur qui reçoit la catégorie sélectionnée
    public AlimentListPage(Category categorieRecue)
    {
        InitializeComponent();
        categorie = categorieRecue;
        dataService = new DataService();
        TitreLabel.Text = categorieRecue.Name;
    }

    // refresh la liste à chaque retour sur la page
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await dataService.LoadData();
        aliments = dataService.GetAlimentsByCategorie(categorie.CategoryId);
        AlimentListView.ItemsSource = aliments;
    }

    // bouton ajouter un nouvel aliment
    private async void OnAddClicked(object sender, EventArgs eventArgs)
    {
        await Navigation.PushAsync(new AddEditAlimentPage(categorie));
    }

    // bouton modifier un aliment
    private async void OnEditClicked(object sender, EventArgs eventArgs)
    {
        Button bouton = (Button)sender;
        Aliment alimentSelectionne = (Aliment)bouton.CommandParameter;
        await Navigation.PushAsync(new AddEditAlimentPage(categorie, alimentSelectionne));
    }

    // bouton supprimer un aliment
    private async void OnDeleteClicked(object sender, EventArgs eventArgs)
    {
        Button bouton = (Button)sender;
        Aliment alimentASupprimer = (Aliment)bouton.CommandParameter;

        string message = $"Supprimer {alimentASupprimer.Name} ?";
        bool confirmation = await DisplayAlert("Supprimer", message, "Oui", "Non");

        if (!confirmation) return;

        await dataService.RemoveAliment(alimentASupprimer.AlimentId);

        // refresh la liste affichée
        aliments = dataService.GetAlimentsByCategorie(categorie.CategoryId);
        AlimentListView.ItemsSource = aliments;
    }

    // clic sur un aliment dans la liste : ouvre la page de détail
    private async void OnAlimentSelected(object sender, SelectionChangedEventArgs eventArgs)
    {
        Aliment? alimentSelectionne = (Aliment?)eventArgs.CurrentSelection.FirstOrDefault();
        if (alimentSelectionne == null) return;

        // déselectionne pour pouvoir recliquer dessus plus tard
        AlimentListView.SelectedItem = null;
        await Navigation.PushAsync(new AlimentDetailPage(categorie, alimentSelectionne));
    }
}