[assembly: Xamarin.Forms.Dependency(typeof(Sales.Droid.Implementation.PathService))]
namespace Sales.Droid.Implementation
{
    using Interfaces;
    using System;
    using System.IO;

    public class PathService : IPathService
    {
        public string GetDatabasePath()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            return Path.Combine(path, "Sales.db3");
        }
    }



}