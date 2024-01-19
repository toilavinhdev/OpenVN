using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Auth;
using SharedKernel.Libraries;
using System.Reflection;

namespace OpenVN.Api
{
    [Authorize]
    [Route("api/v1/opensync/[controller]")]
    [ApiController]
    public class BaseController<TEntity> : ControllerBase
    {
        protected readonly IMediator _mediator;
        protected readonly IToken _token;

        public BaseController(IMediator mediator, IToken token)
        {
            _mediator = mediator;
            _token = token;
        }

        [HttpGet("filterable")]
        public virtual IActionResult Get()
        {
            var properties = typeof(TEntity).GetProperties().Where(p => p.GetIndexParameters().Length == 0);
            var result = new List<object>();

            foreach (var property in properties)
            {
                if (Attribute.IsDefined(property, typeof(FilterableAttribute)))
                {
                    result.Add(new
                    {
                        Key = property.Name,
                        Text = ((FilterableAttribute)property.GetCustomAttribute(typeof(FilterableAttribute))).displayName,
                    });
                }
            }
            return Ok(new SimpleDataResult { Data = result });
        }
    }
}
