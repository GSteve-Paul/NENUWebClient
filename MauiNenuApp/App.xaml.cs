using MauiNenuHttpClient;

namespace MauiNenuApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
            NenuClient nenuClient = new NenuClient();
            Current.Resources.Add("client", nenuClient);
        }
    }
}