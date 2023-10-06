using MauiNenuHttpClient.Exception;

namespace MauiNenuHttpClient
{
    public class NenuService
    {
        protected NenuClient _Client;
        
        public NenuService(NenuClient client) 
        {
            if (client.IsLogin)
                _Client = client;
            else
                throw new NotAuthenticatedException();
        }
    }
}
