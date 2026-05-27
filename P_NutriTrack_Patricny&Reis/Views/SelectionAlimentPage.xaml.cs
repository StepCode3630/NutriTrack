using P_NutriTrack_Patricny_Reis.DataModels;
using P_NutriTrack_Patricny_Reis.Services;

namespace P_NutriTrack_Patricny_Reis.Views;

public partial class SelectionAlimentPage : ContentPage
{

    /// <summary>
    ///  retour à la page d'accueil avec logo home
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
    private async void OnHomeClicked(object sender, EventArgs eventArgs)
    {
        await Navigation.PopToRootAsync();
    }

    private DataService dataService;
    private List<Aliment> tousLesAliments = null!;

    public SelectionAlimentPage()
    {
        InitializeComponent();
        dataService = new DataService();
    }

    /// <summary>
    /// Recup les données du json via loadData()
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await dataService.LoadData();
        tousLesAliments = dataService.GetAliments();
        AlimentListView.ItemsSource = tousLesAliments;
    }

    /// <summary>
    ///  Filtre la liste à chaque frappe
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
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

    /// <summary>
    /// Si aliment clic alors dirige vers sa page
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnAlimentSelected(object sender, SelectionChangedEventArgs e)
    {
        Aliment? alimentChoisi = (Aliment?)e.CurrentSelection.FirstOrDefault();
        if (alimentChoisi == null) return;

        AlimentListView.SelectedItem = null;
        await Navigation.PushAsync(new AjoutConsoPage(alimentChoisi));
    }
}