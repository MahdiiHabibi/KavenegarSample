using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KavenegarSample.Data;
using KavenegarSample.Models;
using KavenegarSample.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace KavenegarSample.Controllers
{
	public class PeopleController : Controller
	{
		private readonly ContextDb _context;
		private readonly ISenderCode _senderCode;
		public PeopleController(ContextDb context, ISenderCode senderCode)
		{
			_context = context;
			_senderCode = senderCode;
		}

		// GET: People
		public async Task<IActionResult> Index()
		{
			return View(await _context.Person.ToListAsync());
		}

		// GET: People/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var person = await _context.Person
				.FirstOrDefaultAsync(m => m.UserId == id);
			if (person == null)
			{
				return NotFound();
			}

			return View(person);
		}

		// GET: People/Create
		public IActionResult Create()
		{
			return View();
		}

		// POST: People/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("UserId,FullName,PhoneNumber,Age,IsActive")] Person person)
		{
			if (ModelState.IsValid)
			{
				var random = new Random();
				var code = random.Next(10000, 99999);

				_context.Add(person);
				await _context.SaveChangesAsync();

				if (await _senderCode.SendCode(person.PhoneNumber, "کاوه نگار",
						person.FullName.Split(" "[0]).ToString(),code.ToString()));
				{
					var claims = new List<Claim>()
					{
						new(ClaimTypes.MobilePhone, person.PhoneNumber),
						new(ClaimTypes.PostalCode, code.ToString()),
						new (ClaimTypes.NameIdentifier,person.UserId.ToString())
					};
					var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
					var principal = new ClaimsPrincipal(identity);
					var properties = new AuthenticationProperties
					{
						IsPersistent = false
					};
					await HttpContext.SignInAsync(principal, properties);
					CreateMassageAlert("Success", "Activation Code Is Send To Your Phone", "Success");
				}

				return RedirectToAction("ConformActiveCode");
			}
			return View(person);
		}



		public IActionResult ConformActiveCode()
		{
			return View();
		}

		public IActionResult ConformActiveCode(GetCodeOfActivationViewModel model)
		{
			if (ModelState.IsValid)
			{
				var userPhoneNumber = User.FindFirstValue(ClaimTypes.MobilePhone);
				var userActiveCode = int.Parse(User.FindFirstValue(ClaimTypes.PostalCode) ?? "0");
				var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
				if (model.Code != userActiveCode)
				{
					CreateMassageAlert("danger", "Active Code In Not True ", "Danger");
					return NoContent();
				}
				else
				{
					CreateMassageAlert("success","your Account in actived","success");
					var person = _context.Person.Find(userId);
					person.IsActive = true;
				}

			}
			CreateMassageAlert("danger", "Some Things is Wrong in Getting Information. Add Person Again", "Danger");
			return RedirectToAction("Index");
		}


		// GET: People/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var person = await _context.Person.FindAsync(id);
			if (person == null)
			{
				return NotFound();
			}
			return View(person);
		}

		// POST: People/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("UserId,FullName,PhoneNumber,Age,IsActive")] Person person)
		{
			if (id != person.UserId)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(person);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!PersonExists(person.UserId))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				return RedirectToAction(nameof(Index));
			}
			return View(person);
		}

		// GET: People/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var person = await _context.Person
				.FirstOrDefaultAsync(m => m.UserId == id);
			if (person == null)
			{
				return NotFound();
			}

			return View(person);
		}

		// POST: People/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var person = await _context.Person.FindAsync(id);
			if (person != null)
			{
				_context.Person.Remove(person);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool PersonExists(int id)
		{
			return _context.Person.Any(e => e.UserId == id);
		}

		private void CreateMassageAlert(string typeOfAlert, string descriptionOfAlert, string titleOfAlert, bool? isTake = false)
		{
			TempData["Massage_TypeOfAlert"] = typeOfAlert;
			TempData["Massage_DescriptionOfAlert"] = descriptionOfAlert;
			TempData["Massage_TitleOfAlert"] = titleOfAlert;
			if (isTake == true)
			{
				TempData.Keep();
			}
		}

	}
}
