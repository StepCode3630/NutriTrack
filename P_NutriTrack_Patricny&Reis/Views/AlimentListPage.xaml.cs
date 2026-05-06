using P_NutriTrack_Patricny_Reis.DataModels;
using P_NutriTrack_Patricny_Reis.Services;
namespace P_NutriTrack_Patricny_Reis.Views;

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
        await dataService.Init();
        // récup directement les aliments de cette catégorie
        Task<List<Aliment>> aliments = dataService.GetAlimentsById(categorie.CategoryId);
        // afficher liste
         AlimentListView.ItemsSource = (System.Collections.IEnumerable)aliments;
    }
    private async void OnAddClicked(object sender, EventArgs eventArgs)
    {
        // ouvre la page d ajout en passant la catégorie actuelle
        await Navigation.PushAsync(new AddEditAlimentPage(categorie));
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

        // demande confirmation pour supprimer ou non
        string message = $"Supprimer {alimentASupprimer.Name} ?";
        bool confirmation = await DisplayAlert(
            "Supprimer",
            message,
            "Oui", "Non");

        if (!confirmation)
            return;

        // supprime via le DataService
        dataService.removeAliment(alimentASupprimer.AlimentId);

        // rafraîchit la liste
        Task<List<Aliment>> aliments = dataService.GetAlimentsById(categorie.CategoryId);
        AlimentListView.ItemsSource = (System.Collections.IEnumerable)aliments;
    }
    // quand on clique sur un aliment de la liste
    private async void OnAlimentSelected(object sender, SelectionChangedEventArgs eventArgs)
    {
        // récup l'aliment cliqué
        Aliment alimentSelectionne = (Aliment)eventArgs.CurrentSelection.FirstOrDefault();
        if (alimentSelectionne == null)
            return;

        // déselectionne pour pouvoir recliquer au retour
        AlimentListView.SelectedItem = null;

        // ouvre la page détail
        await Navigation.PushAsync(new AlimentDetailPage(categorie, alimentSelectionne));
    }
}