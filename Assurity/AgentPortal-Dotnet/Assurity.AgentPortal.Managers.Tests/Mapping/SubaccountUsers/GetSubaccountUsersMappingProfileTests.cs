namespace Assurity.AgentPortal.Managers.Tests.Mapping.SubaccountUsers;

using Assurity.AgentPortal.Managers.Mapping.SubaccountUsers;
using AutoMapper;

public class GetSubaccountUsersMappingProfileTests
{
    public IMapper Mapper
    {
        get
        {
            var mapperConfiguration = new MapperConfiguration(
                configuration =>
                {
                    configuration.AddProfile(typeof(GetSubaccountUsersMappingProfile));
                });

            return mapperConfiguration.CreateMapper();
        }
    }

    [Fact]
    public void GetSubaccountUsersMappingProfile_AssertConfigurationIsValid()
    {
        // Assert
        Mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }
}
