﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MyRecipeBook.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MyRecipeBookBaseController : ControllerBase
    {
        protected static bool IsNotAuthenticated(AuthenticateResult authenticate)
        {
            return authenticate.Succeeded == false
                || authenticate.Principal is null
                || authenticate.Principal.Identities.Any(id => id.IsAuthenticated == false);
        }
    }
}
