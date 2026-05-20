using P_NutriTrack_Patricny_Reis.DataModels;
using P_NutriTrack_Patricny_Reis.Services;

namespace P_NutriTrack_Patricny_Reis.Views;

public partial class SelectionAlimentPage : ContentPage
{

    // retour à la page d'accueil avec logo home
    private async void OnHomeClicked(object sender, EventArgs eventArgs)
    {
        await Navigation.PopToRootAsync();
    }

    private DataService dataService;
    private List<Aliment> tousLesAliments = null!;

    public SelectionAlimentPage()
    {
        InitializeComponent();
        //Sqlite
        dataService = DataService.Instance;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        tousLesAliments = await dataService.GetAliments();
        AlimentListView.ItemsSource = tousLesAliments;
    }

    // Filtre la liste à chaque frappe
    private void OnRechercheChanged(object sender, TextChangedEventArgs e)
    {
        string recherche = (e.NewTextValue ?? "").Trim().ToLower();

        if (string.IsNullOrEmpty(recherche))
        {
            AlimentListView.ItemsSource = tousLesAliments;
        }
        else
        {
            List<Aliment> filtres = tousLesAliments
                .Where(a => a.Name.ToLower().Contains(recherche))
                .ToList();
            AlimentListView.ItemsSource = filtres;
        }
    }

    private async void OnAlimentSelected(object sender, SelectionChangedEventArgs e)
    {
        Aliment? alimentChoisi = (Aliment?)e.CurrentSelection.FirstOrDefault();
        if (alimentChoisi == null) return;

        AlimentListView.SelectedItem = null;
        await Navigation.PushAsync(new AjoutConsoPage(alimentChoisi));
    }
}