using System.Threading.Tasks;

namespace MobileChatWeb.Interfaces
{
    public interface IFirebaseNotification
    {
        Task<bool> Send(string token, string title, string message);
        Task<bool> SendAll(string title, string message);
    }
}
