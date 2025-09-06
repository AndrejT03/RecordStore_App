using Microsoft.AspNetCore.Mvc;
using RecordStore.Domain;
using RecordStore.Domain.DTO;
using RecordStore.Service.Interface;
using System.Diagnostics;

namespace RecordStore.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRecordService _recordService;
        private readonly IArtistService _artistService;

        public HomeController(IRecordService recordService, IArtistService artistService)
        {
            _recordService = recordService;
            _artistService = artistService;
        }

        public IActionResult Index()
        {
            var viewModel = new HomeViewModel
            {
                Records = _recordService.GetAll(),
                Artists = _artistService.GetAll()
            };
            return View(viewModel);
        }
    }
}