namespace Sales.ViewModels
{
    using GalaSoft.MvvmLight.Command;
    using Sales.Common.Models;
    using Sales.Helpers;
    using Sales.Services;
    using Sales.Views;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Input;
    using Xamarin.Forms;

    public class ProductItemViewModel  : Product
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Contructors
        public ProductItemViewModel()
        {
            apiService = new ApiServices();
        }
        #endregion

        #region Commands

        public ICommand EditProductCommand { get => new RelayCommand(EditProduct); }

        private async void EditProduct()
        {
            MainViewModel.GetInstance().EditProduct = new EditProductViewModel(this);
            //await Application.Current.MainPage.Navigation.PushAsync(new EditProductView());
            await App.Navigator.PushAsync(new EditProductView());  
        }

        public ICommand DeleteProductCommand { get => new RelayCommand(DeleteProduct); }
        #endregion

        #region Methods
        private async void DeleteProduct()
        {
            var answer = await Application.Current.MainPage.DisplayAlert(
                        Languages.Confirm,
                         Languages.DeleteConfirmation, 
                         Languages.Yes, 
                         Languages.No
                         );

            if (!answer)
            {
                return;
            }

            //Aqui valido si hay conecction con  internet:
            var connection = await apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
               
                await Application.Current.MainPage.DisplayAlert(Languages.Error, 
                                                                connection.Message,
                                                                Languages.Accept);
                return;
            }

            var url = Application.Current.Resources["UrlAPI"].ToString();
            var urlPrefix = Application.Current.Resources["UrlPrefix"].ToString();
            var controller = Application.Current.Resources["UrlProductsController"].ToString();
            var response = await apiService.Delete(url, urlPrefix, controller, ProductId, Settings.TokenType, Settings.AccessToken);
            // var response = await apiService.GetList<Product>($"https://salesapiservices.azurewebsites.net", 
            //"/api", "/Products");
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert(Languages.Error, response.Message, Languages.Accept);

                return;
            }


            //aui ya borro el registro en el service, y lluego tengo que refrescar la lista:
            var productsViewModel = ProductsViewModel.GetInstance();
            var deleteProduct = productsViewModel.MyProducts.Where(p => p.ProductId.Equals(ProductId)).FirstOrDefault();

            if (deleteProduct != null)
            {
                productsViewModel.MyProducts.Remove(deleteProduct);
            }

            //aqui refresco:
            productsViewModel.RefreshList();

        }
        #endregion
    }
}
