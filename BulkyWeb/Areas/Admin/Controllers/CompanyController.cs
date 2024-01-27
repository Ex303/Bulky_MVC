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
    public class CompanyController : Controller
	{

		private readonly IUnitOfWork _UnitOfWork;
		private readonly IWebHostEnvironment _WebHostEnvironment;
		public CompanyController(IUnitOfWork unitOfWork)
		{
			_UnitOfWork = unitOfWork;
		}
		public IActionResult Index()
		{
			List<Company> objCompanyList = _UnitOfWork.Company.GetAll().ToList();
			
			return View(objCompanyList);
		}

		public IActionResult Upsert(int? id)
		{


			if (id == null || id == 0)
				return View(new Company());
			else
			{
				Company obj = _UnitOfWork.Company.Get(u => u.Id == id);
				return View(obj);
			}
				
		}
		[HttpPost]
		public IActionResult Upsert(Company ojb)
		{
			//if (obj.Name == obj.DisplayOrder.ToString())
			//{
			//    ModelState.AddModelError("Name","The Display Order cannot exaclty match the Name.");
			//}

			if (ModelState.IsValid)
			{ 
				if(ojb.Id == 0)
					_UnitOfWork.Company.Add(ojb);
				else
					_UnitOfWork.Company.Update(ojb);


				_UnitOfWork.Save();
				TempData["success"] = "Company created successfully";
				return RedirectToAction("Index");
			}
			else
			{
				return View(ojb);
			}

		}


		#region API CALLS
		[HttpGet]
		public IActionResult GetAll()
		{
            List<Company> objCompanyList = _UnitOfWork.Company.GetAll().ToList();
			return Json(new {data = objCompanyList });
        }
		[HttpDelete]
        public IActionResult Delete(int? id)
        {
			var CompanyToBeDeleted = _UnitOfWork.Company.Get(u => u.Id == id);
			if (CompanyToBeDeleted == null)
			{
				return Json(new { success = false, message = "Error while deleting" });
			}

			_UnitOfWork.Company.Remove(CompanyToBeDeleted);
			_UnitOfWork.Save();

            List<Company> objCompanyList = _UnitOfWork.Company.GetAll().ToList();
            return Json(new { success = true, message = "Delete Succesful" });
        }
        #endregion

    }
}
