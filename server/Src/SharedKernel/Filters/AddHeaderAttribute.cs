using Microsoft.AspNetCore.Mvc.Filters;

namespace SharedKernel.Filters
{
    /// <summary>
    /// Filter attribute to add information to header
    /// </summary>
    public class AddHeaderAttribute : ResultFilterAttribute
    {
        private readonly string _name;
        private readonly string _value;

        public AddHeaderAttribute(string name, string value) => (_name, _value) = (name, value);

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            context.HttpContext.Response.Headers.Add(_name, _value);
            base.OnResultExecuting(context);
        }
    }
}
