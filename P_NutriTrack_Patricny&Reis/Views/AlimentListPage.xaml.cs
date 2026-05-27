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

    /// <summary>
    /// retour à la page d'accueil
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
    private async void OnHomeClicked(object sender, EventArgs eventArgs)
    {
        await Navigation.PopToRootAsync();
    }

    private DataService dataService;
    private List<Aliment> aliments = null!;
    private Category categorie;

    /// <summary>
    /// constructeur qui reçoit la catégorie sélectionnée
    /// </summary>
    /// <param name="categorieRecue"></param>
    public AlimentListPage(Category categorieRecue)
    {
        InitializeComponent();
        categorie = categorieRecue;
        dataService = new DataService();
        TitreLabel.Text = categorieRecue.Name;
    }

    /// <summary>
    /// refresh la liste à chaque retour sur la page
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await dataService.LoadData();
        aliments = dataService.GetAlimentsByCategorie(categorie.CategoryId);
        AlimentListView.ItemsSource = aliments;
    }

    /// <summary>
    /// bouton ajouter un nouvel aliment
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
    private async void OnAddClicked(object sender, EventArgs eventArgs)
    {
        await Navigation.PushAsync(new AddEditAlimentPage(categorie));
    }

    /// <summary>
    /// bouton modifier un aliment
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
    private async void OnEditClicked(object sender, EventArgs eventArgs)
    {
        Button bouton = (Button)sender;
        Aliment alimentSelectionne = (Aliment)bouton.CommandParameter;
        await Navigation.PushAsync(new AddEditAlimentPage(categorie, alimentSelectionne));
    }

    /// <summary>
    /// bouton supprimer un aliment
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
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

    /// <summary>
    /// clic sur un aliment dans la liste : ouvre la page de détail
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
    private async void OnAlimentSelected(object sender, SelectionChangedEventArgs eventArgs)
    {
        Aliment? alimentSelectionne = (Aliment?)eventArgs.CurrentSelection.FirstOrDefault();
        if (alimentSelectionne == null) return;

        // déselectionne pour pouvoir recliquer dessus plus tard
        AlimentListView.SelectedItem = null;
        await Navigation.PushAsync(new AlimentDetailPage(categorie, alimentSelectionne));
    }
}