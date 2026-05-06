using P_NutriTrack_Patricny_Reis.Services;

namespace P_NutriTrack_Patricny_Reis
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            InitDatabase();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }

        public async void InitDatabase()
        {
            var ServiceCollection = new DataService();
            await ServiceCollection.Init();
        }
    }
}