namespace Sales.ViewModels
{
    using GalaSoft.MvvmLight.Command;
    using Plugin.Geolocator;
    using Plugin.Geolocator.Abstractions;
    using Plugin.Media;
    using Plugin.Media.Abstractions;
    using Sales.Common.Models;
    using Sales.Helpers;
    using Sales.Services;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Xamarin.Forms;

    public class AddProductViewModel:BaseViewModel
    {
        #region Services

        private ApiServices ApiServices;

        #endregion

        #region Atributtes
        //aqui queda almacenada la foto con el plugin de media:
        private MediaFile file;
        private bool isRunning;
        private bool isEnabled;
        private ImageSource imageSource;
        private ObservableCollection<Category> categories;
        private Category category;

        #endregion

        #region Properties
        public ObservableCollection<Category> Categories
        {
            get => categories;
            set
            {
                if (categories != value)
                {
                    categories = value;
                    OnPropertyChanged();
                }
                  
                                   
                
            }
        }

        //esta la utilizo para cargar la lista api y tenerla en memoria
        public List<Category> MyCategories { get; set; }

        //esta es la que binda si el usurio selecciona una categora:
        public Category Category
        {
            get => category;
            set
            {
                if (category != value)
                {
                    category = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Description { get; set; }
        public string Price { get; set; }
        public string Remarks { get; set; }
        public bool IsRunning
        { get => isRunning;
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
        #endregion

        #region Constructor
        public AddProductViewModel()
        {
            ApiServices = new ApiServices();

            IsEnabled = true;
            ImageSource = "noproduct";

            

            this.LoadCategories();

        }

        #endregion

        #region Commands
        public ICommand ChangeImageCommand { get => new RelayCommand(ChangeImage); }

        public ICommand SaveCommand { get => new RelayCommand(Save); }

        #endregion

        #region Methods                                 
        private async void LoadCategories()
        {
            this.IsRunning = true;
            this.IsEnabled = false;

            var connection = await this.ApiServices.CheckConnection();
            if (!connection.IsSuccess)
            {
                this.IsRunning = false;
                this.IsEnabled = true;
                await Application.Current.MainPage.DisplayAlert(Languages.Error, connection.Message, Languages.Accept);
                return;
            }

            var answer = await this.LoadCategoriesFromAPI();
            if (answer)
            {
                this.RefreshList();
                                       

            }

            this.IsRunning = false;
            this.IsEnabled = true;
        }

        private void RefreshList()
        {
           this.Categories = new ObservableCollection<Category>(MyCategories.OrderBy(c => c.Description));
        }

        private async Task<bool> LoadCategoriesFromAPI()
        {
            var url = Application.Current.Resources["UrlAPI"].ToString();
            var prefix = Application.Current.Resources["UrlPrefix"].ToString();
            var controller = Application.Current.Resources["UrlCategoriesController"].ToString();
            var response = await this.ApiServices.GetList<Category>(url, prefix, controller, Settings.TokenType, Settings.AccessToken);
            if (!response.IsSuccess)
            {
                return false;
            }

            this.MyCategories = (List<Category>)response.Result;
            return true;
        }

        private async void ChangeImage()
        {
            await CrossMedia.Current.Initialize();
            var souce = await Application.Current.MainPage.DisplayActionSheet(
                 Languages.ImageSource,
                 Languages.Cancel,null,
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
                ImageSource = ImageSource.FromStream(()=> 
                {
                    var stream = file.GetStream();
                    return stream;
                });
            }
        }

        private async void Save()
        {
            if (string.IsNullOrEmpty(Description))
            {
                await Application.Current.MainPage.DisplayAlert(Languages.Error, 
                    Languages.DescriptionError, 
                    Languages.Accept);
                
                return;
            }

            if (string.IsNullOrEmpty(Price))
            {
                await Application.Current.MainPage.DisplayAlert(Languages.Error, Languages.PriceError, Languages.Accept);

                return;
            }

            var price = decimal.Parse(Price);

            if (price < 0)
            {
                await Application.Current.MainPage.DisplayAlert(Languages.Error,
                    Languages.PriceError, 
                    Languages.Accept);
                return;
            }

            //aqui pregunto y obligo a seleccionar una categoria en el picker:
            if (this.Category == null)
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.CategoryError,
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
            }

            //aqui armo el objeto con las variables desde el formulario add new product:
            //var product = new Product
            //{
            //   Description = Description,
            //   Price       = price,
            //   Remarks     = Remarks,
            //   ImageArray = imageArray,
            //};

            var location = await GetLocation();

            var product = new Product
            {
                Description = this.Description,
                Price = price,
                Remarks = this.Remarks,
                ImageArray = imageArray,
                CategoryId = this.Category.CategoryId,
                //aqui logeo el producto, con userId
                UserId = MainViewModel.GetInstance().UserASP.Id,
                //localizacion:
                Latitude = location == null ? 0 : location.Latitude,
                Longitude = location == null ? 0 : location.Longitude,


            };


            var url = Application.Current.Resources["UrlAPI"].ToString();
            var urlPrefix = Application.Current.Resources["UrlPrefix"].ToString();
            var controller = Application.Current.Resources["UrlProductsController"].ToString();

            var response = await ApiServices.Post(url, urlPrefix, controller, product, Settings.TokenType, Settings.AccessToken);


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
            //aqui utlilizo el singleton para utilizar la propiedad de tippo lista MyProducts:
            productViewModel.MyProducts.Add(newProduct);
            productViewModel.RefreshList();
           
            IsRunning = false;
            IsEnabled = true;

            //aqui desapilo y regreso a la página anterior:
            //await Application.Current.MainPage.Navigation.PopAsync();
            await App.Navigator.PopAsync();
        }

        private async Task<Position> GetLocation()
        {
            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 50;
            var location = await locator.GetPositionAsync();
            return location;
        }


        #endregion
    }
}
