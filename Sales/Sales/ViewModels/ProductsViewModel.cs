namespace Sales.ViewModels
{
    using GalaSoft.MvvmLight.Command;
    using Sales.Common.Models;
    using Sales.Helpers;
    using Sales.Services;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Xamarin.Forms;

    public class ProductsViewModel : BaseViewModel
    {
        #region Services
        ApiServices apiService;
        DataService dataService;
        #endregion

        #region Atributtes  
        private string filter;
        ObservableCollection<ProductItemViewModel> listProducts;
        bool isRefreshing;
        #endregion

        #region Properties

       public Category Category { get; set; }

        public string Filter
        {
            get =>filter;
            set
            {
                if (filter != value)
                {
                    filter = value;
                    this.RefreshList();
                  
                }
            }
        }
        public List<Product> MyProducts { get; set; }

        public bool IsRefreshing
        {
            get => isRefreshing;
            set
            {
                if (isRefreshing != value)
                {
                    isRefreshing = value;
                    OnPropertyChanged();
                }
            }
        }
        public ObservableCollection<ProductItemViewModel> ListProducts
        {
            get => listProducts;
            set
            {
                if (listProducts != value)
                {
                    listProducts = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Contructs
        public ProductsViewModel(Category category)
        {
            //singleton
            instance = this;

            //service
            apiService = new ApiServices();
            dataService = new DataService();

            //Methods
            LoadProducts();

            this.Category = category;
        }
        #endregion

        #region Singlenton

        private static ProductsViewModel instance;        

        public static ProductsViewModel GetInstance()
        {
            //if (instance == null)
            //{
            //    return new ProductsViewModel();
            //}

            return instance;
        }

        #endregion

        #region Commands

        public ICommand SearchCommand { get => new RelayCommand(RefreshList); }

       

        public ICommand RefreshCommand { get => new RelayCommand(Refresh); }

        #endregion

        #region Mehtods

        

        private void Refresh()
        {
            LoadProducts();
        }

        private async void LoadProducts()
        {
            this.IsRefreshing = true;

            var connection = await this.apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                this.IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert(Languages.Error, connection.Message, Languages.Accept);
                return;
            }

            var answer = await this.LoadProductsFromAPI();
            if (answer)
            {
                this.RefreshList();
            }

            this.IsRefreshing = false;
        }

        private async Task<bool> LoadProductsFromAPI()
        {
            var url = Application.Current.Resources["UrlAPI"].ToString();
            var prefix = Application.Current.Resources["UrlPrefix"].ToString();
            var controller = Application.Current.Resources["UrlProductsController"].ToString();
            var response = await this.apiService.GetList<Product>(url, prefix, controller, this.Category.CategoryId, Settings.TokenType, Settings.AccessToken);
            if (!response.IsSuccess)
            {
                return false;
            }

            this.MyProducts = (List<Product>)response.Result;
            return true;
        }



        //private async void LoadProducts()
        //{
        //    IsRefreshing = true;

        //    //Aqui valido si hay conecction con  internet:
        //    var connection = await apiService.CheckConnection();

        //    //if (!connection.IsSuccess)
        //    //{
        //    //    IsRefreshing = false;
        //    //    await Application.Current.MainPage.DisplayAlert(Languages.Error,
        //    //                                                   connection.Message,
        //    //                                                   Languages.Accept);
        //    //    return;
        //    //}

        //    if (connection.IsSuccess)
        //    {
        //        var answers = await LoadProductsFromAPI();

        //        if (answers)
        //        {
        //            SaveProductsToDB();
        //        }
        //    }
        //    else
        //    {
        //        await LoadProductsFromDB();
        //    }

        //    //si esto sucese espor que no hay productos o no conexion:
        //    if (MyProducts == null || MyProducts.Count == 0)
        //    {
        //        IsRefreshing = false;
        //        await Application.Current.MainPage.DisplayAlert(Languages.Error,
        //                                                          connection.Message,
        //                                                          Languages.Accept);

        //        return;
        //    }

        //    //Aqui un método para refrezcar la lista:
        //    this.RefreshList();

        //}

        private async Task LoadProductsFromDB()
        {
            //Aqui traigo todos los productos de la bd:
            MyProducts = await dataService.GetAllProducts();
               
        }

        private async Task SaveProductsToDB()
        {
            //aqui borros los datos en la bd local si existen antes de grabarlos:
            await dataService.DeleteAllProducts();  
            //aqui guando en bd los datos de los datos del lla lsita:
            await dataService.Insert(MyProducts);
        }

        //private async Task<bool> LoadProductsFromAPI()
        //{
        //    var url = Application.Current.Resources["UrlAPI"].ToString();
        //    var urlPrefix = Application.Current.Resources["UrlPrefix"].ToString();
        //    var controller = Application.Current.Resources["UrlProductsController"].ToString();
        //    var response = await apiService.GetList<Product>(url, urlPrefix, controller, Settings.TokenType, Settings.AccessToken);
        //    // var response = await apiService.GetList<Product>($"https://salesapiservices.azurewebsites.net", 
        //    //"/api", "/Products");
        //    if (!response.IsSuccess)
        //    {
        //        //await Application.Current.MainPage.DisplayAlert(Languages.Error, response.Message, Languages.Accept);
        //        //IsRefreshing = false;

        //        return false;
        //    }

        //    MyProducts = (List<Product>)response.Result;

        //    return true;
        //}

        public void RefreshList()
        {
            //NO esto funciona, pero el lo que no se puede hacer porque es 
            //ineficiente cuando hay 200 o mas registro en la bd:
            //var myList = new List<ProductItemViewModel>();
            //foreach (var item in listProduct)
            //{
            //    myList.Add(new ProductItemViewModel {



            //    });
            // }

            if (string.IsNullOrEmpty(Filter ))
            {
                //Mejor opcion con landa y linq:
                var myListProductItemViewModel = MyProducts.Select(p => new ProductItemViewModel
                {
                    Description = p.Description,
                    ImagePath = p.ImagePath,
                    ImageArray = p.ImageArray,
                    IsAvailable = p.IsAvailable,
                    Price = p.Price,
                    ProductId = p.ProductId,
                    PublishOn = p.PublishOn,
                    Remarks = p.Remarks,


                });


                //aqui armos las observablecollection a aprtir de una genericcollection(list)
                ListProducts = new ObservableCollection<ProductItemViewModel>(myListProductItemViewModel.OrderBy(p => p.Description));
                IsRefreshing = false;
            }
            else
            {
                //Mejor opcion con landa y linq:
                var myListProductItemViewModel = MyProducts.Select(p => new ProductItemViewModel
                {
                    Description = p.Description,
                    ImagePath = p.ImagePath,
                    ImageArray = p.ImageArray,
                    IsAvailable = p.IsAvailable,
                    Price = p.Price,
                    ProductId = p.ProductId,
                    PublishOn = p.PublishOn,
                    Remarks = p.Remarks,


                }).Where(p=>p.Description.ToLower().Trim().Contains(Filter.ToLower().Trim())).ToList();


            //aqui armos las observablecollection a aprtir de una genericcollection(list)
            ListProducts = new ObservableCollection<ProductItemViewModel>(myListProductItemViewModel.OrderBy(p => p.Description));
            IsRefreshing = false;
            }

           
        }
        #endregion
    }
}
