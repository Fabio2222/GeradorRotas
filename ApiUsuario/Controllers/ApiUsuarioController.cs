using System.Collections.Generic;
using System.Threading.Tasks;
using ApiUsuario.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace ApiUsuario.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiUsuarioController : ControllerBase
    {
        private readonly ApiUsuarioService _usuarioService;
        IWebHostEnvironment _appEnvironment;

        public ApiUsuarioController(ApiUsuarioService usuarioService, IWebHostEnvironment env)
        {
            _usuarioService = usuarioService;
            _appEnvironment = env;
        }

        [HttpGet]
        public ActionResult<List<Usuario>> Get()
        {
            var usuario = _usuarioService.Get();

            if (usuario == null)
                return BadRequest("User - A Api esta fora do ar. Tente novamente em instantes.");

            return usuario;
        }

        [HttpGet("{id}", Name = "GetUser")]
        public ActionResult<Usuario> Get(string id)
        {
            var usuario = _usuarioService.Get(id);

            if (usuario == null)
                return NotFound();

            return usuario;
        }

        [HttpGet("login/{usuarionome}")]
        public ActionResult<Usuario> GetUserByUsername(string usuarionome)
        {
            var usuario = _usuarioService.GetUserByUsername(usuarionome);

            if (usuario == null)
                return NotFound();

            return usuario;
        }

        [HttpPost]
        public async Task<ActionResult<Usuario>> Create(Usuario usuario)
        {
            var usuarioInsertion = await _usuarioService.Create(usuario);

            if (usuarioInsertion.Error == "temUsuario") 
                return BadRequest("Usuário - O usuário já está cadastrado no sistema.");

            if (usuarioInsertion == null)
                return BadRequest("User - A Api esta fora do ar. Tente novamente em instantes.");

            return CreatedAtRoute("GetUser", new { id = usuario.Id }, usuario);
        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, Usuario usuarioIn)
        {
            Usuario usuario = new();

            usuario = _usuarioService.Get(id);

            if (usuario == null)
                return NotFound();

            _usuarioService.Update(id, usuarioIn);

            return CreatedAtRoute("GetUsuario", new { id = usuarioIn.Id }, usuarioIn);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            Usuario usuario = new();

            usuario = _usuarioService.Get(id);

            if (usuario == null)
                return NotFound();

            _usuarioService.Remove(usuario.Id, usuario);

            return Ok();
        }
    }
}
