namespace Assurity.Kafka.Consumer.Mappers
{
    using Assurity.Kafka.Accessors.Entities;
    using AutoMapper;
    using Avro.Generic;

    public class PGRUP_GroupMasterMappingProfile : Profile
    {
        public PGRUP_GroupMasterMappingProfile()
        {
            CreateMap<GenericRecord, PGRUP_GROUP_MASTER>()
                .ForMember(
                    destination => destination.COMPANY_CODE,
                    memberOption => memberOption.MapFrom(source => source[nameof(PGRUP_GROUP_MASTER.COMPANY_CODE)].ToString().Trim()))
                .ForMember(
                    destination => destination.GROUP_NUMBER,
                    memberOption => memberOption.MapFrom(source => source[nameof(PGRUP_GROUP_MASTER.GROUP_NUMBER)].ToString().Trim()))
                .ForMember(
                    destination => destination.NAME_ID,
                    memberOption => memberOption.MapFrom(source => source[nameof(PGRUP_GROUP_MASTER.NAME_ID)]))
                .ForMember(
                    destination => destination.STATUS_CODE,
                    memberOption => memberOption.MapFrom(source => source[nameof(PGRUP_GROUP_MASTER.STATUS_CODE)]))
                .ForMember(
                    destination => destination.PNAME,
                    memberOption => memberOption.Ignore());
        }
    }
}
