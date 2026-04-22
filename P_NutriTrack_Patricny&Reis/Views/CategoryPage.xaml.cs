// using permet d'utiliser des classes définies ailleurs dans le projet
using P_NutriTrack_Patricny_Reis.DataModels;
using P_NutriTrack_Patricny_Reis.Services;
// adresse de la classe dans le projet
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
    private async void OnCategoryTapped(object sender, ItemTappedEventArgs eventArgs)
    {
        // récup la categorie cliquée
        Category categorieSelectionnee = (Category)eventArgs.Item;
        // ouvre AlimentListPage en lui donnant la catégorie
        await Navigation.PushAsync(new AlimentListPage(categorieSelectionnee));
    }
}