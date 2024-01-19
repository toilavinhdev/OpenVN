using Microsoft.AspNetCore.Authorization;

namespace SharedKernel.Filters
{
    public class OpenAuthorizeAttribute : AuthorizeAttribute
    {
        public override bool Match(object obj)
        {
            return base.Match(obj);
        }
    }
}
