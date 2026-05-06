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
        await dataService.Init();
        // récup la liste des catégories
        Task<List<Category>> categories = dataService.GetCategories();
        CategoryListView.ItemsSource = (System.Collections.IEnumerable)categories;
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
}