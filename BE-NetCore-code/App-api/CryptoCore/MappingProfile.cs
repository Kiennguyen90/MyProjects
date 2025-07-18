using AutoMapper;
using CryptoCore.ViewModels.Respones;
using CryptoInfrastructure.Models;

namespace CryptoCore
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserInformationRespone>();
        }
    }
}
