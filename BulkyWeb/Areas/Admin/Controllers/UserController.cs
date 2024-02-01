using System.Collections.Generic;
using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BulkyWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
	{

		private readonly ApplicationDbContext _db;
		private readonly IWebHostEnvironment _WebHostEnvironment;
		public UserController(ApplicationDbContext db)
		{
            _db = db;
		}
		public IActionResult Index()
		{
			return View();
		}

		#region API CALLS
		[HttpGet]
		public IActionResult GetAll()
		{
            List<ApplicationUser> objUserList = _db.ApplicationUser.Include(u=>u.Company).ToList();

			var userRole = _db.UserRoles.ToList();
			var roles = _db.Roles.ToList();

			foreach (var item in objUserList)
			{
				var roleId = userRole.FirstOrDefault(u => u.UserId == item.Id).RoleId;
				item.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;

				if (item.Company == null)
				{
					item.Company = new()
					{
						Name = ""
					};
				}
			}
			return Json(new {data = objUserList });
        }
		[HttpPost]
        public IActionResult LockUnlock([FromBody]string id)
        {
			var objFromDb = _db.ApplicationUser.FirstOrDefault(u => u.Id == id);
			if (objFromDb == null)
			{
				return Json(new { success = true, message = "Error while locking/unlocking" });
			}

			if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
				//the user is currently locked
				objFromDb.LockoutEnd = DateTime.Now;
			else
				objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);

			_db.SaveChanges();

            return Json(new { success = true, message = "Operation Succesful" });
        }
        #endregion

    }
}
