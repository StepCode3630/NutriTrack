/******************************************************************************
** PROGRAMME  AddEditAlimentPage.xaml.cs                                     **
**                                                                           **
** Lieu      : ETML - section informatique                                   **
** Auteur    : Camille Rais                                                  **
** Date      : 01.04.2026                                                    **
**                                                                           **
******************************************************************************/

/******************************************************************************
** DESCRIPTION                                                               **
**                                                                           **
** Page d'ajout ou de modification d'un aliment                              **
** Selon le constructeur utilisé on est en mode ajout ou édition             **
**                                                                           **
******************************************************************************/

using P_NutriTrack_Patricny_Reis.DataModels;
using P_NutriTrack_Patricny_Reis.Services;

namespace P_NutriTrack_Patricny_Reis.Views;

public partial class AddEditAlimentPage : ContentPage
{
    private DataService dataService;
    // catégorie dans laquelle on ajoute / modifie l'aliment
    private Category categorie;
    // aliment à modifier - reste null si on est en mode ajout
    private Aliment? alimentAModifier;

    /// <summary>
    /// constructeur pour ajout d'un nouvel aliment
    /// </summary>
    /// <param name="categorieRecue"></param>
    public AddEditAlimentPage(Category categorieRecue)
    {
        InitializeComponent();
        categorie = categorieRecue;
        dataService = new DataService();
        alimentAModifier = null;
        TitreLabel.Text = "Ajouter un aliment";
    }

    /// <summary>
    /// retour à la page d'accueil
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
    private async void OnHomeClicked(object sender, EventArgs eventArgs)
    {
        await Navigation.PopToRootAsync();
    }

    /// <summary>
    /// constructeur pour modification d'un aliment existant
    /// </summary>
    /// <param name="categorieRecue"></param>
    /// <param name="alimentRecu"></param>
    public AddEditAlimentPage(Category categorieRecue, Aliment alimentRecu)
    {
        InitializeComponent();
        // pres-remplit le formulaire avec les valeurs de la bdd
        categorie = categorieRecue;
        dataService = new DataService();
        alimentAModifier = alimentRecu;
        TitreLabel.Text = "Modifier un aliment";
        NomEntry.Text = alimentRecu.Name;
        CaloriesEntry.Text = alimentRecu.Calories.ToString();
        ProteinesEntry.Text = alimentRecu.Proteines_g.ToString();
        GlucidesEntry.Text = alimentRecu.Glucides_g.ToString();
        LipidesEntry.Text = alimentRecu.Lipides_g.ToString();
        FibresEntry.Text = alimentRecu.Fibres_g.ToString();
        // pré remplit les vitamines et minéraux de liste joint par virgules
        if (alimentRecu.Vitamines != null)
            VitaminesEntry.Text = string.Join(", ", alimentRecu.Vitamines);
        if (alimentRecu.Mineraux != null)
            MinerauxEntry.Text = string.Join(", ", alimentRecu.Mineraux);
    }

    /// <summary>
    /// bouton enregistrer : valide et sauvegarde
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
    private async void OnSaveClicked(object sender, EventArgs eventArgs)
    {
        // vérifie que le nom n'est pas vide
        if (string.IsNullOrWhiteSpace(NomEntry.Text))
        {
            await DisplayAlert("Erreur", "Le nom est obligatoire", "OK");
            return;
        }

        // charge les données
        await dataService.LoadData();
        List<Aliment> tousLesAliments = dataService.GetAliments();

        if (alimentAModifier == null)
        {
            // mode ajout
            Aliment nouvelAliment = new Aliment();

            // calcul d'un nouvel ID unique
            // quand la liste est vide
            if (tousLesAliments.Count == 0)
                nouvelAliment.AlimentId = 1;
            else
                // prend id le plus grand et fait +1
                nouvelAliment.AlimentId = tousLesAliments
                    .Max(item => item.AlimentId) + 1;

            // remplit les valeurs avec ce qui a été saisi
            nouvelAliment.Name = NomEntry.Text;
            nouvelAliment.Calories = ConvertirEnInt(CaloriesEntry.Text);
            nouvelAliment.Proteines_g = ConvertirEnDouble(ProteinesEntry.Text);
            nouvelAliment.Glucides_g = ConvertirEnDouble(GlucidesEntry.Text);
            nouvelAliment.Lipides_g = ConvertirEnDouble(LipidesEntry.Text);
            nouvelAliment.Fibres_g = ConvertirEnDouble(FibresEntry.Text);
            nouvelAliment.CategoryFk = categorie.CategoryId;
            // pour le texte saisi par l user
            nouvelAliment.Vitamines = ConvertirEnListe(VitaminesEntry.Text);
            nouvelAliment.Mineraux = ConvertirEnListe(MinerauxEntry.Text);

            await dataService.AddAliment(nouvelAliment);
        }
        else
        {
            // mode modification
            Aliment alimentModifie = new Aliment();
            alimentModifie.AlimentId = alimentAModifier.AlimentId;
            alimentModifie.Name = NomEntry.Text;
            alimentModifie.Calories = ConvertirEnInt(CaloriesEntry.Text);
            alimentModifie.Proteines_g = ConvertirEnDouble(ProteinesEntry.Text);
            alimentModifie.Glucides_g = ConvertirEnDouble(GlucidesEntry.Text);
            alimentModifie.Lipides_g = ConvertirEnDouble(LipidesEntry.Text);
            alimentModifie.Fibres_g = ConvertirEnDouble(FibresEntry.Text);
            alimentModifie.CategoryFk = categorie.CategoryId;
            // pour le texte saisi par l user
            alimentModifie.Vitamines = ConvertirEnListe(VitaminesEntry.Text);
            alimentModifie.Mineraux = ConvertirEnListe(MinerauxEntry.Text);

            await dataService.UpdateAliment(alimentModifie);
        }

        // retour à la page d'avant
        await Navigation.PopAsync();
    }

    /// <summary>
    /// méthode utilitaire pour convertir un texte en int
    /// </summary>
    /// <param name="texte"></param>
    /// <returns></returns>
    private int ConvertirEnInt(string texte)
    {
        if (int.TryParse(texte, out int resultat))
            return resultat;
        return 0;
    }

    /// <summary>
    /// méthode utilitaire pour convertir un texte en double
    /// </summary>
    /// <param name="texte"></param>
    /// <returns></returns>
    private double ConvertirEnDouble(string texte)
    {
        if (double.TryParse(texte, out double resultat))
            return resultat;
        return 0;
    }

    /// <summary>
    /// transforme le texte saisi par l'user  quand il ajoute un aliment en liste
    /// </summary>
    /// <param name="texte"></param>
    /// <returns></returns>
    private List<string> ConvertirEnListe(string? texte)
    {
        if (string.IsNullOrWhiteSpace(texte))
            return new List<string>();

        return texte.Split(',')
                    .Select(element => element.Trim())
                    .Where(element => element.Length > 0)
                    .ToList();
    }
}