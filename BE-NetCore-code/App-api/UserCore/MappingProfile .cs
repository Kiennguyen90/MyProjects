using AutoMapper;
using CryptoInfrastructure.Model;
using UserCore.ViewModels.Respones;

namespace UserCore
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<ApplicationUser, UserComonInfoRespone>();
            CreateMap<UserComonInfoRespone, ApplicationUser>();
            //CreateMap<IdentityResult, IdentityResult>();
            //CreateMap<IdentityError, IdentityError>();
            //CreateMap<RegenerateAccessTokenRequest, RegenerateAccessTokenRequest>();
        }
    }
}
