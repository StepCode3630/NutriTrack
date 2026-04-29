using P_NutriTrack_Patricny_Reis.DataModels;
using P_NutriTrack_Patricny_Reis.Services;
namespace P_NutriTrack_Patricny_Reis.Views;
public partial class AlimentListPage : ContentPage
{
    private DataService dataService;
    private List<Aliment> aliments = null!;
    // category recu de categoryPage
    private Category categorie;
    // constructeur reçoit la catégorie à afficher
    public AlimentListPage(Category categorieRecue)
    {
        InitializeComponent();
        categorie = categorieRecue;
        dataService = new DataService();
        // met le nom de la catégorie en titre
        TitreLabel.Text = categorieRecue.Name;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // charge les données
        await dataService.LoadData();
        // récup directement les aliments de cette catégorie
        aliments = dataService.GetAlimentsById(categorie.CategoryId);
        // afficher liste
        AlimentListView.ItemsSource = aliments;
    }
    private async void OnAddClicked(object sender, EventArgs eventArgs)
    {
        // affiche juste un message pour l instant
        // connecter à AddEditAlimentPage quand la page sera la
        await DisplayAlert("Info", "Page d'ajout à venir", "OK");
    }

    // bouton modifier sur une carte aliment
    private async void OnEditClicked(object sender, EventArgs eventArgs)
    {
        // on récupère l aliment depuis le CommandParameter du bouton
        Button bouton = (Button)sender;
        Aliment alimentSelectionne = (Aliment)bouton.CommandParameter;

        // ouvre la page de modification
        await Navigation.PushAsync(new AddEditAlimentPage(categorie, alimentSelectionne));
    }

    // bouton supprimer sur une carte aliment
    private async void OnDeleteClicked(object sender, EventArgs eventArgs)
    {
        Button bouton = (Button)sender;
        Aliment alimentASupprimer = (Aliment)bouton.CommandParameter;

        // demande confirmation
        bool confirmation = await DisplayAlert(
            "Supprimer",
            $"Supprimer {alimentASupprimer.Name} ?",
            "Oui", "Non");

        if (!confirmation)
            return;

        // supprime via le DataService
        dataService.removeAliment(alimentASupprimer.AlimentId);

        // rafraîchit la liste
        aliments = dataService.GetAlimentsById(categorie.CategoryId);
        AlimentListView.ItemsSource = aliments;
    }
}