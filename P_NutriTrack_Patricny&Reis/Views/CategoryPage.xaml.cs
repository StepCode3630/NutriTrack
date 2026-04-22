using P_NutriTrack_Patricny_Reis.Models;
using P_NutriTrack_Patricny_Reis.Services;

namespace P_NutriTrack_Patricny_Reis.Views;

public partial class CategoryPage : ContentPage
{
	private DataService _dataService;
	private List<Category> _categories;
	public CategoryPage()
	{
		InitializeComponent();
	}
}