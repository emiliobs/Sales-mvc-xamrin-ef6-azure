using Newtonsoft.Json;
using Sales.Common.Models;
using Sales.Helpers;
using Sales.Services;
using Sales.ViewModels;
using Sales.Views;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation (XamlCompilationOptions.Compile)]
namespace Sales
{
	public partial class App : Application
	{
        #region Properties
        public static NavigationPage Navigator { get; internal set; } 
        #endregion

        #region Contructs
        public App()
        {
            InitializeComponent();

            var mainViewModel = MainViewModel.GetInstance();

            if (Settings.IsRemembered)
            {

                if (!string.IsNullOrEmpty(Settings.UserASP))
                {
                    mainViewModel.UserASP = JsonConvert.DeserializeObject<MyUserASP>(Settings.UserASP);
                }

                //mainViewModel.Products = new ProductsViewModel();
                mainViewModel.Categories = new CategoriesViewModel();
                this.MainPage = new MasterPage();
            }
            else
            {
                mainViewModel.Login = new LoginViewModel();
                
                this.MainPage = new NavigationPage(new LoginView());
            }


            //if (Settings.IsRemembered && !string.IsNullOrEmpty(Settings.AccessToken))
            //{
            //    MainViewModel.GetInstance().Products = new ProductsViewModel();
            //    //la mamster page tiene su propio navegador
            //    MainPage = new MasterPage();
            //    //MainPage = new NavigationPage(new ProductsPage());
            //}
            //else
            //{
            //    // MainPage = new NavigationPage (new ProductsPage());
            //    MainViewModel.GetInstance().Login = new LoginViewModel();
            //    MainPage = new NavigationPage(new LoginView());
            //}


        }

        #endregion

        #region Methods
        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        public static Action HideLoginView
        {
            get
            {
                return new Action(() => Current.MainPage = new NavigationPage(new LoginView()));
            }
        }

        public static async Task NavigateToProfile(TokenResponse token)
        {
            if (token == null)
            {
                Application.Current.MainPage = new NavigationPage(new LoginView());
                return;
            }

            Settings.IsRemembered = true;
            Settings.AccessToken = token.AccessToken;
            Settings.TokenType = token.TokenType;
            
         var apiService = new ApiServices();
            var url = Application.Current.Resources["UrlAPI"].ToString();
            var prefix = Application.Current.Resources["UrlPrefix"].ToString();
            var controller = Application.Current.Resources["UrlUsersController"].ToString();
            var controllerGetUser = Application.Current.Resources["UrlUsersControllerGetUser"].ToString();
            var response = await apiService.GetUser(url, prefix, $"{controller}{controllerGetUser}", 
                                                    token.UserName, token.TokenType, token.AccessToken);
            if (response.IsSuccess)
            {
                var userASP = (MyUserASP)response.Result;
                MainViewModel.GetInstance().UserASP = userASP;
                Settings.UserASP = JsonConvert.SerializeObject(userASP);
            }

            //MainViewModel.GetInstance().Products = new ProductsViewModel();
            MainViewModel.GetInstance().Categories = new CategoriesViewModel();
            Application.Current.MainPage = new MasterPage();
        }


        #endregion
    }
}
