// using permet d'utiliser des classes définies ailleurs dans le projet
using P_NutriTrack_Patricny_Reis.DataModels;
using P_NutriTrack_Patricny_Reis.Services;
// adresse de la classe dans le projet

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
** DESCRIPTION                                                               **
**                                                                           **     
**                                                                           **
**                                                                           **
******************************************************************************/


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
        // charge les données depuis le JSON
        await dataService.LoadData();
        // récup la liste des catégories
        categories = dataService.GetCategories();
        CategoryListView.ItemsSource = categories;
    }
    // appel quand on clique sur une catégorie dans liste
    private async void OnCategorySelected(object sender, SelectionChangedEventArgs eventArgs)
    {
        // récup la catégorie cliquée
        Category categorieSelectionnee = (Category)eventArgs.CurrentSelection.FirstOrDefault();
        if (categorieSelectionnee == null)
            return;
        // déselectionne pour pouvoir recliquer dessus qund on revient dessus
        CategoryListView.SelectedItem = null;
        // ouvre AlimentListPage en lui donnant la catégorie
        await Navigation.PushAsync(new AlimentListPage(categorieSelectionnee));
    }

    // affiche ou cache la liste quand on clique sur "Category list"
    private void OnToggleListClicked(object sender, TappedEventArgs eventArgs)
    {
        // inverse la visibilité de la liste
        CategoryListView.IsVisible = !CategoryListView.IsVisible;

        // change la flèche selon l état
        if (CategoryListView.IsVisible)
            FlecheLabel.Text = "↑";
        else
            FlecheLabel.Text = "↓";
    }
}