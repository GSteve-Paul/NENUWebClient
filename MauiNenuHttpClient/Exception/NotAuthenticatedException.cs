
namespace MauiNenuHttpClient.Exception
{
    public class NotAuthenticatedException :System.Exception
    {
        public NotAuthenticatedException() : base("NenuClient尚未登陆成功") { }
    }
}
