using System.Collections.Generic;
using ApiEquipe.Service;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace ApiEquipe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiEquipeController : ControllerBase
    {
        private readonly ApiEquipeService _equipeService;

        public ApiEquipeController(ApiEquipeService equipeService)
        {
            _equipeService = equipeService;
        }

        [HttpGet]
        public ActionResult<List<Equipe>> Get()
        {
            var equipe = _equipeService.Get();

            if (equipe == null)
                return BadRequest("Equipe - A Api esta fora do ar. Tente novamente em instantes.");

            return equipe;
        }

        [HttpGet("{id}", Name = "GetEquipe")]
        public ActionResult<Equipe> Get(string id)
        {
            var equipe = _equipeService.Get(id);

            if (equipe == null)
                return NotFound();

            return equipe;
        }

        [HttpGet("city/{city}")]
        public ActionResult<List<Equipe>> GetCidade(string cidade)
        {
            var equipes = _equipeService.GetCidade(cidade);

            if (equipes == null)
                return NotFound();

            return equipes;
        }

        [HttpGet("equip/{equipName}")]
        public ActionResult<Equipe> GetEquipesByNomeEquipe(string nomeEquipe)
        {
            var equipe = _equipeService.GetEquipesByEquipeNome(nomeEquipe);

            if (equipe == null)
                return NotFound();

            return equipe;
        }

        [HttpPost]
        public ActionResult<Equipe> Create(Equipe equipe)
        {
            var equipeInsertion = _equipeService.Create(equipe);

            if (equipeInsertion == null)
                return BadRequest("Equipe - A Api esta fora do ar. Tente novamente em instantes.");

            return CreatedAtRoute("GetEquipe", new { id = equipe.Id }, equipe);
        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, Equipe equipeIn)
        {
            Equipe equipe = new();

            equipe = _equipeService.Get(id);

            if (equipe == null)
                return NotFound();

            _equipeService.Update(id, equipeIn);

            return CreatedAtRoute("GetEquipe", new { id = equipeIn.Id }, equipeIn);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            Equipe equipe = new();

            equipe = _equipeService.Get(id);

            if (equipe == null)
                return NotFound();

            _equipeService.Remove(equipe.Id, equipe);

            return Ok();
        }
    }
}

