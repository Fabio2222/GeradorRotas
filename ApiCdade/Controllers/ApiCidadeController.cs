using System.Collections.Generic;
using ApiCdade.Service;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace ApiCdade.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiCidadeController : ControllerBase
    {
        private readonly ApiCidadeService _cidadeService;
        public ApiCidadeController(ApiCidadeService cidadeService)
        {
            _cidadeService = cidadeService;
        }

        [HttpGet]
        public ActionResult<List<Cidade>> Get()
        {
            var cidade = _cidadeService.Get();

            if (cidade == null)
                return BadRequest("Cidade - A Api esta fora do ar. Tente novamente em instantes.");

            return cidade;
        }

        [HttpGet("{id}", Name = "GetCidade")]
        public ActionResult<Cidade> Get(string id)
        {
            var cidade = _cidadeService.Get(id);

            if (cidade == null)
                return NotFound();

            return cidade;
        }

        [HttpPost]
        public ActionResult<Cidade> Create(Cidade cidade)
        {
            var insercaoCidade = _cidadeService.Create(cidade);

            if (insercaoCidade == null)
                return BadRequest("Cidade - A Api esta fora do ar. Tente novamente em instantes.");
            if (insercaoCidade.Id == null)
                return BadRequest("Cidade - A cidade já está cadastrada no banco de dados.");

            return CreatedAtRoute("GetCidade", new { id = cidade.Id }, cidade);
        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, Cidade cidadeIn)
        {
            Cidade cidade = new();

            cidade = _cidadeService.Get(id);

            if (cidade == null)
                return NotFound();

            _cidadeService.Update(id, cidadeIn);

            return CreatedAtRoute("GetCidade", new { id = cidadeIn.Id }, cidadeIn);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            Cidade cidade = new();

            cidade = _cidadeService.Get(id);

            if (cidade == null)
                return NotFound();

            _cidadeService.Remove(cidade.Id, cidade);

            return Ok();
        }
    }
}