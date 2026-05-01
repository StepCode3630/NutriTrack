using P_NutriTrack_Patricny_Reis.DataModels;
using P_NutriTrack_Patricny_Reis.Services;

namespace P_NutriTrack_Patricny_Reis.Views;

/******************************************************************************
** PROGRAMME  AlimentListPage.xaml.cs                                        **
**                                                                           **
** Lieu      : ETML - section informatique                                   **
** Auteur    : Camille Rais                                                  **
** Date      : 01.04.2026                                                    **
**                                                                           **
******************************************************************************/

public partial class AlimentListPage : ContentPage
{
    private DataService dataService;
    private List<Aliment> aliments = null!;
    private Category categorie;

    public AlimentListPage(Category categorieRecue)
    {
        InitializeComponent();
        categorie = categorieRecue;
        dataService = new DataService();
        TitreLabel.Text = categorieRecue.Name;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await dataService.LoadData();
        aliments = dataService.GetAlimentsById(categorie.CategoryId);
        AlimentListView.ItemsSource = aliments;
    }

    private async void OnAddClicked(object sender, EventArgs eventArgs)
    {
        await Navigation.PushAsync(new AddEditAlimentPage(categorie));
    }

    private async void OnEditClicked(object sender, EventArgs eventArgs)
    {
        Button bouton = (Button)sender;
        Aliment alimentSelectionne = (Aliment)bouton.CommandParameter;
        await Navigation.PushAsync(new AddEditAlimentPage(categorie, alimentSelectionne));
    }

    private async void OnDeleteClicked(object sender, EventArgs eventArgs)
    {
        Button bouton = (Button)sender;
        Aliment alimentASupprimer = (Aliment)bouton.CommandParameter;

        string message = $"Supprimer {alimentASupprimer.Name} ?";
        bool confirmation = await DisplayAlert("Supprimer", message, "Oui", "Non");

        if (!confirmation) return;

        dataService.removeAliment(alimentASupprimer.AlimentId);
        aliments = dataService.GetAlimentsById(categorie.CategoryId);
        AlimentListView.ItemsSource = aliments;
    }

    private async void OnAlimentSelected(object sender, SelectionChangedEventArgs eventArgs)
    {
        Aliment alimentSelectionne = (Aliment)eventArgs.CurrentSelection.FirstOrDefault();
        if (alimentSelectionne == null) return;

        AlimentListView.SelectedItem = null;
        await Navigation.PushAsync(new AlimentDetailPage(categorie, alimentSelectionne));
    }
}
