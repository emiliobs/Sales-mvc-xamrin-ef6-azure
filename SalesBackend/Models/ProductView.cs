namespace SalesBackend.Models
{
    using Sales.Common.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    public class ProductView : Product
    {
        public HttpPostedFileBase ImageFile { get; set; }
    }
}