namespace Sales.ViewModels
{
    using GalaSoft.MvvmLight.Command;
    using Plugin.Media;
    using Plugin.Media.Abstractions;
    using Sales.Common.Models;
    using Sales.Helpers;
    using Sales.Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Input;
    using Xamarin.Forms;
   

    public class EditProductViewModel: BaseViewModel
    {

        #region Services

        private ApiServices ApiServices;

        #endregion

        #region Attributtes
        private Product product;

        //aqui queda almacenada la foto con el plugin de media:
        private MediaFile file;
        private bool isRunning;
        private bool isEnabled;
        private ImageSource imageSource;

        #endregion

        #region Properties

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
        public ImageSource ImageSource
        {
            get => imageSource;
            set
            {
                if (imageSource != value)
                {
                    imageSource = value;
                    OnPropertyChanged();
                }
            }
        }

        public Product Product
        {
            get => product;
            set
            {
                if (product != value)
                {
                    product = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Constrictor
        public EditProductViewModel(Product product)
        {
            this.product = product;

            ApiServices = new ApiServices();

            IsEnabled = true;
            ImageSource = product.ImageFullPath;
        }



        #endregion

        #region Commands
        public ICommand DeleteCommand { get => new RelayCommand(Delete); }
        public ICommand ChangeImageCommand { get => new RelayCommand(ChangeImage); }   
        public ICommand SaveCommand { get => new RelayCommand(Save); }

        #endregion

        #region Methods

        private async void Delete()
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

            IsRunning = true;
            IsEnabled = false;

            //Aqui valido si hay conecction con  internet:
            var connection = await ApiServices.CheckConnection();
            if (!connection.IsSuccess)
            {

                await Application.Current.MainPage.DisplayAlert(Languages.Error,
                                                                connection.Message,
                                                                Languages.Accept);

                IsRunning = false;
                IsEnabled = true;
                return;
            }

            var url = Application.Current.Resources["UrlAPI"].ToString();
            var urlPrefix = Application.Current.Resources["UrlPrefix"].ToString();
            var controller = Application.Current.Resources["UrlProductsController"].ToString();
            var response = await ApiServices.Delete(url, urlPrefix, controller, product.ProductId, Settings.TokenType, Settings.AccessToken);
            // var response = await apiService.GetList<Product>($"https://salesapiservices.azurewebsites.net", 
            //"/api", "/Products");
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert(Languages.Error, response.Message, Languages.Accept);

                return;
            }


            //aui ya borro el registro en el service, y lluego tengo que refrescar la lista:
            var productsViewModel = ProductsViewModel.GetInstance();
            var deleteProduct = productsViewModel.MyProducts.Where(p => p.ProductId.Equals(product.ProductId)).FirstOrDefault();

            if (deleteProduct != null)
            {
                productsViewModel.MyProducts.Remove(deleteProduct);
            }

            //aqui refresco:
            productsViewModel.RefreshList();

            IsRunning = false;
            IsEnabled = true;
            //aqui cuando ya elimine hago un back:
            // await Application.Current.MainPage.Navigation.PopAsync();
            await App.Navigator.PopAsync();

        }
        private async void ChangeImage()
        {
            await CrossMedia.Current.Initialize();
            var souce = await Application.Current.MainPage.DisplayActionSheet(
                 Languages.ImageSource,
                 Languages.Cancel, null,
                 Languages.FromGallery,
                 Languages.NewPicture
                );

            if (souce == Languages.Cancel)
            {
                file = null;
                return;
            }

            if (souce == Languages.NewPicture)
            {
                file = await CrossMedia.Current.TakePhotoAsync(

                    new StoreCameraMediaOptions
                    {
                        Directory = "Sample",
                        Name = "test,jpg",
                        PhotoSize = PhotoSize.Small,
                    }

                    );
            }
            else
            {
                file = await CrossMedia.Current.PickPhotoAsync();
            }

            if (file != null)
            {
                ImageSource = ImageSource.FromStream(() =>
                {
                    var stream = file.GetStream();
                    return stream;
                });
            }
        }

        private async void Save()
        {
            if (string.IsNullOrEmpty(Product.Description))
            {
                await Application.Current.MainPage.DisplayAlert(Languages.Error,
                    Languages.DescriptionError,
                    Languages.Accept);

                return;
            }          

            if (Product.Price < 0)
            {
                await Application.Current.MainPage.DisplayAlert(Languages.Error,
                    Languages.PriceError,
                    Languages.Accept);
                return;
            }

            IsRunning = true;
            IsEnabled = false;

            //Aqui valido si hay conecction con  internet:
            var connection = await ApiServices.CheckConnection();
            if (!connection.IsSuccess)
            {
                IsRunning = false;
                IsEnabled = true;

                await Application.Current.MainPage.DisplayAlert(Languages.Error,
                                                                connection.Message,
                                                                Languages.Accept);
                return;
            }

            //aqui todo para guardar la foto:
            byte[] imageArray = null;
            if (file != null)
            {
                //aqui convierto un arreglo de string a byte:
                imageArray = FileHelper.ReadFully(file.GetStream());
                Product.ImageArray = imageArray;
            }

            var url = Application.Current.Resources["UrlAPI"].ToString();
            var urlPrefix = Application.Current.Resources["UrlPrefix"].ToString();
            var controller = Application.Current.Resources["UrlProductsController"].ToString();

            var response = await ApiServices.Put(url, urlPrefix, controller, product, Product.ProductId, Settings.TokenType, Settings.AccessToken); 


            if (!response.IsSuccess)
            {
                IsRunning = false;
                IsEnabled = true;
                await Application.Current.MainPage.DisplayAlert(Languages.Error,
                                                              response.Message,
                                                              Languages.Accept);
                return;

            }

            //new product
            var newProduct = (Product)response.Result;
            //patron sigleton
            var productViewModel = ProductsViewModel.GetInstance();

            //aqui busco el producto a editar:
            var oldProduct = productViewModel.MyProducts.Where(p=>p.ProductId.Equals(product.ProductId)).FirstOrDefault();

            //si lo encuentra lo vvalido:
            if (oldProduct != null)
            {
                //lo elimino y grabo el editado:
                productViewModel.MyProducts.Remove(oldProduct);
                ////aqui refresco la lista:
                //productViewModel.RefreshList();

            }

            //aqui utlilizo el singleton para utilizar la propiedad de tippo lista MyProducts:
            productViewModel.MyProducts.Add(newProduct);
            //aqui refresco la lista:
            productViewModel.RefreshList();

            IsRunning = false;
            IsEnabled = true;

            //aqui desapilo y regreso a la página anterior:
            //await Application.Current.MainPage.Navigation.PopAsync();
            await App.Navigator.PopAsync();
        }

        #endregion
    }
}
