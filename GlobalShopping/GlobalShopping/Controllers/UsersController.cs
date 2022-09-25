using AspNetCoreHero.ToastNotification.Abstractions;
using GlobalShopping.Common;
using GlobalShopping.Data;
using GlobalShopping.Data.Entities;
using GlobalShopping.Enums;
using GlobalShopping.Helpers;
using GlobalShopping.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GlobalShopping.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly ICombosHelper _combosHelper;
        private readonly IBlobHelper _blobHelper;
		private readonly IMailHelper _mailHelper;
        private readonly IToastifyService _toastify;

        public UsersController(DataContext context, IUserHelper userHelper, ICombosHelper combosHelper, IBlobHelper blobHelper,
            IMailHelper mailHelper, IToastifyService toastify)
        {
            _context = context;
            _userHelper = userHelper;
            _combosHelper = combosHelper;
            _blobHelper = blobHelper;
			_mailHelper = mailHelper;
            _toastify = toastify;
        }

        public async Task<IActionResult> Index()
        {
            return _context.Users != null ?
                        View(await _context.Users
                        .Include(u => u.City)
                        .ThenInclude(c => c.State)
                        .ThenInclude(s => s.Country)
                        .ToListAsync()) :
                        Problem("Entity set 'DataContext.Users'  is null.");
        }

        public async Task<IActionResult> Create()
        {
            AddUserViewModel model = new()
            {
                Id = Guid.Empty.ToString(),
                Countries = await _combosHelper.GetComboCountriesAsync(),
                States = await _combosHelper.GetComboStatesAsync(0),
                Cities = await _combosHelper.GetComboCitiesAsync(0),
                UserType = UserType.Admin,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                Guid imageId = Guid.Empty;

                if (model.ImageFile != null)
                {
                    imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "users");
                }

                model.ImageId = imageId;
                User user = await _userHelper.AddUserAsync(model);
                if (user == null)
                {
                    _toastify.Error("Este correo ya está siendo usado.");
                    model.Countries = await _combosHelper.GetComboCountriesAsync();
                    model.States = await _combosHelper.GetComboStatesAsync(model.CountryId);
                    model.Cities = await _combosHelper.GetComboCitiesAsync(model.StateId);
                    return View(model);
                }

                string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                string tokenLink = Url.Action("ConfirmEmail", "Account", new
                {
                    userid = user.Id,
                    token = myToken
                }, protocol: HttpContext.Request.Scheme);

                Response response = _mailHelper.SendMail(
                    $"{model.FirstName} {model.LastName}",
                    model.Username,
                    "GlobalShopping - Confirmación de Email",
                    $"<h1>GlobalShopping - Confirmación de Email</h1>" +
                        $"Para habilitar el usuario por favor hacer clic en el siguiente link:, " +
                        $"<hr/><br/><p><a href = \"{tokenLink}\">Confirmar Email</a></p>");
                if (response.IsSuccess)
                {
                    _toastify.Success("Las instrucciones para habilitar el administrador han sido enviadas al correo.");
                    return View(model);
                }

                _toastify.Error(response.Message);
            }

            model.Countries = await _combosHelper.GetComboCountriesAsync();
            model.States = await _combosHelper.GetComboStatesAsync(model.CountryId);
            model.Cities = await _combosHelper.GetComboCitiesAsync(model.StateId);
            return View(model);
        }

        public JsonResult GetStates(int countryId)
        {
            Country country = _context.Countries
                .Include(c => c.States)
                .FirstOrDefault(c => c.Id == countryId);
            if (country == null)
            {
                return null;
            }

            return Json(country.States.OrderBy(d => d.Name));
        }

        public JsonResult GetCities(int stateId)
        {
            State state = _context.States
                .Include(s => s.Cities)
                .FirstOrDefault(s => s.Id == stateId);
            if (state == null)
            {
                return null;
            }

            return Json(state.Cities.OrderBy(c => c.Name));
        }
    }
}
