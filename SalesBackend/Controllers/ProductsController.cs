using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Sales.Common.Models;
using SalesBackend.Models;
using SalesBackend.Helpers;

namespace SalesBackend.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private LocalDataContext db = new LocalDataContext();

        // GET: Products
        public async Task<ActionResult> Index() => View(await db.Products.OrderBy(p =>p.Description).ToListAsync());

        // GET: Products/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ProductView view)
        {
            if (ModelState.IsValid)
            {
                var picture = string.Empty;
                var folder = "~/Content/Products";
                if (view.ImageFile != null)
                {
                    picture = FilesHelpers.UploadPhoto(view.ImageFile, folder);
                    picture = $"{folder}/{picture}";
                }

                var product = this.ToProduct(view, picture);

                try
                {
                    db.Products.Add(product);
                    await db.SaveChangesAsync();
                }
                catch (Exception e)
                {

                   
                }
                return RedirectToAction("Index");
            }

            return View(view);
        }

        private Product ToProduct(ProductView view, string picture)
        {
            return new Product()
            {
              Description = view.Description,
              ImagePath = picture,
              IsAvailable = view.IsAvailable,
              Price = view.Price,
              ProductId = view.ProductId,
              PublishOn = view.PublishOn,
              Remarks = view.Remarks,
              
              
            }; 
        }



        // GET: Products/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            //de producto al productView
            var view = this.ToView(product);
            return View(view);
        }

        private ProductView ToView(Product product)
        {
            return new ProductView()
            {
                Description = product.Description,
                ImagePath =  product.ImagePath,
                IsAvailable = product.IsAvailable,
                Price = product.Price,
                ProductId = product.ProductId,
                PublishOn = product.PublishOn,
                Remarks = product.Remarks,

            };
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ProductView view)
        {
            if (ModelState.IsValid)
            {
                var picture = view.ImagePath;
                var folder = "~/Content/Products";
                if (view.ImageFile != null)
                {
                    picture = FilesHelpers.UploadPhoto(view.ImageFile, folder);
                    picture = $"{folder}/{picture}";
                }

                var product = this.ToProduct(view, picture);   
                try
                {
                    db.Entry(product).State = EntityState.Modified;

                    await db.SaveChangesAsync();
                }
                catch (Exception e)
                {

                  
                }

                return RedirectToAction("Index");
            }
            return View(view);
        }

        // GET: Products/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Product product = await db.Products.FindAsync(id);
            db.Products.Remove(product);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
