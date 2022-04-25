using System.Diagnostics;
using System.Threading.Tasks;
using GeradorRotas.Frontend.Service;
using GeradorRotas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;

namespace GeradorRotas.Frontend.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            string user = "Anonymous";
            bool authenticate = false;

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                user = HttpContext.User.Identity.Name;
                authenticate = true;

                if (HttpContext.User.IsInRole("adm"))
                    ViewBag.Role = "adm";
                else
                    ViewBag.Role = "user";

            }
            else
            {
                user = "Não Logado";
                authenticate = false;
                ViewBag.Role = "";
            }
            var usuarios = await new ConnectToUsuarioApi().GetUsers();
            if (usuarios.Count < 1)
            {
                await new ConnectToUsuarioApi().CreateNewUser(new Usuario() { Usuarionome = "adm", Senha = "adm", Regra = "adm"});
            }
            ViewBag.User = user;
            ViewBag.Authenticate = authenticate;

            return View();
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
