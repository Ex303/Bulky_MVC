using BulkyWeb_Temp.Data;
using BulkyWeb_Temp.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWeb_Temp.Pages.Categories
{
	[BindProperties]
	public class CreateModel : PageModel
	{
		public readonly ApplicationDbContext _db;

		
		public Category Category { get; set; }
		public CreateModel(ApplicationDbContext db)
		{
			_db = db;
		}

		public void OnGet()
		{

		}

		public IActionResult OnPost()
		{
			_db.Categories.Add(Category);
			_db.SaveChanges();
			TempData["success"] = "Category created successfully";
			return RedirectToPage("index");

		}
	}
}
