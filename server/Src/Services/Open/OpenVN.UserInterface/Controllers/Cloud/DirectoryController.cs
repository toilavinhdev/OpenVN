using MediatR;
using Microsoft.AspNetCore.Mvc;
using OpenVN.Api;
using SharedKernel.Auth;
using Directory = OpenVN.Domain.Directory;

namespace OpenVN.UserInterface.Controllers
{
    public class DirectoryController : BaseController<Directory>
    {
        public DirectoryController(IMediator mediator, IToken token) : base(mediator, token)
        {
        }

        [HttpGet("all-by-dir/{directoryId}")]
        public async Task<IActionResult> AllInDirectory(string directoryId, CancellationToken cancellationToken = default)
        {
            var query = new GetAllDirectoryQuery(directoryId);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(new ServiceResult { Data =  result, Total = result.Count });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id, CancellationToken cancellationToken = default)
        {
            var query = new GetDirectoryByIdQuery(id);
            return Ok(new SimpleDataResult { Data =  await _mediator.Send(query, cancellationToken) });
        }

        [HttpPost("paging")]
        public async Task<IActionResult> Post(string directoryId, PagingRequest request, CancellationToken cancellationToken = default)
        {
            var query = new GetDirectoryPagingQuery(directoryId, request);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(new ServiceResult { Data =  result, Total = result.Count });
        }

        [HttpPost]
        public async Task<IActionResult> Post(DirectoryDto dto, CancellationToken cancellationToken = default)
        {
            var command = new AddDirectoryCommand(dto);
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(new SimpleDataResult { Data = result });
        }

        [HttpPut]
        public async Task<IActionResult> Put(DirectoryDto dto, CancellationToken cancellationToken = default)
        {
            var command = new UpdateDirectoryCommand(dto);
            await _mediator.Send(command, cancellationToken);
            return Ok(new BaseResponse());
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(List<string> ids, CancellationToken cancellationToken = default)
        {
            var command = new DeleteDirectoryCommand(ids);
            await _mediator.Send(command, cancellationToken);
            return Ok(new BaseResponse());
        }

        [HttpGet("path/{q}")]
        public async Task<IActionResult> GetPath(string q, CancellationToken cancellationToken = default)
        {
            var query = new GetDirectoryPathQuery(q);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(new SimpleDataResult { Data = result });
        }

        [HttpGet("children-node-id/{q}")]
        public async Task<IActionResult> GetChildrenNodeId(string q, CancellationToken cancellationToken = default)
        {
            var query = new GetChildrenNodeDirectoryQuery(q);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(new SimpleDataResult { Data = result });
        }

        [HttpGet("properties/{id}")]
        public async Task<IActionResult> GetProperties(string id, CancellationToken cancellationToken = default)
        {
            var query = new GetDirectoryPropertiesQuery(id);
            return Ok(new SimpleDataResult { Data = await _mediator.Send(query, cancellationToken) });
        }
    }
}
