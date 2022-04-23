using System.Collections.Generic;
using System.Threading.Tasks;
using GeradorRotas.Frontend.Service;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace GeradorRotas.Frontend.Controllers
{
    public class EquipeController : Controller
    {
        public async Task<IActionResult> Index()
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

            var equipes = await new ConnectToEquipeApi().GetEquipes();

            return View(equipes);
        }

        public async Task<IActionResult> Create()
        {
            var pessoas = await new ConnectToPessoaApi().GetPeople();
            var cidades = await new ConnectToCidadeApi().GetCities();

            List<Pessoa> pessoasSemEquipe = pessoas.FindAll(pessoa => pessoa.NomeEquipe  == "");

            ViewBag.Pessoas = pessoasSemEquipe;
            ViewBag.Cidades = cidades;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<Equipe>> Create(Equipe equipe, List<string> selectedPessoa)
        {
            List<Pessoa> pessoasChoices = new();

            foreach (string selPessoas in selectedPessoa)
            {
                Pessoa pessoa = await new ConnectToPessoaApi().GetPersonByName(selPessoas);

                pessoa.NomeEquipe = equipe.Nome;

                pessoasChoices.Add(pessoa);
            }

            equipe.Pessoas = pessoasChoices;

            var equipeInsercao = await new ConnectToEquipeApi().CreateNewEquip(equipe);

            if (equipeInsercao == null)
                return BadRequest("Equipe - Houve um erro na gravação da nova equipe. Favor tentar novamente");

            foreach (Pessoa pessoa in pessoasChoices)
            {
                await new ConnectToPessoaApi().EditPerson(pessoa);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipe = await new ConnectToEquipeApi().GetEquipById(id);

            if (equipe == null)
            {
                return NotFound();
            }

            return View(equipe);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id, Equipe equipeRemove)
        {
            var equipe = await new ConnectToEquipeApi().GetEquipById(id);
            var equipeRemoved = await new ConnectToEquipeApi().RemoveEquip(id);

            if (equipeRemoved.Error != "ok")
                return BadRequest("Equipe - Houve um erro na exclusão da equipe. Favor tentar novamente");

            foreach (Pessoa pessoa in equipe.Pessoas)
            {
                pessoa.NomeEquipe = "";

                await new ConnectToPessoaApi().EditPerson(pessoa);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
