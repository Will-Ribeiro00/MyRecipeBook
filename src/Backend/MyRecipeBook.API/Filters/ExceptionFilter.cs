using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MyRecipeBook.Communication.Responses.Exception;
using MyRecipeBook.Exception;
using MyRecipeBook.Exception.ExceptionsBase;
using System.Net;

namespace MyRecipeBook.API.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is MyRecipeBookException)
                HandleProjectException(context);
            else
                ThrowUnknowException(context);
        }

        private static void HandleProjectException(ExceptionContext context)
        {
            if(context.Exception is ErrorOnValidationException errorOnValidationException)
            {
                var errorList = errorOnValidationException!.ErrorMessages;

                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Result = new BadRequestObjectResult(new ResponseErrorJson(errorList));
            }
            else if (context.Exception is InvalidLoginException invalidLoginException)
            {
                var errorList = invalidLoginException!.Message;

                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Result = new UnauthorizedObjectResult(new ResponseErrorJson(errorList));
            }
            else if (context.Exception is NotFoundException notFoundException)
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                context.Result = new NotFoundObjectResult(new ResponseErrorJson(notFoundException.Message));
            }
        }
        private static void ThrowUnknowException(ExceptionContext context)
        {
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Result = new ObjectResult(new ResponseErrorJson(ResourceMessagesExceptions.UNKNOWN_ERROR));
        }
    }
}
