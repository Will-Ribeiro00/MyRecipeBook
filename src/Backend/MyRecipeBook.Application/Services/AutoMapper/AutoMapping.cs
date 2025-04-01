using AutoMapper;
using MyRecipeBook.Communication.Requests.User;
using MyRecipeBook.Domain.Entities;

namespace MyRecipeBook.Application.Services.AutoMapper
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            RequestToDomain();
        }

        private void RequestToDomain()
        {
            CreateMap<RequestRegisterUserJson, User>()
                .ForMember(dest => dest.Password, option => option.Ignore());
        }
        private void DomainToResponse()
        {

        }
    }
}
