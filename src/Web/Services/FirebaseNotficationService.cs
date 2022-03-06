using MobileChat.Web.Interfaces;
using MobileChat.Web.Models;
using MobileChat.Web.Rest;
using System.Text.Json;
using System.Threading.Tasks;

namespace MobileChat.Web.Services
{
    public class FirebaseNotficationService : IFirebaseNotification
    {
        private readonly RestClient RestClient;
        public FirebaseNotficationService()
        {
            RestClient = new RestClient();
        }
        public async Task<bool> Send(string token, string title, string message)
        {
            Firebase.Response response = JsonSerializer.Deserialize<Firebase.Response>(await RestClient.PostAsync(new Firebase.Message()
            {
                to = token,
                data = new Firebase.Data() { message = title, body = message },
            }, "https://fcm.googleapis.com/fcm/send",
            "",
            AuthType.Token));

            if (response.success == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> SendAll(string title, string message)
        {
            Firebase.Response response = JsonSerializer.Deserialize<Firebase.Response>(await RestClient.PostAsync(new Firebase.Message()
            {
                to = "general",
                data = new Firebase.Data() { message = title, body = message },
            }, "https://fcm.googleapis.com/fcm/send",
            "",
            AuthType.Token));

            if (response.success == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
