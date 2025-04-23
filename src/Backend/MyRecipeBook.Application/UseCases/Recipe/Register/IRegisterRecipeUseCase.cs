﻿using MyRecipeBook.Communication.Requests.Recipe;
using MyRecipeBook.Communication.Responses.Recipe;

namespace MyRecipeBook.Application.UseCases.Recipe.Register
{
    public interface IRegisterRecipeUseCase
    {
        public Task<ResponseRegisteredRecipeJson> Execute(RequestRecipeJson request);
    }
}
