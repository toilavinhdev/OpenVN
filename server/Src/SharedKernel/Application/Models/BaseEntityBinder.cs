using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SharedKernel.Application
{
    public class BaseEntityBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            return Task.CompletedTask;
        }
    }
}
