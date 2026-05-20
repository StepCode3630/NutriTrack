
/******************************************************************************
** PROGRAMME  *.cs                                                           **
**                                                                           **
** Lieu      : ETML - section informatique                                   **
** Auteur    : Camille Rais                                                  **
** Date      : 01.04.2026                                                    **
**                                                                           **
** Modifications                                                             **
**   Auteur  :                                                               **
**   Version :                                                               **
**   Date    :                                                               **
**   Raisons :                                                               **
**                                                                           **
**                                                                           **
******************************************************************************/

/******************************************************************************
** PAGE D'ACCEUIL DE L'APP                                                   **
** VISUEL DIRECTE SUR LES CATéGORIE DISPO                                    **     
** VISUEL SUR "MA CONSO DU JOUR"                                             **
**                                                                           **
******************************************************************************/
using P_NutriTrack_Patricny_Reis.DataModels;
using P_NutriTrack_Patricny_Reis.Services;

namespace P_NutriTrack_Patricny_Reis.Views;

public partial class CategoryPage : ContentPage
{
    private DataService dataService;
    private List<Category> categories = null!;

    public CategoryPage()
    {
        InitializeComponent();
        dataService = new DataService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // charge données depuis JSON
        await dataService.LoadData();
        categories = dataService.GetCategories();

        // Liste déroulante avec grille partage même source
        CategoryListView.ItemsSource = categories;
        CategoryGridView.ItemsSource = categories;
        // affiche le bilan du jour sur page d accueil dans 4mini cartes
        BilanJournalier bilan = dataService.GetBilanDuJour();
        AccueilCaloriesLabel.Text = $"{bilan.TotalCalories:F0}";
        AccueilProteinesLabel.Text = $"{bilan.TotalProteines:F1}";
        AccueilGlucidesLabel.Text = $"{bilan.TotalGlucides:F1}";
        AccueilLipidesLabel.Text = $"{bilan.TotalLipides:F1}";
    }

    // clique  sur une catégorie depuis liste ou depuis grille
    private async void OnCategorySelected(object sender, SelectionChangedEventArgs eventArgs)
    {
        Category? categorieSelectionnee = (Category?)eventArgs.CurrentSelection.FirstOrDefault();
        if (categorieSelectionnee == null) return;

        // se deselectionne pour qu on puisse cliquer dessus encore après
        CollectionView source = (CollectionView)sender;
        source.SelectedItem = null;

        // si liste déroulante ouverte alors se ferme
        if (CategoryListView.IsVisible)
        {
            CategoryListView.IsVisible = false;
            FlecheLabel.Text = "↓";
        }

        // Ouvre la page des aliments
        await Navigation.PushAsync(new AlimentListPage(categorieSelectionnee));
    }

    // Clic sur Category list cela replie ou déplie la liste
    private void OnToggleListClicked(object sender, TappedEventArgs eventArgs)
    {
        CategoryListView.IsVisible = !CategoryListView.IsVisible;
        FlecheLabel.Text = CategoryListView.IsVisible ? "↑" : "↓";
    }

    // Clic sur la carte "Consommation journalière" (action à définir plus tard)
    private async void OnConsommationClicked(object sender, TappedEventArgs eventArgs)
    {
        await Navigation.PushAsync(new ConsommationJourPage());
    }
}
