using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MottuCrudAPI.Domain.Entities;   // Motocicleta
using MottuCrudAPI.Persistense;       // Patio
using MottuCrudAPI.DTO.Response;
using MottuCrudAPI.DTO.Request;
using MottuCrudAPI.WebApi.SwaggerExamples;

namespace MottuCrudAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class MotocicletasController : ControllerBase
    {
        private readonly IMotocicletaRepository _repo;
        private const int MaxPageSize = 50;

        public MotocicletasController(IMotocicletaRepository repo) => _repo = repo;

        [HttpGet(Name = "GetMotocicletas")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize is < 1 or > MaxPageSize ? 10 : pageSize;

            var (items, total) = await _repo.GetPagedAsync(pageNumber, pageSize);

            var responses = items.Select(m => new MotocicletaResponse
            {
                Id = m.Id,
                Placa = m.Placa,
                Modelo = m.Modelo,
                Status = m.Status,
                PatioId = m.PatioId, // agora Guid
                Links = new[]
                {
                    new LinkDto("self", Url.Link("GetMotocicletaById", new { id = m.Id })!, "GET"),
                    new LinkDto("update", Url.Link("UpdateMotocicleta", new { id = m.Id })!, "PUT"),
                    new LinkDto("delete", Url.Link("DeleteMotocicleta", new { id = m.Id })!, "DELETE")
                }
            });

            var paged = new PagedList<MotocicletaResponse>(responses, total, pageNumber, pageSize);

            var collectionLinks = new[]
            {
                new LinkDto("self", Url.Link("GetMotocicletas", new { pageNumber, pageSize })!, "GET"),
                new LinkDto("first", Url.Link("GetMotocicletas", new { pageNumber = 1, pageSize })!, "GET"),
                new LinkDto("last", Url.Link("GetMotocicletas", new { pageNumber = Math.Max(1, (int)Math.Ceiling(total/(double)pageSize)), pageSize })!, "GET")
            };

            return Ok(new { paged.PageNumber, paged.PageSize, paged.TotalCount, paged.TotalPages, links = collectionLinks, items = paged.Items });
        }

        [HttpGet("{id:int}", Name = "GetMotocicletaById")]
        [ProducesResponseType(typeof(MotocicletaResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(MotocicletaResponseExample))]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var entity = await _repo.GetAsync(id);
            if (entity is null) return NotFound();

            var dto = new MotocicletaResponse
            {
                Id = entity.Id,
                Placa = entity.Placa,
                Modelo = entity.Modelo,
                Status = entity.Status,
                PatioId = entity.PatioId, // Guid
                Links = new[]
                {
                    new LinkDto("self", Url.Link("GetMotocicletaById", new { id = entity.Id })!, "GET"),
                    new LinkDto("update", Url.Link("UpdateMotocicleta", new { id = entity.Id })!, "PUT"),
                    new LinkDto("delete", Url.Link("DeleteMotocicleta", new { id = entity.Id })!, "DELETE")
                }
            };
            return Ok(dto);
        }

        [HttpPost(Name = "CreateMotocicleta")]
        [SwaggerRequestExample(typeof(MotocicletaRequest), typeof(MotocicletaRequestExample))]
        [SwaggerResponseExample(StatusCodes.Status201Created, typeof(MotocicletaResponseExample))]
        [ProducesResponseType(typeof(MotocicletaResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] MotocicletaRequest request)
        {
            if (request is null)
            {
                return BadRequest(new ValidationProblemDetails(new Dictionary<string, string[]>
                {
                    ["body"] = new[] { "O corpo da requisição não pode ser nulo." }
                })
                {
                    Title = "Requisição inválida",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var entity = new Motocicleta
            {
                Placa = request.Placa,
                Modelo = request.Modelo,
                PatioId = request.PatioId // Guid
            };
            entity = await _repo.AddAsync(entity);

            var dto = new MotocicletaResponse
            {
                Id = entity.Id,
                Placa = entity.Placa,
                Modelo = entity.Modelo,
                Status = entity.Status,
                PatioId = entity.PatioId, // Guid
                Links = new[]
                {
                    new LinkDto("self", Url.Link("GetMotocicletaById", new { id = entity.Id })!, "GET"),
                    new LinkDto("update", Url.Link("UpdateMotocicleta", new { id = entity.Id })!, "PUT"),
                    new LinkDto("delete", Url.Link("DeleteMotocicleta", new { id = entity.Id })!, "DELETE")
                }
            };

            return CreatedAtRoute("GetMotocicletaById", new { id = entity.Id }, dto);
        }

        [HttpPut("{id:int}", Name = "UpdateMotocicleta")]
        [SwaggerRequestExample(typeof(MotocicletaRequest), typeof(MotocicletaRequestExample))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] MotocicletaRequest request)
        {
            if (request is null)
            {
                return BadRequest(new ValidationProblemDetails(new Dictionary<string, string[]>
                {
                    ["body"] = new[] { "O corpo da requisição não pode ser nulo." }
                })
                {
                    Title = "Requisição inválida",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var entity = await _repo.GetAsync(id);
            if (entity is null) return NotFound();

            entity.Placa = request.Placa;
            entity.Modelo = request.Modelo;
            entity.PatioId = request.PatioId; // Guid
            await _repo.UpdateAsync(entity);

            return NoContent();
        }

        [HttpDelete("{id:int}", Name = "DeleteMotocicleta")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var entity = await _repo.GetAsync(id);
            if (entity is null) return NotFound();
            await _repo.DeleteAsync(entity);
            return NoContent();
        }
    }
}
