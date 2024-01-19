using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OpenVN.TrackingApi.Dto;
using SharedKernel.Application;
using SharedKernel.Domain;
using SharedKernel.Libraries;
using SharedKernel.MySQL;

namespace OpenVN.TrackingApi
{
    [Route("api/v1")]
    [ApiController]
    public class TrackingController : ControllerBase
    {
        private readonly IDbConnection _dbConnection;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _accessor;

        public TrackingController(IDbConnection dbConnection, IMapper mapper, IHttpContextAccessor accessor)
        {
            _dbConnection = dbConnection;
            _mapper = mapper;
            _accessor = accessor;
        }

        [HttpPost("tracking-dynamic")]
        public async Task<IActionResult> TrackingDynamic(TrackingDto tracking)
        {
            var entity = _mapper.Map<Tracking>(tracking);
            var properties = typeof(Tracking).GetProperties().Where(p => p.GetIndexParameters().Length == 0);
            var columns = string.Join(", ", properties.Select(p => $"`{p.Name}`"));
            var parameters = string.Join(", ", properties.Select(p => $"@{p.Name}"));
            var cmd = @$"INSERT INTO {new Tracking().GetTableName()} ( {columns} ) VALUES ( {parameters} );";

            entity.Id = AuthUtility.GenerateSnowflakeId();
            entity.CreatedDate = DateHelper.Now;
            entity.Ip = AuthUtility.TryGetIP(_accessor.HttpContext.Request);

            await _dbConnection.ExecuteAsync(cmd, entity, autoCommit: true);
            return Ok(new BaseResponse());
        }
    }
}