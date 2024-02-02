using System.Collections.Generic;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
	{

		private readonly IUnitOfWork _UnitOfWork;
		private readonly IWebHostEnvironment _WebHostEnvironment;
		public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment WebHostEnvironment)
		{
			_UnitOfWork = unitOfWork;
			_WebHostEnvironment = WebHostEnvironment;
		}
		public IActionResult Index()
		{
			List<Product> objProductList = _UnitOfWork.Product.GetAll(includeProperties: "Category").ToList();
			
			return View(objProductList);
		}

		public IActionResult Upsert(int? id)
		{
			//ViewBag.CategoryList = CategoryList;
			//ViewData["CategoryList"] = CategoryList;
			ProductVM productVM = new()
			{
				CategoryList = _UnitOfWork.Category.GetAll().Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString()
				}),
			Product = new Product()
			};

			if (id == null || id == 0)
				return View(productVM);
			else
			{
				productVM.Product = _UnitOfWork.Product.Get(u => u.Id == id, includeProperties: "ProductImages");
				return View(productVM);
			}
				
		}
		[HttpPost]
		public IActionResult Upsert(ProductVM productVM, List<IFormFile> files)
		{
			//if (obj.Name == obj.DisplayOrder.ToString())
			//{
			//    ModelState.AddModelError("Name","The Display Order cannot exaclty match the Name.");
			//}

			if (ModelState.IsValid)
			{
				if (productVM.Product.Id == 0)
					_UnitOfWork.Product.Add(productVM.Product);
				else
					_UnitOfWork.Product.Update(productVM.Product);

				_UnitOfWork.Save();

				string wwwRootPath = _WebHostEnvironment.WebRootPath;
				if (files != null)
				{

					foreach (IFormFile image in files)
					{
						string filename = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
						string productpath = @"images\products\product-" + productVM.Product.Id;
						string finalPath = Path.Combine(wwwRootPath, productpath);

						if (!Directory.Exists(finalPath))
							Directory.CreateDirectory(finalPath);

						using (var fileStream = new FileStream(Path.Combine(finalPath, filename), FileMode.Create))
						{
							image.CopyTo(fileStream);
						}

						ProductImage productImage = new()
						{
							imageUrl = @"\" + productpath + @"\" + filename,
							productId = productVM.Product.Id,
						};

						if (productVM.Product.ProductImages == null)
							productVM.Product.ProductImages = new List<ProductImage>();

						productVM.Product.ProductImages.Add(productImage);
					}

					_UnitOfWork.Product.Update(productVM.Product);
					_UnitOfWork.Save();
				}

				
				TempData["success"] = "Product created/updated successfully";
				return RedirectToAction("Index");
			}
			else
			{
				productVM.CategoryList = _UnitOfWork.Category.GetAll().Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString()
				});

				return View(productVM);
			}

		}

		public IActionResult DeleteImage(int imageId)
		{
			var imageToBeDeleted = _UnitOfWork.ProductImage.Get(u=>u.Id == imageId);
			int productId = imageToBeDeleted.productId;
			if (imageToBeDeleted != null)
			{
				if (!string.IsNullOrEmpty(imageToBeDeleted.imageUrl))
				{
					var oldImagePath = Path.Combine(_WebHostEnvironment.WebRootPath, imageToBeDeleted.imageUrl.TrimStart('\\'));
					if (System.IO.File.Exists(oldImagePath))
						System.IO.File.Delete(oldImagePath);
				}

				_UnitOfWork.ProductImage.Remove(imageToBeDeleted);
				_UnitOfWork.Save();

				TempData["success"] = "Deleted successfully";
			}

			return RedirectToAction(nameof(Upsert), new { id = productId });
		}


		#region API CALLS
		[HttpGet]
		public IActionResult GetAll()
		{
            List<Product> objProductList = _UnitOfWork.Product.GetAll(includeProperties: "Category").ToList();
			return Json(new {data = objProductList });
        }
		[HttpDelete]
        public IActionResult Delete(int? id)
        {
			var productToBeDeleted = _UnitOfWork.Product.Get(u => u.Id == id);
			if (productToBeDeleted == null)
			{
				return Json(new { success = false, message = "Error while deleting" });
			}

			string productpath = @"images\products\product-" + id;
			string finalPath = Path.Combine(_WebHostEnvironment.WebRootPath, productpath);

			if (Directory.Exists(finalPath))
			{
				string[] filePaths = Directory.GetFiles(finalPath);
				foreach (string filePath in filePaths)
					System.IO.File.Delete(filePath);

				Directory.Delete(finalPath);
			}

			_UnitOfWork.Product.Remove(productToBeDeleted);
			_UnitOfWork.Save();

            return Json(new { success = true, message = "Delete Succesful" });
        }
        #endregion

    }
}
