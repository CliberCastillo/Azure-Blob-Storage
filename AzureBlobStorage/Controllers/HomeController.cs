using Azure.Storage.Blobs;
using AzureBlobStorage.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AzureBlobStorage.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly BlobServiceClient _blobServiceClient;

        public HomeController(
            ILogger<HomeController> logger,
            IConfiguration configuration,
            BlobServiceClient blobServiceClient)
        {
            _logger = logger;
            _configuration = configuration;
            _blobServiceClient = blobServiceClient;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> InsertarEmpleado(string nombre, string apellido, int edad, IFormFile perfil)
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_configuration.GetConnectionString("nameblob"));
            BlobClient blobClient = containerClient.GetBlobClient(perfil.FileName);
            await blobClient.UploadAsync(perfil.OpenReadStream());
            return RedirectToAction("Index");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}