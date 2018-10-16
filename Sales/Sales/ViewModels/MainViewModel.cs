namespace Sales.ViewModels
{
    using GalaSoft.MvvmLight.Command;
    using Sales.Common.Models;
    using Sales.Helpers;
    using Sales.Views;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Text;
    using System.Windows.Input;
    using Xamarin.Forms;

    public class MainViewModel
    {
        #region Properties

        //Aqui creo esta propiedad para porder acceder al usuro desde toda la palicación:
        public MyUserASP UserASP { get; set; }
        public ProductsViewModel Products { get; set; }
        public AddProductViewModel AddProduct { get; set; }
        public EditProductViewModel EditProduct { get; set; }
        public LoginViewModel Login { get; set; }
        public RegisterPageViewModel Register{ get; set; }
        public CategoriesViewModel Categories { get; set; }

        public ObservableCollection<MenuItemViewModel> Menu { get; set; }

        //aqui cargo los datos que estan grabados en el claims
        public string UserFullName
        {
            get
            {
                if (UserASP != null && UserASP.Claims != null && UserASP.Claims.Count > 0)
                {
                    return $"{UserASP.Claims[0].ClaimValue} {UserASP.Claims[1].ClaimValue}";
                }

                return null;
            }

        }
        //public string  UserImageFullPath
        //{
        //    get
        //    {
        //        if (UserASP != null && UserASP.Claims != null && UserASP.Claims.Count > 3)
        //        {
        //            return $"https://salesapiservices.azurewebsites.net{UserASP.Claims[3].ClaimValue.Substring(1)}";
        //           // return $"http://192.168.0.11:54268{UserASP.Claims[3].ClaimValue.Substring(1)}";
        //        }

        //        return null;
        //    }

        //}

            //esta porpiedad me toma la foto coon la de facebook o la de mi api:
        public string UserImageFullPath
        {
            get
            {
                foreach (var claim in this.UserASP.Claims)
                {
                    if (claim.ClaimType == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/uri")
                    {
                        //aqui pregunto si la foto es mia o de facebook, siempieza con ~ es una foto mia de mi api,
                        //si no es de facebook
                        if (claim.ClaimValue.StartsWith("~"))
                        {
                            return $"https://salesapiservices.azurewebsites.net{claim.ClaimValue.Substring(1)}";
                        }

                        return claim.ClaimValue;
                    }
                }

                return null;
            }
        }


        #endregion

        #region Contructs

        public MainViewModel()
        {
            instance = this;
            //Products = new ProductsViewModel();

            LoadMenu();
        }

       

        #endregion

        #region Singlenton

        private static MainViewModel instance;

        public static MainViewModel GetInstance()
        {
            if (instance == null)
            {
                return new MainViewModel();
            }

            return instance;
        }

        #endregion

        #region Commands

        public ICommand AddProductCommand { get => new RelayCommand(GoToAddProduct); }



        #endregion

        #region Methods
        private void LoadMenu()
        {
            this.Menu = new ObservableCollection<MenuItemViewModel>();

            this.Menu.Add(new MenuItemViewModel
            {
                Icon = "ic_info",
                PageName = "AboutPage",
                Title = Languages.About,
            });

            this.Menu.Add(new MenuItemViewModel
            {
                Icon = "ic_phonelink_setup",
                PageName = "SetupPage",
                Title = Languages.Setup,
            });

            this.Menu.Add(new MenuItemViewModel
            {
                Icon = "ic_exit_to_app",
                PageName = "LoginView",
                Title = Languages.Exit,
            });

        }


        private async void GoToAddProduct()
        {
            //instancio la clase justo en el mmento que la necesite:
            AddProduct = new AddProductViewModel();
            //await Application.Current.MainPage.Navigation.PushAsync(new AddProductPage());
            //esta propiedad Navigator fuela se creo en la master datail page, para navegar atraves de la masterpage:
            await App.Navigator.PushAsync(new AddProductPage());
        }

        #endregion
    }
}
