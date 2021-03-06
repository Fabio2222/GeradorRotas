using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using GeradorRotas.Frontend.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using RouteAdministration.Frontend.Service;

namespace GeradorRotas.Frontend.Controllers
{
    public class UploadController : Controller
    {
        IWebHostEnvironment _appEnvironment;

        public UploadController(IWebHostEnvironment env)
        {
            _appEnvironment = env;
        }

        public IActionResult Index()
        {
            string usuario = "Anonymous";
            bool authenticate = false;

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                usuario = HttpContext.User.Identity.Name;
                authenticate = true;

                if (HttpContext.User.IsInRole("adm"))
                    ViewBag.Role = "adm";
                else
                    ViewBag.Role = "user";

            }
            else
            {
                usuario = "Não Logado";
                authenticate = false;
                ViewBag.Role = "";
            }

            ViewBag.Usuario = usuario;
            ViewBag.Authenticate = authenticate;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendFile(IFormFile file)
        {
            if (ReadFiles.IsValid("Plan", ".xlsx", _appEnvironment.WebRootPath))
                RemoveFiles.RemoveFromFolder("Plan", ".xlsx", _appEnvironment.WebRootPath);

            var pathFile = Path.GetTempFileName();
            string pathWebRoot = _appEnvironment.WebRootPath;

            if (CheckExcelFile.IsExcel(file))
            {
                if (!await WriteFiles.WriteFileInFolder(file, pathWebRoot))
                    return BadRequest(new { message = "Houve um erro na gravação do arquivo. Por favor, tente novamente." });
            }
            else
            {
                return BadRequest(new { message = "Apenas arquivos com extensão .xls ou .xlsx" });
            }

            ViewData["Resultado"] = $"Um arquivo foi enviado ao servidor, com tamanho total de {file.Length} bytes!";

            ReadFiles.ReOrderExcel("Plan", ".xlsx", _appEnvironment.WebRootPath);

            var headers = ReadFiles.ReadHeaderExcelFile(pathWebRoot);

            List<string> cidades = new();
            List<string> uniqueCidades = new();

            headers.ForEach(header =>
            {
                if (header == "CIDADE")
                {
                    cidades = ReadFiles.ReadColumnExcelFile(pathWebRoot, header);
                }
            });

            cidades.ForEach(cidade =>
            {
                if (!uniqueCidades.Contains(cidade))
                    uniqueCidades.Add(cidade);
            });

            foreach (var cidade in uniqueCidades)
            {
                Cidade newCidade = new() { Nome = cidade, Estado = "SP" };

                await new ConnectToCidadeApi().CreateNewCity(newCidade);
            }

            return View(ViewData);
        }
    }
}
