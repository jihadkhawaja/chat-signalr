using MobileChat.Cache;
using MobileChat.Interface;
using MobileChat.Models;
using MobileChat.Services;
using MobileChat.Views;
using Xamarin.Forms;

namespace MobileChat
{
    public partial class App : Application
    {
        public static AppSettings appSettings;

        public static string CurrentPage = "ChatPage";

        //const
        public const string iOSAppID = "";
        public const string appStoreAppBaseURL = "https://apps.apple.com/us/app/id";
        public const string playStoreAppID = "";
        public const string playStoreAppBaseURL = "https://play.google.com/store/apps/details?id=";
        public const string AppName = "Mobile Chat";

        //SignalR Web URL example (http://localhost:2736/chathub) where the chat web app is hosted
        public const string hubName = "chathub";
#if DEBUG
        //development
        public const string hubConnectionURL = "your address here" + hubName;
#else
        //production
        public const string hubConnectionURL = "your address here" + hubName;
#endif
        
        //follow me and give this repo a star if you liked it <3
        public const string feedback = "https://twitter.com/jihadkhawaja";

        public App()
        {
            RegisterServices();

            //load settings and user credentials
            try
            {
                SavingManager.FileManager.CreateDirectory("appsettings", "data");
                appSettings = new AppSettings();
                appSettings = SavingManager.JsonSerialization.ReadFromJsonFile<AppSettings>("appsettings/user");
            }
            catch { }

            InitializeComponent();

            //MainPage = new NavigationPage(new ChatPage());
            MainPage = new NavigationPage(new FriendsPage());
        }
        private void RegisterServices()
        {
            DependencyService.Register<ISignalR, SignalRService>();
            DependencyService.Register<IChat, ChatService>();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}