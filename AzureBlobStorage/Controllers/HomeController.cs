using Azure.Storage.Blobs;
using AzureBlobStorage.Models;
using Microsoft.AspNetCore.Mvc;

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

        public async Task<IActionResult> Index()
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_configuration.GetConnectionString("nameblob"));
            var blobs = new List<Blob>();
            await foreach (var blobItem in containerClient.GetBlobsAsync())
            {
                blobs.Add(new Blob
                {
                    Name = blobItem.Name,
                    Uri = containerClient.Uri.AbsoluteUri + "/" + blobItem.Name,
                    ContentType = blobItem.Properties.ContentType
                });
            }
            return View(blobs);
        }
        [HttpPost]
        public async Task<IActionResult> Index(IFormFile perfil)
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_configuration.GetConnectionString("nameblob"));
            await containerClient.UploadBlobAsync(perfil.FileName, perfil.OpenReadStream());
            return RedirectToAction("Index");
        }
        [HttpGet("Descargar")]
        public async Task<IActionResult> Descargar(string name)
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_configuration.GetConnectionString("nameblob"));
            var blob = containerClient.GetBlobClient(name);
            if (await blob.ExistsAsync())
            {
                var image = await blob.DownloadAsync();
                return File(image.Value.Content, image.Value.ContentType, name);
            }
            return RedirectToAction("Index");
        }        
        [HttpGet("Eliminar")]
        public async Task<IActionResult> Eliminar(string name)
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_configuration.GetConnectionString("nameblob"));
            var blob = containerClient.GetBlobClient(name);
            if (await blob.ExistsAsync())
            {
                blob = containerClient.GetBlobClient(name);
                await blob.DeleteIfExistsAsync();
            }
            return RedirectToAction("Index");
        }
    }
}