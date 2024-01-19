using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Auth;
using SharedKernel.Domain;
using SharedKernel.Libraries;

namespace OpenVN.Api
{
    [AllowAnonymous]
    public class LocationController : BaseController<BaseLocation>
    {

        public LocationController(IMediator mediator, IToken token) : base(mediator, token)
        {
        }

        [HttpGet("provinces")]
        public async Task<IActionResult> GetProvinces(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetProvincesQuery(), cancellationToken);
            return Ok(new ServiceResult { Data = result, Total = result.Count });
        }

        [HttpGet("districts")]
        public async Task<IActionResult> GetDistricts(string provinceId, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetDistrictsQuery(provinceId), cancellationToken);
            return Ok(new ServiceResult { Data = result, Total = result.Count });
        }

        [HttpGet("wards")]
        public async Task<IActionResult> GetWards(string districtId, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetWardsQuery(districtId), cancellationToken);
            return Ok(new ServiceResult { Data = result, Total = result.Count });
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search(SearchParam param, CancellationToken cancellationToken = default)
        {
            var query = param.Value;
            var result = await _mediator.Send(new SearchLocationQuery(query), cancellationToken);

            query = query == null ? "" : query.ToLower();
            query = query.ToLower().ViToEn();

            var count = result.Count(x => x.Name.ViToEn().ToLower().Contains(query));
            foreach (var item in result)
            {
                count += item.Children.Count(x => x.Name.ViToEn().ToLower().Contains(query));
                foreach (var item2 in item.Children)
                {
                    count += item2.ChildrenCount;
                }
            }

            return Ok(new ServiceResult { Data = result, Total = count });
        }

        [HttpGet("paging")]
        public async Task<IActionResult> GetRank(int page, int size, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetRankQuery(new PagingRequest(page, size)), cancellationToken);
            var sessionIp = AuthUtility.TryGetIP(_token.Context.HttpContext.Request);
            return Ok(new ServiceResult { Data = new RankResultDto { Ranks = result.Data.ToList(), SessionIp = sessionIp}, Total = result.Count });
        }
    }
}
