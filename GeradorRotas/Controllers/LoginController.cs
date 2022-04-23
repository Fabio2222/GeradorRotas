using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using GeradorRotas.Frontend.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace GeradorRotas.Frontend.Controllers
{
    public class LoginController : Controller
    {
        IWebHostEnvironment _appEnvironment;

        public LoginController(IWebHostEnvironment env)
        {
            _appEnvironment = env;
        }

        public IActionResult Index()
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

            ViewBag.User = user;
            ViewBag.Authenticate = authenticate;

            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<Usuario>> Login(Usuario usuarioLogin)
        {
            var usuarioLoginSearch = await new ConnectToUsuarioApi().GetUserByUsername(usuarioLogin.Usuarionome);

            if (usuarioLoginSearch != null)
            {
                if (usuarioLogin.Senha == usuarioLoginSearch.Senha)
                {
                    List<Claim> usuarioClaims = new()
                    {
                        new Claim(ClaimTypes.Name, usuarioLoginSearch.Usuarionome),
                        new Claim("Role", usuarioLoginSearch.Regra),
                        new Claim(ClaimTypes.Role, usuarioLoginSearch.Regra),
                    };

                    var myIdentity = new ClaimsIdentity(usuarioClaims, "User");
                    var usuarioPrincipal = new ClaimsPrincipal(new[] { myIdentity });

                    await HttpContext.SignInAsync(usuarioPrincipal);

                    return RedirectToRoute(new { controller = "Upload", action = "Index" });
                }
            }

            ViewBag.Message = "Usuário ou senha incorretos.";

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<Usuario>> Create(Usuario usuarioLogin)
        {
            var usuarioLoginInsercao = await new ConnectToUsuarioApi().CreateNewUser(usuarioLogin);

            if (usuarioLoginInsercao == null || usuarioLoginInsercao.Error != "")
                return BadRequest("Usuário - Houve um erro na gravação do novo usuário. Favor tentar novamente");

            return RedirectToAction(nameof(Index));
        }
    }
}
