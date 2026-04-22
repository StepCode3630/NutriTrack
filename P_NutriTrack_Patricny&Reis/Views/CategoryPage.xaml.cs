// using permet d'utiliser des classes définies ailleurs dans le projet
using P_NutriTrack_Patricny_Reis.DataModels;
using P_NutriTrack_Patricny_Reis.Services;

namespace P_NutriTrack_Patricny_Reis.Views;
public partial class CategoryPage : ContentPage
{
    // variables privées
    private DataService dataService;
    private List<Category> categories;
    public CategoryPage()
    {
        InitializeComponent();
        dataService = new DataService();
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        NutriData donnees = await dataService.LoadAsync();
        categories = donnees.Categories;
        CategoryListView.ItemsSource = categories;
    }
    private async void OnAddClicked(object sender, EventArgs eventArgs)
    {
        string nomCategorie = await DisplayPromptAsync(
            "Nouvelle catégorie",
            "Entrez le nom :");
        if (string.IsNullOrWhiteSpace(nomCategorie))
            return;
        Category nouvelleCategorie = new Category();
        nouvelleCategorie.Name = nomCategorie;
        if (categories.Count == 0)
            nouvelleCategorie.CategoryId = 1;
        else
            nouvelleCategorie.CategoryId = categories.Max(categorie => categorie.CategoryId) + 1;
        categories.Add(nouvelleCategorie);
        await dataService.SaveAsync();
        CategoryListView.ItemsSource = null;
        CategoryListView.ItemsSource = categories;
    }
    // appel quand on clique sur une catégorie dans liste
    private async void OnCategoryTapped(object sender, ItemTappedEventArgs eventArgs)
    {
        // récup la categorie cliquée
        Category categorieSelectionnee = (Category)eventArgs.Item;
        // ouvre AlimentListPage en lui donnant la catégorie
        await Navigation.PushAsync(new AlimentListPage(categorieSelectionnee));
    }
}