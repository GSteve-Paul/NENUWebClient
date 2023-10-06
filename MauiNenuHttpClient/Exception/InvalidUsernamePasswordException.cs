
namespace MauiNenuHttpClient.Exception
{
    public class InvalidUsernamePasswordException : System.Exception
    {
        public InvalidUsernamePasswordException() : base("用户名或密码错误") {}
    }
}
