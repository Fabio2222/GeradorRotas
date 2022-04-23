using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GeradorRotas.Frontend.Service;
using GeradorRotas.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;

namespace GeradorRotas.Controllers
{
    public class CidadeController : Controller
    {
        IWebHostEnvironment _appEnvironment;

        public CidadeController(IWebHostEnvironment appEnvironment)
        {
            _appEnvironment = appEnvironment;
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

            ViewBag.User = user;
            ViewBag.Authenticate = authenticate;

            var cidades = await new ConnectToCidadeApi().GetCities();

            return View(cidades);
        }

        public IActionResult Create()
        {
            List<Pessoa> cidades = ReadFiles.ReadTXTCities(_appEnvironment.WebRootPath);

            ViewBag.Cidades = cidades;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<Cidade>> Create(Cidade cidade)
        {
            var cidadeInsercao = await new ConnectToCidadeApi().CreateNewCity(cidade);

            if (cidadeInsercao == null)
                return BadRequest("Cidade - Houve um erro na gravação da nova cidade. Favor tentar novamente");

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cidade = await new ConnectToCidadeApi().GetCityById(id);

            if (cidade == null)
            {
                return NotFound();
            }

            return View(cidade);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id, Cidade cidade)
        {
            var cidadeRemove = await new ConnectToCidadeApi().RemoveCidade(id);

            if (cidadeRemove.Error != "ok")
                return BadRequest("Cidade - Houve um erro na exclusão da cidade. Favor tentar novamente");

            return RedirectToAction(nameof(Index));
        }
    }
}
