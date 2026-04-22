using P_NutriTrack_Patricny_Reis.DataModels;
using P_NutriTrack_Patricny_Reis.Services;
namespace P_NutriTrack_Patricny_Reis.Views;
public partial class AlimentListPage : ContentPage
{
    private DataService dataService;
    private List<Aliment> aliments;
    // category recu de categoryPage
    private Category categorie;
    // constructeur reçoit la catégorie à afficher
    public AlimentListPage(Category categorieRecue)
    {
        InitializeComponent();
        categorie = categorieRecue;
        dataService = new DataService();
        // met le nom de la catégorie comme titre
        TitreLabel.Text = categorieRecue.Name;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // charge les donnees
        NutriData donnees = await dataService.LoadAsync();
        // filtrer :garde que les aliments de la catégorie choisie
        aliments = donnees.Aliments
                          .Where(aliment => aliment.CategoryFk == categorie.CategoryId)
                          .ToList();
        // afficher la liste
        AlimentListView.ItemsSource = aliments;
    }
    private async void OnAddClicked(object sender, EventArgs eventArgs)
    {
        // affiche juste un message pour l instant
        // connecter à AddEditAlimentPage quand la page sera la
        await DisplayAlert("Info", "Page d'ajout à venir", "OK");
    }
}