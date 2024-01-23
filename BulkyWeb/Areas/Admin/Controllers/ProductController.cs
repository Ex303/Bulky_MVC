using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class ProductController : Controller
	{

		private readonly IUnitOfWork _UnitOfWork;
		public ProductController(IUnitOfWork unitOfWork)
		{
			_UnitOfWork = unitOfWork;
		}
		public IActionResult Index()
		{
			List<Product> objProductList = _UnitOfWork.Product.GetAll().ToList();
			return View(objProductList);
		}

		public IActionResult Create()
		{
			return View();
		}
		[HttpPost]
		public IActionResult Create(Product obj)
		{
			//if (obj.Name == obj.DisplayOrder.ToString())
			//{
			//    ModelState.AddModelError("Name","The Display Order cannot exaclty match the Name.");
			//}

			if (ModelState.IsValid)
			{
				_UnitOfWork.Product.Add(obj);
				_UnitOfWork.Save();
				TempData["success"] = "Product created successfully";
				return RedirectToAction("Index");
			}

			return View();

		}

		public IActionResult Edit(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}

			//works only with the primary key coloumn
			Product? ProductFromDb = _UnitOfWork.Product.Get(u => u.Id == id);
			//can retrieve all columns
			//Product? ProductFromDb1 = _db.Categories.FirstOrDefault(u=>u.Id==id);
			//Product? ProductFromDb2 = _db.Categories.Where(u=>u.Id==id).FirstOrDefault();

			if (ProductFromDb == null)
			{
				return NotFound();
			}

			return View(ProductFromDb);
		}
		[HttpPost]
		public IActionResult Edit(Product obj)
		{

			if (ModelState.IsValid)
			{
				_UnitOfWork.Product.Update(obj);
				_UnitOfWork.Save();
				TempData["success"] = "Product edited successfully";
				return RedirectToAction("Index");
			}

			return View();

		}

		public IActionResult Delete(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}

			Product? ProductFromDb = _UnitOfWork.Product.Get(u => u.Id == id);


			if (ProductFromDb == null)
			{
				return NotFound();
			}

			return View(ProductFromDb);
		}
		[HttpPost, ActionName("Delete")]
		public IActionResult DeletePOST(int? id)
		{
			Product obj = _UnitOfWork.Product.Get(u => u.Id == id);

			if (obj == null)
			{
				return NotFound();
			}

			_UnitOfWork.Product.Remove(obj);
			_UnitOfWork.Save();
			TempData["success"] = "Product deleted successfully";
			return RedirectToAction("Index");

		}

	}
}
