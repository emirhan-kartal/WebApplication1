using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using WebApplication1.ViewModels;
using WebApplication1.Repositories;
using NETCore.Encrypt.Extensions;
using AspNetCoreHero.ToastNotification.Helpers;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly IFileRepository _fileRepository;
        private readonly INotyfService _notify;


        public AdminController(AppDbContext dbContext, IFileRepository fileRepository, INotyfService service)
        {
            ViewData["Layout"] = "_AdminLayout";
            _dbContext = dbContext;
            _fileRepository = fileRepository;
            _notify = service;
        }

        public async Task<IActionResult> Index()
        {
            var files = await _fileRepository.GetAllFilesAsync();

            return View(files);
        }
        public async Task<IActionResult> UserList()
        {
            var users = await _dbContext.Users.Include(e => e.UploadedFiles).ToListAsync();
            var firstAdminUser = await _dbContext.Users
    .Where(u => u.Role == "Admin")
    .OrderBy(u => u.Id)
    .FirstOrDefaultAsync();
            var userModelViews = users.Select(user => new UserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                UploadedFilesCount = user.UploadedFiles?.Count ?? 0,
                isSuperAdmin = firstAdminUser.Id == user.Id ? true : false
            }).ToList();
            return View(userModelViews);
        }


        public async Task<IActionResult> Delete(int id)
        {
            var file = await _fileRepository.GetFileByIdAsync(id);

            if (file == null)
                return NotFound();

            return View(file);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var file = await _fileRepository.GetFileByIdAsync(id);

            if (file == null)
                return NotFound();

            await _fileRepository.DeleteFileAsync(file.Id);

            return RedirectToAction(nameof(Index));
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.FullName == model.UploadedByEmail);

            if (user == null)
            {
                ModelState.AddModelError("", $" {model.UploadedByEmail}, böyle bir kullanıcı bulunamadı!");
                return View(model);
            }

            var newFile = new WebApplication1.Models.File
            {
                FileName = model.FileName,
                FilePath = model.FilePath,
                UploadedAt = DateTime.UtcNow,
                UploadedByUserId = user.Id
            };

            await _fileRepository.AddFileAsync(newFile);

            return RedirectToAction(nameof(Index));
        }
        public IActionResult CreateUser()
        {
            return View();
        }

        // POST: Admin/Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Create a new User entity from the view model
                var user = new User
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    Role = model.Role,
                    // Note: For password, you should use a proper password hashing mechanism
                    PasswordHash = (model.PasswordHash).MD5()
                };
                if (UserExists(model.Id))
                {
                    TempData["ErrorMessage"] = "Kullanıcı zaten mevcut.";
                        _notify.Error("Kullanıcı Zaten Mevcut.");
                        return RedirectToAction(nameof(CreateUser));
                }

                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();

                TempData["SuccessMessage"] = "Kullanıcı oluşturuldu.";
                return RedirectToAction("UserList", "Admin");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: Admin/Users/Edit/5
        public async Task<IActionResult> EditUser(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _dbContext.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var firstAdminUser = await _dbContext.Users
    .Where(u => u.Role == "Admin")
    .OrderBy(u => u.Id)
    .FirstOrDefaultAsync();
                var model = new UpdateUserViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = user.Role,
                    UploadedFilesCount = user.UploadedFiles?.Count ?? 0,
                    isSuperAdmin = firstAdminUser.Id == user.Id ? true : false
                };

                return View(model);
            } else
            {
                _notify.Error("Geçersiz bilgi.");
                return View();

            }
        }

        // POST: Admin/Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(int id, UpdateUserViewModel model)
        {
            System.Diagnostics.Debug.WriteLine("TEST" + ModelState.IsValid);
            System.Diagnostics.Debug.WriteLine("TEST" + model.ToJson());
            if (id != model.Id)
            {
                return NotFound();
            }
            System.Diagnostics.Debug.WriteLine("TEST" + ModelState.IsValid);
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _dbContext.Users.FindAsync(id);
                    if (user == null)
                    {
                        return NotFound();
                    }

                    // Update user properties
                    user.FullName = model.FullName;
                    user.Email = model.Email;
                    user.Role = model.Role;


                    // Only update password if a new password is provided


                    _dbContext.Update(user);
                    await _dbContext.SaveChangesAsync();

                    TempData["SuccessMessage"] = "User updated successfully.";

                    return RedirectToAction("UserList", "Admin");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        Console.WriteLine("Error");
                        throw;
                    }
                }
            }

            return View(model);
        }

        // GET: Admin/Users/Delete/5
        public async Task<IActionResult> DeleteUser(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _dbContext.Users
                .FirstOrDefaultAsync(m => m.Id == id);

            if (user == null)
            {
                return NotFound();
            }
            var firstAdminUser = await _dbContext.Users
              .Where(u => u.Role == "Admin") // Filter for Admin role
              .OrderBy(u => u.Id)           // Order by ID in ascending order
              .FirstOrDefaultAsync();       // Get the first match or null if none found
            if (firstAdminUser.Id == user.Id)
            {
                TempData["ErrorMessage"] = "Bu kişi SüperAdmin.";
                _notify.Error("Bu kişi SüperAdmin");

                return RedirectToAction("UserList", "Admin");

            }
            var model = new UserViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                UploadedFilesCount = user.UploadedFiles?.Count ?? 0
            };

            return View(model);
        }

        // POST: Admin/Users/Delete/5
        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUserConfirmed(int id)
        {
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
            _notify.Success("Kullanıcı başarıyla silindi.");
            TempData["SuccessMessage"] = "Kullanıcı başarıyla silindi.";
            return RedirectToAction("UserList", "Admin");
        }

        // Helper method to check if user exists
        private bool UserExists(int id)
        {
            return _dbContext.Users.Any(e => e.Id == id);
        }
    }
}


