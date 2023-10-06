
namespace MauiNenuHttpClient.Exception
{
    public class InvalidNetworkException : System.Exception
    {
        public InvalidNetworkException() :base("确保你的计算机连接的是iNENU-2G或iNENU-5G等基于校园网的网络") { }
    }
}
