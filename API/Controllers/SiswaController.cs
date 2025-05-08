using Microsoft.AspNetCore.Mvc;
using API.Models;
using API.Services;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SiswaController : ControllerBase
    {
        private readonly SiswaService _service = new SiswaService();

        [HttpGet]
        public IActionResult GetAll() => Ok(_service.GetAll());

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var siswa = _service.GetById(id);
            return siswa == null ? NotFound() : Ok(siswa);
        }

        [HttpPost]
        public IActionResult Add([FromBody] Siswa siswa)
        {
            _service.Add(siswa);
            return CreatedAtAction(nameof(GetById), new { id = siswa.Id }, siswa);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Siswa siswa)
        {
            _service.Update(id, siswa);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _service.Delete(id);
            return NoContent();
        }

        [HttpPost("{id}/pelanggaran")]
        public IActionResult TambahPelanggaran(int id, [FromBody] PoinPelanggaran pelanggaran)
        {
            _service.TambahPelanggaran(id, pelanggaran);
            return Ok(new { message = "Pelanggaran ditambahkan", pelanggaran });
        }
    }
}
