using MauiNenuHttpClient;
using MauiNenuExamGrades;
using System.Collections.Concurrent;
using System.Reflection;

namespace MauiNenuApp
{
    public partial class ExamGradePage : ContentPage
    {
        public ExamGradePage()
        {
            InitializeComponent();

            //get the client
            NenuClient client = null;
            bool hasValue = Application.Current.Resources.TryGetValue("client", out object obj);
            if (!hasValue)
            {
                DisplayAlert("Œ¥’“µΩNenuClient", "¥ÌŒÛ", "πÿ±’");
                return;
            }
            client = (NenuClient)obj;
            NenuExamGrade nenuExamGrade = new NenuExamGrade(client);
            ConcurrentBag<ExamGradeRow> examGrades = nenuExamGrade.GetExamGrades();

#if WINDOWS
            GradeTableView.Header = Resources["HeaderComplex"];
            GradeTableView.ItemTemplate = (DataTemplate)Resources["ContentComplex"];
#endif

#if ANDROID
            GradeTableView.Header = Resources["HeaderSimple"];
            GradeTableView.ItemTemplate = (DataTemplate)Resources["ContentSimple"];
#endif

            GradeTableView.ItemsSource = examGrades;
        }
    }
}