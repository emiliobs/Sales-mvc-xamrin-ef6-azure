namespace Sales.Infrastructure
{
    using Sales.ViewModels;

    public class InstanceLocator
    {
        #region Properties
        public MainViewModel Main { get; set; }
        #endregion

        #region Contructs
        public InstanceLocator()
        {
            Main = new MainViewModel();
        } 
        #endregion
    }
}
