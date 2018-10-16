using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Sales;
using Sales.Common.Models;
using Sales.ViewModels;
using Sales.Views;

public class CategoryItemViewModel : Category
{
    #region Commands
    public ICommand GotoCategoryCommand
    {
        get
        {
            return new RelayCommand(GotoCategory);
        }
    }

    private async void GotoCategory()
    {
        //MainViewModel.GetInstance().Products = new ProductsViewModel(this);
        await App.Navigator.PushAsync(new ProductsPage());
    }
    #endregion
}
