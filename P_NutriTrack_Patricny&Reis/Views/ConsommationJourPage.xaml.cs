using P_NutriTrack_Patricny_Reis.DataModels;
using P_NutriTrack_Patricny_Reis.Services;

namespace P_NutriTrack_Patricny_Reis.Views;

public partial class ConsommationJourPage : ContentPage
{
    private DataService dataService;
    // Liste affichee à l écran avec info
    private List<ConsoAffichage> consoAffichees = null!;

    public ConsommationJourPage()
    {
        InitializeComponent();
        dataService = new DataService();
        DateLabel.Text = DateTime.Today.ToString("dddd dd MMMM");
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await dataService.LoadData();
        AfficherConsommations();
    }

    private void AfficherConsommations()
    {
        List<Consommation> consoDuJour = dataService.GetConsommationsDuJour();

        // enrichit chaque conso avec les infos de son aliment
        consoAffichees = new List<ConsoAffichage>();
        double totalCal = 0, totalProt = 0, totalGluc = 0, totalLip = 0;

        foreach (Consommation conso in consoDuJour)
        {
            Aliment? aliment = dataService.GetAlimentById(conso.AlimentFk);
            if (aliment == null) continue;

            // Calcul nutritionnel selon la quantité pour 100g
            double facteur = conso.Quantite_g / 100.0;
            double cal = aliment.Calories * facteur;
            double prot = aliment.Proteines_g * facteur;
            double gluc = aliment.Glucides_g * facteur;
            double lip = aliment.Lipides_g * facteur;

            totalCal += cal;
            totalProt += prot;
            totalGluc += gluc;
            totalLip += lip;

            consoAffichees.Add(new ConsoAffichage
            {
                ConsommationId = conso.ConsommationId,
                NomAliment = aliment.Name,
                InfoLigne = $"{conso.Quantite_g}g - {cal:F0} Kcal"
            });
        }

        ConsoListView.ItemsSource = consoAffichees;
        TotalCaloriesLabel.Text = $"{totalCal:F0}";
        TotalProteinesLabel.Text = $"{totalProt:F1}";
        TotalGlucidesLabel.Text = $"{totalGluc:F1}";
        TotalLipidesLabel.Text = $"{totalLip:F1}";
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SelectionAlimentPage());
    }

    private async void OnEditClicked(object sender, EventArgs e)
    {
        Button bouton = (Button)sender;
        int consoId = (int)bouton.CommandParameter;

        // voit la conso et l'aliment correspondant
        Consommation? conso = dataService.GetConsommationsDuJour()
            .FirstOrDefault(c => c.ConsommationId == consoId);
        if (conso == null) return;

        Aliment? aliment = dataService.GetAlimentById(conso.AlimentFk);
        if (aliment == null) return;

        // Ouvre page d'ajout mode édition
        await Navigation.PushAsync(new AjoutConsoPage(aliment, conso));
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        Button bouton = (Button)sender;
        int consoId = (int)bouton.CommandParameter;

        bool confirm = await DisplayAlert("Supprimer",
            "Retirer cet aliment de ta journée ?", "Oui", "Non");
        if (!confirm) return;

        dataService.removeConso(consoId);
        AfficherConsommations();
    }
}

// classe pour l'affichage
public class ConsoAffichage
{
    public int ConsommationId { get; set; }
    public string NomAliment { get; set; } = "";
    public string InfoLigne { get; set; } = "";
}