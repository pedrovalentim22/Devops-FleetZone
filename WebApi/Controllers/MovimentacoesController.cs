using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MottuCrudAPI.WebApi.SwaggerExamples;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MottuCrudAPI.Domain.Entities; // Movimentacao, Motocicleta
using MottuCrudAPI.Persistense;     // Patio correto
using MottuCrudAPI.DTO.Response;
using MottuCrudAPI.DTO.Request;

namespace MottuCrudAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class MovimentacoesController : ControllerBase
    {
        private readonly IMovimentacaoRepository _repo;
        private const int MaxPageSize = 50;

        public MovimentacoesController(IMovimentacaoRepository repo) => _repo = repo;

        [HttpGet(Name = "GetMovimentacoes")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize is < 1 or > MaxPageSize ? 10 : pageSize;

            var (items, total) = await _repo.GetPagedAsync(pageNumber, pageSize);

            var responses = items.Select(m => new MovimentacaoResponse
            {
                Id = m.Id,
                DataHora = m.DataHora,
                Tipo = m.Tipo,
                Observacao = m.Observacao,
                MotocicletaId = m.MotocicletaId,
                PatioId = m.PatioId, // Guid
                Links = new[]
                {
                    new LinkDto("self", Url.Link("GetMovimentacaoById", new { id = m.Id })!, "GET"),
                    new LinkDto("update", Url.Link("UpdateMovimentacao", new { id = m.Id })!, "PUT"),
                    new LinkDto("delete", Url.Link("DeleteMovimentacao", new { id = m.Id })!, "DELETE")
                }
            });

            var paged = new PagedList<MovimentacaoResponse>(responses, total, pageNumber, pageSize);

            var collectionLinks = new[]
            {
                new LinkDto("self", Url.Link("GetMovimentacoes", new { pageNumber, pageSize })!, "GET"),
                new LinkDto("first", Url.Link("GetMovimentacoes", new { pageNumber = 1, pageSize })!, "GET"),
                new LinkDto("last", Url.Link("GetMovimentacoes", new { pageNumber = Math.Max(1, (int)Math.Ceiling(total/(double)pageSize)), pageSize })!, "GET")
            };

            return Ok(new { paged.PageNumber, paged.PageSize, paged.TotalCount, paged.TotalPages, links = collectionLinks, items = paged.Items });
        }

        [HttpGet("{id:int}", Name = "GetMovimentacaoById")]
        [ProducesResponseType(typeof(MovimentacaoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(MovimentacaoResponseExample))]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var entity = await _repo.GetAsync(id);
            if (entity is null) return NotFound();

            var dto = new MovimentacaoResponse
            {
                Id = entity.Id,
                DataHora = entity.DataHora,
                Tipo = entity.Tipo,
                Observacao = entity.Observacao,
                MotocicletaId = entity.MotocicletaId,
                PatioId = entity.PatioId, // Guid
                Links = new[]
                {
                    new LinkDto("self", Url.Link("GetMovimentacaoById", new { id = entity.Id })!, "GET"),
                    new LinkDto("update", Url.Link("UpdateMovimentacao", new { id = entity.Id })!, "PUT"),
                    new LinkDto("delete", Url.Link("DeleteMovimentacao", new { id = entity.Id })!, "DELETE")
                }
            };
            return Ok(dto);
        }

        [HttpPost(Name = "CreateMovimentacao")]
        [SwaggerRequestExample(typeof(MovimentacaoRequest), typeof(MovimentacaoRequestExample))]
        [SwaggerResponseExample(StatusCodes.Status201Created, typeof(MovimentacaoResponseExample))]
        [ProducesResponseType(typeof(MovimentacaoResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] MovimentacaoRequest request)
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

            var entity = new Movimentacao
            {
                Tipo = request.Tipo,
                Observacao = request.Observacao,
                MotocicletaId = request.MotocicletaId,
                PatioId = request.PatioId // Guid
            };
            entity = await _repo.AddAsync(entity);

            var dto = new MovimentacaoResponse
            {
                Id = entity.Id,
                DataHora = entity.DataHora,
                Tipo = entity.Tipo,
                Observacao = entity.Observacao,
                MotocicletaId = entity.MotocicletaId,
                PatioId = entity.PatioId, // Guid
                Links = new[]
                {
                    new LinkDto("self", Url.Link("GetMovimentacaoById", new { id = entity.Id })!, "GET"),
                    new LinkDto("update", Url.Link("UpdateMovimentacao", new { id = entity.Id })!, "PUT"),
                    new LinkDto("delete", Url.Link("DeleteMovimentacao", new { id = entity.Id })!, "DELETE")
                }
            };

            return CreatedAtRoute("GetMovimentacaoById", new { id = entity.Id }, dto);
        }

        [HttpPut("{id:int}", Name = "UpdateMovimentacao")]
        [SwaggerRequestExample(typeof(MovimentacaoRequest), typeof(MovimentacaoRequestExample))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] MovimentacaoRequest request)
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

            entity.Tipo = request.Tipo;
            entity.Observacao = request.Observacao;
            entity.MotocicletaId = request.MotocicletaId;
            entity.PatioId = request.PatioId; // Guid
            await _repo.UpdateAsync(entity);

            return NoContent();
        }

        [HttpDelete("{id:int}", Name = "DeleteMovimentacao")]
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
