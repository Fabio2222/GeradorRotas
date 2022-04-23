using System.Collections.Generic;
using ApiPessoa.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace ApiPessoa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiPessoaController : ControllerBase
    {

        private readonly ApiPessoaService _pessoaService;
        IWebHostEnvironment _appEnvironment;

        public ApiPessoaController(ApiPessoaService personService, IWebHostEnvironment env)
        {
            _pessoaService = personService;
            _appEnvironment = env;
        }

        [HttpGet]
        public ActionResult<List<Pessoa>> Get()
        {
            var people = _pessoaService.Get();

            if (people == null)
                return BadRequest("Pessoa - A Api esta fora do ar. Tente novamente em instantes.");

            return people;
        }

        [HttpGet("{id}", Name = "GetPerson")]
        public ActionResult<Pessoa> Get(string id)
        {
            var person = _pessoaService.Get(id);

            if (person == null)
                return NotFound();

            return person;
        }

        [HttpGet("name/{name}")]
        public ActionResult<Pessoa> GetByName(string name)
        {
            var person = _pessoaService.GetByName(name);

            if (person == null)
                return NotFound();

            return person;
        }

        [HttpPost]
        public ActionResult<Pessoa> Create(Pessoa pessoa)
        {
            var personInsert = _pessoaService.Create(pessoa);

            if (personInsert == null)
                return BadRequest("Pessoa - A Api esta fora do ar. Tente novamente em instantes.");

            return CreatedAtRoute("GetPerson", new { id = pessoa.Id }, pessoa);
        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, Pessoa pessoaIn)
        {
            Pessoa pessoa = new();

            pessoa = _pessoaService.Get(id);

            if (pessoa == null)
                return NotFound();

            _pessoaService.Update(id, pessoaIn);

            return CreatedAtRoute("GetPerson", new { id = pessoaIn.Id }, pessoaIn);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            Pessoa pessoa = new();

            pessoa = _pessoaService.Get(id);

            if (pessoa == null)
                return NotFound();

            _pessoaService.Remove(pessoa.Id, pessoa);

            return Ok();
        }
    }
}
