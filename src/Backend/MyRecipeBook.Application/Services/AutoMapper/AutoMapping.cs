using AutoMapper;
using MyRecipeBook.Communication.Enums;
using MyRecipeBook.Communication.Requests.Recipe;
using MyRecipeBook.Communication.Requests.User;
using MyRecipeBook.Communication.Responses.Recipe;
using MyRecipeBook.Communication.Responses.User;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Enums;
using Sqids;

namespace MyRecipeBook.Application.Services.AutoMapper
{
    public class AutoMapping : Profile
    {
        private readonly SqidsEncoder<long> _idEncoder;
        public AutoMapping(SqidsEncoder<long> idEncoder)
        {
            _idEncoder = idEncoder;

            RequestToDomain();
            DomainToResponse();
        }

        private void RequestToDomain()
        {
            CreateMap<RequestRegisterUserJson, User>()
                .ForMember(dest => dest.Password, option => option.Ignore());

            CreateMap<RequestRecipeJson, Recipe>()
                .ForMember(dest => dest.Instructions, option => option.Ignore())
                .ForMember(dest => dest.Ingredients, option => option.MapFrom(source => source.Ingredients.Distinct()))
                .ForMember(dest => dest.DishTypes, option => option.MapFrom(source => source.DishTypes.Distinct()));

            CreateMap<string, Ingredient>()
                .ForMember(dest => dest.Item, option => option.MapFrom(source => source));

            CreateMap<Communication.Enums.DishType, Domain.Entities.DishType>()
                .ForMember(dest => dest.Type, option => option.MapFrom(source => source));

            CreateMap<RequestIntructionJson, Instruction>();
        }

        private void DomainToResponse()
        {
            CreateMap<User, ResponseUserProfileJson>();

            CreateMap<Recipe, ResponseRegisteredRecipeJson>()
                .ForMember(dest => dest.Id, config => config.MapFrom(source => _idEncoder.Encode(source.Id)));
        }
    }
}
