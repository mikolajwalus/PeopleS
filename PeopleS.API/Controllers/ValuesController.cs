using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PeopleS.API.Data;
using PeopleS.API.Models;

namespace PeopleS.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ValuesController : ControllerBase
    {
        private readonly IPeopleSRepository _repo;
        public ValuesController(IPeopleSRepository repo)
        {
            _repo = repo;
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetValue(int id)
        {
            var result = await _repo.GetValue(id);

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetValues()
        {
            var result = await _repo.GetValues();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddValue(Value value)
        {
            if(value != null) _repo.Add(value);

            if(await _repo.SaveAll()) return Ok(value);

            return BadRequest();
        }
    }
}