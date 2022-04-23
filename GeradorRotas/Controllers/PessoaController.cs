using System.Threading.Tasks;
using GeradorRotas.Frontend.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using ServiceStack;

namespace GeradorRotas.Frontend.Controllers
{
    public class PessoaController : Controller
    {
        public async Task<IActionResult> Index()
        {
            string usuario = "Anônimo";
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

            ViewBag.User = usuario;
            ViewBag.Authenticate = authenticate;

            var people = await new ConnectToPessoaApi().GetPeople();

            return View(people);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<Pessoa>> Create(Pessoa pessoa)
        {
            var pessoaInsercao = await new ConnectToPessoaApi().CreateNewPerson(pessoa);

            if (pessoaInsercao == null)
                return BadRequest("Pessoa - Houve um erro na gravação da nova pessoa. Favor tentar novamente");

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pessoa = await new ConnectToPessoaApi().GetPersonById(id);

            if (pessoa == null)
            {
                return NotFound();
            }
            return View(pessoa);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Pessoa pessoa)
        {
            var pessoaEdit = await new ConnectToPessoaApi().EditPerson(pessoa);

            if (pessoaEdit == null)
                return BadRequest("Pessoa - Houve um erro na edição da pessoa. Favor tentar novamente");

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pessoa = await new ConnectToPessoaApi().GetPersonById(id);

            if (pessoa == null)
            {
                return NotFound();
            }

            return View(pessoa);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id, Pessoa pessoa)
        {
            var pessoaRemove = await new ConnectToPessoaApi().RemovePerson(id);

            if (pessoaRemove.Error != "ok")
                return BadRequest("Pessoa - Houve um erro na exclusão do usuário. Favor tentar novamente");

            return RedirectToAction(nameof(Index));
        }
    }
}


