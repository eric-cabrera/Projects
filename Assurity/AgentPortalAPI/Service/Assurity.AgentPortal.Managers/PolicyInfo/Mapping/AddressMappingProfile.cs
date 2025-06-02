namespace Assurity.AgentPortal.Managers.PolicyInfo.Mapping
{
    using Assurity.PolicyInfo.Contracts.V1;
    using AutoMapper;

    public class AddressMappingProfile : Profile
    {
        public AddressMappingProfile()
        {
            CreateMap<Address, Address>()
                .ForMember(
                    address => address.ZipExtension,
                    opt => opt.MapFrom(address => GetZipExtension(address)));
        }

        private static string GetZipExtension(Address address)
        {
            var zipExtension = address?.ZipExtension?.Trim() ?? string.Empty;
            var boxNumber = address?.BoxNumber?.Trim() ?? string.Empty;

            if (zipExtension.Length == 4)
            {
                return zipExtension;
            }

            if (boxNumber.Length == 4)
            {
                return boxNumber;
            }

            return null;
        }
    }
}