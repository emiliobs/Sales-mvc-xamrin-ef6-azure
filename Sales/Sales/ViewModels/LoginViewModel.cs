namespace Sales.ViewModels
{
    using GalaSoft.MvvmLight.Command;
    using Newtonsoft.Json;
    using Sales.Common.Models;
    using Sales.Helpers;
    using Sales.Services;
    using Sales.Views;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows.Input;
    using Xamarin.Forms;

    public class LoginViewModel:BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Atributtes       
        private bool isRunning;
        private bool isEnabled;
        #endregion

        #region Properties
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsRemembered { get; set; }
        public bool IsRunning
        {
            get => isRunning;
            set
            {
                if (isRunning != value)
                {
                    isRunning = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool IsEnabled
        {
            get => isEnabled;
            set
            {
                if (isEnabled != value)
                {
                    isEnabled = value;
                    OnPropertyChanged();
                }
            }

        }


        #endregion

        #region Contructors
        public LoginViewModel()
        {
            //Service
            apiService = new ApiServices();

            //Properties
            IsEnabled = true;
            IsRemembered = true;

            Email = "barrera_emilio@hotmail.com";
            Password = "Eabs123.";


        }
        #endregion

        #region Commands
        public ICommand LoginTwitterComand
        {
            get
            {
                return new RelayCommand(LoginTwitter);
            }
        }

       


        public ICommand LoginInstagramComand
        {
            get
            {
                return new RelayCommand(LoginInstagram);
            }
        }                  


        public ICommand LoginFacebookComand
        {
            get
            {
                return new RelayCommand(LoginFacebook);
            }
        }

        

        public ICommand RegisterCommand { get => new RelayCommand(Register); }

       
        public ICommand LoginCommand { get => new RelayCommand(Login); }

        #endregion

        #region Methods

        private async void LoginTwitter()
        {
            var connection = await this.apiService.CheckConnection();

            if (!connection.IsSuccess)
            {
                this.IsRunning = false;
                this.IsEnabled = true;
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    connection.Message,
                    Languages.Accept);
                return;
            }

            await Application.Current.MainPage.Navigation.PushAsync(
                new LoginTwitterPage());
        }
        private async void LoginInstagram()
        {
            var connection = await this.apiService.CheckConnection();

            if (!connection.IsSuccess)
            {
                this.IsRunning = false;
                this.IsEnabled = true;
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    connection.Message,
                    Languages.Accept);
                return;
            }

            await Application.Current.MainPage.Navigation.PushAsync(
                new LoginInstagramPage());
        }

        private async void LoginFacebook()
        {
            var connection = await this.apiService.CheckConnection();

            if (!connection.IsSuccess)
            {
                this.IsRunning = false;
                this.IsEnabled = true;
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    connection.Message,
                    Languages.Accept);
                return;
            }

            await Application.Current.MainPage.Navigation.PushAsync( new LoginFacebookPage());

        }

        private async void Register()
        {
            MainViewModel.GetInstance().Register = new RegisterPageViewModel();
           await Application.Current.MainPage.Navigation.PushAsync(new RegisterPage());
        }


        private async void Login()
        {
            if (string.IsNullOrEmpty(Email))
            {

                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.EmailValidation,
                    Languages.Accept
                    );

                return;
            }

            if (string.IsNullOrEmpty(Password))
            {

                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.PasswordValidation,
                    Languages.Accept
                    );

                return;
            }

            IsRunning = true;
            IsEnabled = false;
            //Aqui valido si hay conecction con  internet:
            var connection = await apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                IsRunning = false;
                IsEnabled = true;
                await Application.Current.MainPage.DisplayAlert(Languages.Error,
                                                               connection.Message,
                                                               Languages.Accept);
                return;
            }

            var url = Application.Current.Resources["UrlAPI"].ToString();
            var token = await apiService.GetToken(url,Email,Password);

            if (token == null || string.IsNullOrEmpty(token.AccessToken))
            {
                IsRunning = false;
                IsEnabled = true;
                await Application.Current.MainPage.DisplayAlert(Languages.Error,
                                                               Languages.SomethingWrong,
                                                               Languages.Accept);

            
                return;
            }

            //aqui guado en persistncia los token que viene del servicio:(aqui  lo guarada el plugin en disco)
            Settings.TokenType = token.TokenType;
            Settings.AccessToken = token.AccessToken;
            Settings.IsRemembered = IsRemembered;

            //aqui guarso el usuraio en persistencia: consumido desde la api
            var UrlPrefix = Application.Current.Resources["UrlPrefix"].ToString();
            var UrlUsersController = Application.Current.Resources["UrlUsersController"].ToString();
            var UrlUsersControllerGetUser = Application.Current.Resources["UrlUsersControllerGetUser"].ToString();
            var response = await apiService.GetUser(url,UrlPrefix, $"{UrlUsersController}{UrlUsersControllerGetUser}", 
                                                    Email, token.TokenType,token.AccessToken);
            if (response.IsSuccess)
            {
                var userAsp = (MyUserASP)response.Result; ;
                MainViewModel.GetInstance().UserASP = userAsp;
                Settings.UserASP = JsonConvert.SerializeObject(userAsp);
            }

            //Aqui instacion la page con la pagina sin navegacion de back productpage();
            //MainViewModel.GetInstance().Products = new ProductsViewModel();
            MainViewModel.GetInstance().Categories = new CategoriesViewModel();
            // Application.Current.MainPage = new ProductsPage();
            Application.Current.MainPage = new MasterPage();
            //App.Current.MainPage = new MasterPage();

            IsRunning = false;
            IsEnabled = true;

        }
        #endregion
    }
}
