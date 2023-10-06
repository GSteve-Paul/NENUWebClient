using MauiNenuHttpClient;
using MauiNenuExamGrades;
using System.Data;

namespace MauiNenuApp
{
    public partial class LoginPage : ContentPage
    {

        public LoginPage()
        {
            InitializeComponent();
        }
      
        public void LoginButton_Clicked(object sender, EventArgs e)
        {
            string username = usernameEntry.Text;
            string password = passwordEntry.Text;

            NenuClient client = null;
            bool hasValue = Application.Current.Resources.TryGetValue("client", out object obj);
            if (!hasValue)
            {
                DisplayAlert("未找到NenuClient", "错误", "关闭");
                return;
            }
            client = (NenuClient)obj;
            client.Login(username, password);

            Shell.Current.GoToAsync("///ExamGradePage");
        }
    }

}