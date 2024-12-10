using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using NETCore.Encrypt.Extensions;
using System;
using WebApplication1.Models;
using WebApplication1.ViewModels;


namespace WebApplication1.Controllers 
{
    public class RegisterController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly INotyfService _notify;
        public RegisterController(AppDbContext context, INotyfService service)
        {
            _dbContext = context;
            _notify = service;
        }
        public IActionResult Index()
        {
            _notify.Information("tEST", 3000);
            return View();
        }

        [HttpPost]
        public IActionResult Index(RegisterModel model)
        {
            if (_dbContext.Users.Count(s => s.Email == model.Email) > 0)
            {
                _notify.Error("Girilen E Posta Kayıtlıdır!");
                return View(model);
            }

           /* var rootFolder = _fileProvider.GetDirectoryContents("wwwroot");
            var photoUrl = "-";
            if (model.PhotoFile.Length > 0 && model.PhotoFile != null)
            {
                var filename = Guid.NewGuid().ToString() + Path.GetExtension(model.PhotoFile.FileName);
                var photoPath = Path.Combine(rootFolder.First(x => x.Name == "Photos").PhysicalPath, filename);
                using var stream = new FileStream(photoPath, FileMode.Create);
                model.PhotoFile.CopyTo(stream);
                photoUrl = filename;

            }*/

            var hashedpass = MD5Hash(model.Password);
            var user = new User();
            user.FullName = model.FullName;
            user.Email = model.Email;
            user.PasswordHash = hashedpass;
            user.Role = "User";
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            _notify.Success("Üye Kaydı Yapılmıştır. Oturum Açınız");

            return RedirectToAction("Index","Login");
        }
        public string MD5Hash(string pass)
        {
            var password = pass;
            var hashed = password.MD5();
            return hashed;
        }


    }
}
