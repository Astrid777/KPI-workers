using CustomIdentityApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace CustomIdentityApp.Controllers
{
    public class FilesController : Controller
    {
        private readonly ApplicationContext _context;

        public FilesController(ApplicationContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var fileuploadViewModel = await LoadAllFiles();
            ViewBag.Message = TempData["Message"];

            return View(fileuploadViewModel);
        }


        [HttpPost]
        public async Task<IActionResult> UploadToDatabase(List<IFormFile> files, string description)
        {
            foreach (var file in files)
            {
                User user = _context.Users.Where(w => w.Email == User.Identity.Name).FirstOrDefault();

                var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                var extension = Path.GetExtension(file.FileName);

                var fileModel = new FileOnDatabaseModel
                {
                    CreatedOn = DateTime.UtcNow,
                    FileType = file.ContentType,
                    Extension = extension,
                    Name = fileName,
                    Description = description,
                    User = user,
                    UserId = user.Id,
                    UserName = user.Email
                };

                using (var dataStream = new MemoryStream())
                {
                    await file.CopyToAsync(dataStream);
                    fileModel.Data = dataStream.ToArray();
                }

                _context.Files.Add(fileModel);

                _context.SaveChanges();
            }
            TempData["Message"] = "Файл успешно загружен в базу";

            return RedirectToAction("Index");
        }

        private async Task<FileUploadViewModel> LoadAllFiles()
        {
            var viewModel = new FileUploadViewModel();
            viewModel.FilesOnDatabase = await _context.Files.ToListAsync();

            return viewModel;
        }

        public async Task<IActionResult> DownloadFileFromDatabase(int id)
        {

            var file = await _context.Files.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (file == null) return null;
            return File(file.Data, file.FileType, file.Name + file.Extension);
        }


        public async Task<IActionResult> DeleteFileFromDatabase(int id)
        {

            var file = await _context.Files.Where(x => x.Id == id).FirstOrDefaultAsync();
            _context.Files.Remove(file);
            _context.SaveChanges();
            TempData["Message"] = $"Файл удален {file.Name + file.Extension} из базы данных";
            return RedirectToAction("Index");
        }
    }
}
