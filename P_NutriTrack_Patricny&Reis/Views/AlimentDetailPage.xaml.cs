/******************************************************************************
** PROGRAMME  AlimentDetailPage.xaml.cs                                      **
**                                                                           **
** Lieu      : ETML - section informatique                                   **
** Auteur    : Camille Rais                                                  **
** Date      : 01.04.2026                                                    **
**                                                                           **
******************************************************************************/

/******************************************************************************
** DESCRIPTION                                                               **
**                                                                           **
** Page qui affiche le détail nutritionnel d'un aliment                      **
** Permet de modifier ou supprimer l'aliment                                 **
** Sensor Capte la secousse du téléphone                                     **
** Sensor vibration : confirme qu'il a entendu la secousse                   **
**                                                                           **
******************************************************************************/

using P_NutriTrack_Patricny_Reis.DataModels;
using P_NutriTrack_Patricny_Reis.Services;
// pour utiliser le capteur
// accélérometre
using Microsoft.Maui.Devices.Sensors;
// vibration
using Microsoft.Maui.Devices;

namespace P_NutriTrack_Patricny_Reis.Views;

public partial class AlimentDetailPage : ContentPage
{

    /// <summary>
    /// retour à la page d'accueil
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
    private async void OnHomeClicked(object sender, EventArgs eventArgs)
    {
        await Navigation.PopToRootAsync();
    }

    private DataService dataService;
    // catégorie de l'aliment (pour pouvoir le modifier ensuite)
    private Category categorie;
    // l'aliment affiché
    private Aliment aliment;

    /// <summary>
    /// constructeur reçoit la catégorie et l'aliment à afficher
    /// </summary>
    /// <param name="categorieRecue"></param>
    /// <param name="alimentRecu"></param>
    public AlimentDetailPage(Category categorieRecue, Aliment alimentRecu)
    {
        InitializeComponent();
        categorie = categorieRecue;
        aliment = alimentRecu;
        dataService = new DataService();

        // remplit les labels avec les valeurs de l'aliment
        AfficherDetails();
    }

    /// <summary>
    /// refresh quand on revient sur la page après modification
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // recharge les données pour avoir les valeurs à jour
        await dataService.LoadData();

        // récupère la version à jour de l'aliment
        Aliment? alimentMaj = dataService.GetAlimentById(aliment.AlimentId);
        if (alimentMaj != null)
        {
            aliment = alimentMaj;
            AfficherDetails();
        }

        // démarre pour écouter le capteur pour détecter si on le secoue
        if (Accelerometer.Default.IsSupported && !Accelerometer.Default.IsMonitoring)
        {
            Accelerometer.Default.ShakeDetected += OnShakeDetected;
            Accelerometer.Default.Start(SensorSpeed.UI);
        }
    }

    /// <summary>
    /// arrête l écoute du capteur quand on quitte la page ou il est implémenté
    /// </summary>
    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        if (Accelerometer.Default.IsMonitoring)
        {
            Accelerometer.Default.Stop();
            Accelerometer.Default.ShakeDetected -= OnShakeDetected;
        }
    }

    /// <summary>
    /// affiche les valeurs nutritionnelles dans les labels
    /// </summary>
    private void AfficherDetails()
    {
        NomLabel.Text = aliment.Name;
        CaloriesLabel.Text = aliment.Calories.ToString();
        ProteinesLabel.Text = aliment.Proteines_g.ToString();
        GlucidesLabel.Text = aliment.Glucides_g.ToString();
        LipidesLabel.Text = aliment.Lipides_g.ToString();
        FibresLabel.Text = aliment.Fibres_g.ToString();

        // affiche vitamines qui viennent de la saisie de txt
        if (aliment.Vitamines != null && aliment.Vitamines.Count > 0)
            VitaminesLabel.Text = string.Join(", ", aliment.Vitamines);
        else
            VitaminesLabel.Text = "-";

        // affiche minéraux
        if (aliment.Mineraux != null && aliment.Mineraux.Count > 0)
            MinerauxLabel.Text = string.Join(", ", aliment.Mineraux);
        else
            MinerauxLabel.Text = "-";
    }

    /// <summary>
    /// bouton modifier : ouvre la page d'édition
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
    private async void OnEditClicked(object sender, EventArgs eventArgs)
    {
        await Navigation.PushAsync(new AddEditAlimentPage(categorie, aliment));
    }

    /// <summary>
    /// bouton supprimer : demande confirmation puis supprime
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
    private async void OnDeleteClicked(object sender, EventArgs eventArgs)
    {
        string message = $"Supprimer {aliment.Name} ?";
        bool confirmation = await DisplayAlert(
            "Supprimer",
            message,
            "Oui", "Non");

        if (!confirmation) return;

        // supprime via le DataService
        await dataService.RemoveAliment(aliment.AlimentId);

        // retour à la page d'avant
        await Navigation.PopAsync();
    }

    /// <summary>
    /// se declanche quand le téléphone est secoué
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
    private async void OnShakeDetected(object? sender, EventArgs eventArgs)
    {
        // vibre pour confirmer qu il entend la secousse
        try
        {
            Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(300));
        }
        catch (FeatureNotSupportedException)
        {
            // try/catch car certain émulateur ne supportent pas la vibration donc ça évite que l'app crash
        }

        // retour à l'accueil
        await Navigation.PopToRootAsync();
    }
}