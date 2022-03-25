using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobileChat.ViewModel;
using System.Threading.Tasks;

namespace MobileChat.Test
{
    [TestClass]
    public class ChatTest
    {
        [TestMethod]
        public async Task CanConnect_ReturnTrue()
        {
            ChatViewModel chatViewModel = new ChatViewModel();

            await chatViewModel.Connect();

            bool result = chatViewModel.IsConnected;

            Assert.IsTrue(result, "Not connected to server");
        }
    }
}
