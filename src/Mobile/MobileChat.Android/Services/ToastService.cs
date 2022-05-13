using Android.Widget;
using MobileChat.Droid.Services;
using MobileChat.Interface;

[assembly: Xamarin.Forms.Dependency(typeof(ToastService))]

namespace MobileChat.Droid.Services
{
    public class ToastService : IToast
    {
        public void Show(string message)
        {
            Android.Widget.Toast.MakeText(Android.App.Application.Context, message, ToastLength.Long).Show();
        }
    }
}