using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using API.Models;
using API.Services;

namespace ProjectNamespace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PoinPelanggaranController : ControllerBase
    {
        private readonly PoinPelanggaranService _service = new PoinPelanggaranService();

        [HttpGet]
        public ActionResult<List<PoinPelanggaran>> GetAll()
        {
            return Ok(_service.GetAll());
        }

        [HttpGet("siswa/{siswaId}")]
        public ActionResult<List<PoinPelanggaran>> GetBySiswaId(int siswaId)
        {
            return Ok(_service.GetBySiswaId(siswaId));
        }

        [HttpGet("{id}")]
        public ActionResult<PoinPelanggaran> GetById(int id)
        {
            var poin = _service.GetById(id);
            if (poin == null) return NotFound();
            return Ok(poin);
        }

        [HttpPost]
        public IActionResult Create([FromBody] PoinPelanggaran poin)
        {
            _service.Add(poin);
            return Ok(poin);
        }

        [HttpPut("{id}/status")]
        public IActionResult UpdateStatus(int id, [FromBody] StatusPelanggaran status)
        {
            var success = _service.UpdateStatus(id, status);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var success = _service.Delete(id);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpGet("siswa/{siswaId}/total-disetujui")]
        public IActionResult GetTotalPoinDisetujui(int siswaId)
        {
            int total = _service.GetTotalPoinDisetujuiBySiswa(siswaId);
            return Ok(new { SiswaId = siswaId, TotalPoin = total });
        }
    }
}
