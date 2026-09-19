using SJInovacao.Acesso.WebAPI.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SJInovacao.Acesso.WebAPI.Common.Filters
{
    public class ApiResponseWrapperAttribute : ResultFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is ObjectResult objectResult)
            {
                if (objectResult.Value is ApiResponse)
                    return;

                context.Result = new ObjectResult(new ApiResponseWithData<object>
                {
                    Success = true,
                    Data = objectResult.Value
                });
            }
        }
    }
}
