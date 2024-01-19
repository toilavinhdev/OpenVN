using MediatR;
using Microsoft.AspNetCore.Mvc;
using OpenVN.Domain;
using SharedKernel.Auth;

namespace OpenVN.Api
{
    public class SpendingController : BaseController<Spending>
    {
        public SpendingController(IMediator mediator, IToken token) : base(mediator, token)
        {
        }

        [HttpPost]
        public async Task<IActionResult> AddSpending(SpendingDto spending, CancellationToken cancellationToken = default)
        {
            var command = new CreateSpendingCommand(spending);
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(new SimpleDataResult { Data = result });
        }

        //[HttpPost("paging")]
        //public async Task<IActionResult> Paging(PagingRequest request, CancellationToken cancellationToken = default)
        //{
        //    var query = new PagingSpendingQuery(request);
        //    var result = await _mediator.Send(query, cancellationToken);
        //    return Ok(new ServiceResult { Data = result.Data, Total = result.Count });
        //}

        //[HttpPost("paging-with-subs")]
        //public async Task<IActionResult> PagingWithSubs(PagingRequest request, CancellationToken cancellationToken = default)
        //{
        //    var query = new PagingWithSubSpendingQuery(request);
        //    var result = await _mediator.Send(query, cancellationToken);
        //    return Ok(new ServiceResult { Data = result.Data, Total = result.Count });
        //}
    }
}
