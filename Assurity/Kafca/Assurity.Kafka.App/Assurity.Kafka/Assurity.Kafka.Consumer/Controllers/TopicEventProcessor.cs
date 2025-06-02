namespace Assurity.Kafka.Consumer.Controllers
{
    using System;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Managers.Interfaces;
    using Assurity.Kafka.Utilities.Config;
    using Assurity.Kafka.Utilities.Constants;
    using AutoMapper;
    using Avro.Generic;
    using Confluent.Kafka;
    using NewRelic.Api.Agent;

    public class TopicEventProcessor : ITopicEventProcessor
    {
        public TopicEventProcessor(
            IPPOLCEventManager pPOLCEventManager,
            IPPOLM_POLICY_BENEFIT_MISCEventManager pPOLM_POLICY_BENEFIT_MISCEventManager,
            IPNAMEEventManager pNAMEEventManager,
            IPADDREventManager pADDREventManager,
            IPPEND_NEW_BUSINESS_PENDINGEventManager pPEND_NEW_BUSINESS_PENDINGEventManager,
            IPPEND_NEW_BUS_PEND_UNDERWRITINGEventManager pPEND_NEW_BUS_PEND_UNDERWRITINGEventManager,
            IPRELA_RELATIONSHIP_MASTEREventManager pRELA_RELATIONSHIP_MASTEREventManager,
            IPMUIN_MULTIPLE_INSUREDSEventManager pMUIN_MULTIPLE_INSUREDSEventManager,
            IPNALKEventManager pNALKEventManager,
            IPCOMC_COMMISSION_CONTROLEventManager pCOMC_COMMISSION_CONTROLEventManager,
            IPCOMC_COMMISSION_CONTROL_TYPE_SEventManager pCOMC_COMMISSION_CONTROL_TYPE_SEventManager,
            IPHIER_AGENT_HIERARCHYEventManager pHIER_AGENT_HIERARCHYEventManager,
            IPPBEN_POLICY_BENEFITSEventManager pPBEN_POLICY_BENEFITSEventManager,
            IPPBEN_POLICY_BENEFITS_TYPES_BA_OREventManager pPBEN_POLICY_BENEFITS_TYPES_BA_OREventManager,
            IPPBEN_POLICY_BENEFITS_TYPES_BFEventManager pPBEN_POLICY_BENEFITS_TYPES_BFEventManager,
            IPPBEN_POLICY_BENEFITS_TYPES_SLEventManager pPBEN_POLICY_BENEFITS_TYPES_SLEventManager,
            IPPBEN_POLICY_BENEFITS_TYPES_SPEventManager pPBEN_POLICY_BENEFITS_TYPES_SPEventManager,
            IPPBEN_POLICY_BENEFITS_TYPES_SUEventManager pPBEN_POLICY_BENEFITS_TYPES_SUEventManager,
            IPMEDREventManager pMEDREventManager,
            IPRQRMEventManager pRQRMEventManager,
            IPRQRMTBLEventManager pRQRMTBLEventManager,
            IPGRUP_GROUP_MASTEREventManager pgrup_Group_MasterEventManager,
            ISysNBRequirementsEventManager sysNBRequirementsEventManager,
            ISysACAgentDataEventManager sysACAgentDataEventManager,
            ISysACAgentMarketCodesEventManager sysACAgentMarketCodesEventManager,
            ISysZ9ProcessEventManager sysZ9ProcessEventManager,
            IQUEUESEventManager queuesEventManager,
            IPACTGEventManager pactgEventManager,
            IPBDRVEventManager pbdrvEventManager,
            IPACON_ANNUITY_POLICYEventManager pACON_ANNUITY_POLICYEventManager,
            IMapper mapper,
            IConfigurationManager configuration)
        {
            PPOLCEventManager = pPOLCEventManager;
            PPOLM_POLICY_BENEFIT_MISCEventManager = pPOLM_POLICY_BENEFIT_MISCEventManager;
            PNAMEEventManager = pNAMEEventManager;
            PADDREventManager = pADDREventManager;
            PPEND_NEW_BUSINESS_PENDINGEventManager = pPEND_NEW_BUSINESS_PENDINGEventManager;
            PPEND_NEW_BUS_PEND_UNDERWRITINGEventManager = pPEND_NEW_BUS_PEND_UNDERWRITINGEventManager;
            PRELA_RELATIONSHIP_MASTEREventManager = pRELA_RELATIONSHIP_MASTEREventManager;
            PMUIN_MULTIPLE_INSUREDSEventManager = pMUIN_MULTIPLE_INSUREDSEventManager;
            PNALKEventManager = pNALKEventManager;
            PCOMC_COMMISSION_CONTROLEventManager = pCOMC_COMMISSION_CONTROLEventManager;
            PCOMC_COMMISSION_CONTROL_TYPE_SEventManager = pCOMC_COMMISSION_CONTROL_TYPE_SEventManager;
            PHIER_AGENT_HIERARCHYEventManager = pHIER_AGENT_HIERARCHYEventManager;
            PPBEN_POLICY_BENEFITSEventManager = pPBEN_POLICY_BENEFITSEventManager;
            PPBEN_POLICY_BENEFITS_TYPES_BA_OREventManager = pPBEN_POLICY_BENEFITS_TYPES_BA_OREventManager;
            PPBEN_POLICY_BENEFITS_TYPES_BFEventManager = pPBEN_POLICY_BENEFITS_TYPES_BFEventManager;
            PPBEN_POLICY_BENEFITS_TYPES_SLEventManager = pPBEN_POLICY_BENEFITS_TYPES_SLEventManager;
            PPBEN_POLICY_BENEFITS_TYPES_SPEventManager = pPBEN_POLICY_BENEFITS_TYPES_SPEventManager;
            PPBEN_POLICY_BENEFITS_TYPES_SUEventManager = pPBEN_POLICY_BENEFITS_TYPES_SUEventManager;
            PMEDREventManager = pMEDREventManager;
            PRQRMEventManager = pRQRMEventManager;
            PRQRMTBLEventManager = pRQRMTBLEventManager;
            PGRUP_Group_MasterEventManager = pgrup_Group_MasterEventManager;
            SysNBRequirementsEventManager = sysNBRequirementsEventManager;
            SysACAgentDataEventManager = sysACAgentDataEventManager;
            SysACAgentMarketCodesEventManager = sysACAgentMarketCodesEventManager;
            SysZ9ProcessEventManager = sysZ9ProcessEventManager;
            QUEUESEventManager = queuesEventManager;
            PACTGEventManager = pactgEventManager;
            PBDRVEventManager = pbdrvEventManager;
            PACON_ANNUITY_POLICYEventManager = pACON_ANNUITY_POLICYEventManager;
            Mapper = mapper;
            Config = configuration;
        }

        private IMapper Mapper { get; }

        private IConfigurationManager Config { get; set; }

        private IPPOLCEventManager PPOLCEventManager { get; }

        private IPPOLM_POLICY_BENEFIT_MISCEventManager PPOLM_POLICY_BENEFIT_MISCEventManager { get; }

        private IPNAMEEventManager PNAMEEventManager { get; }

        private IPADDREventManager PADDREventManager { get; }

        private IPPEND_NEW_BUSINESS_PENDINGEventManager PPEND_NEW_BUSINESS_PENDINGEventManager { get; }

        private IPPEND_NEW_BUS_PEND_UNDERWRITINGEventManager PPEND_NEW_BUS_PEND_UNDERWRITINGEventManager { get; }

        private IPRELA_RELATIONSHIP_MASTEREventManager PRELA_RELATIONSHIP_MASTEREventManager { get; }

        private IPCOMC_COMMISSION_CONTROL_TYPE_SEventManager PCOMC_COMMISSION_CONTROL_TYPE_SEventManager { get; }

        private IPCOMC_COMMISSION_CONTROLEventManager PCOMC_COMMISSION_CONTROLEventManager { get; }

        private IPHIER_AGENT_HIERARCHYEventManager PHIER_AGENT_HIERARCHYEventManager { get; }

        private IPMUIN_MULTIPLE_INSUREDSEventManager PMUIN_MULTIPLE_INSUREDSEventManager { get; }

        private IPNALKEventManager PNALKEventManager { get; }

        private IPPBEN_POLICY_BENEFITSEventManager PPBEN_POLICY_BENEFITSEventManager { get; }

        private IPPBEN_POLICY_BENEFITS_TYPES_BA_OREventManager PPBEN_POLICY_BENEFITS_TYPES_BA_OREventManager { get; }

        private IPPBEN_POLICY_BENEFITS_TYPES_BFEventManager PPBEN_POLICY_BENEFITS_TYPES_BFEventManager { get; }

        private IPPBEN_POLICY_BENEFITS_TYPES_SLEventManager PPBEN_POLICY_BENEFITS_TYPES_SLEventManager { get; }

        private IPPBEN_POLICY_BENEFITS_TYPES_SPEventManager PPBEN_POLICY_BENEFITS_TYPES_SPEventManager { get; }

        private IPPBEN_POLICY_BENEFITS_TYPES_SUEventManager PPBEN_POLICY_BENEFITS_TYPES_SUEventManager { get; }

        private IPMEDREventManager PMEDREventManager { get; }

        private IPRQRMEventManager PRQRMEventManager { get; }

        private IPRQRMTBLEventManager PRQRMTBLEventManager { get; }

        private IPGRUP_GROUP_MASTEREventManager PGRUP_Group_MasterEventManager { get; }

        private ISysNBRequirementsEventManager SysNBRequirementsEventManager { get; }

        private ISysACAgentDataEventManager SysACAgentDataEventManager { get; }

        private ISysACAgentMarketCodesEventManager SysACAgentMarketCodesEventManager { get; }

        private ISysZ9ProcessEventManager SysZ9ProcessEventManager { get; }

        private IQUEUESEventManager QUEUESEventManager { get; }

        private IPACTGEventManager PACTGEventManager { get; }

        private IPBDRVEventManager PBDRVEventManager { get; }

        private IPACON_ANNUITY_POLICYEventManager PACON_ANNUITY_POLICYEventManager { get; }

        [Trace]
        public async Task ProcessEvent(ConsumeResult<string, GenericRecord> record, bool slowConsumer = false)
        {
            string topicType = record.Topic;
            switch (topicType)
            {
                case var _ when topicType == Config.Topic_PPOLC:
                    await ProcessPPOLCEvent(record.Message.Value);
                    break;
                case var _ when topicType == Config.Topic_PPOLM_POLICY_BENEFIT_MISC:
                    await ProcessPPOLM_POLICY_BENEFIT_MISCEvent(record.Message.Value);
                    break;
                case var _ when topicType == Config.Topic_PNAME:
                    await ProcessPNAMEEvent(record.Message.Value, slowConsumer);
                    break;
                case var _ when topicType == Config.Topic_PADDR:
                    await ProcessPADDREvent(record.Message.Value, slowConsumer);
                    break;
                case var _ when topicType == Config.Topic_PPEND_NEW_BUSINESS_PENDING:
                    await ProcessPPEND_NEW_BUSINESS_PENDINGEvent(record.Message.Value);
                    break;
                case var _ when topicType == Config.Topic_PRELA_RELATIONSHIP_MASTER:
                    await ProcessPRELA_RELATIONSHIP_MASTEREvent(record.Message.Value);
                    break;
                case var _ when topicType == Config.Topic_PCOMC_COMMISSION_CONTROL_TYPE_S:
                    await ProcessPCOMC_COMMISSION_CONTROL_TYPE_SEvent(record.Message.Value);
                    break;
                case var _ when topicType == Config.Topic_PCOMC_COMMISSION_CONTROL:
                    await ProcessPCOMC_COMMISSION_CONTROLEvent(record.Message.Value);
                    break;
                case var _ when topicType == Config.Topic_PHIER_AGENT_HIERARCHY:
                    await ProcessPHIER_AGENT_HIERARCHYEvent(record.Message.Value, slowConsumer);
                    break;
                case var _ when topicType == Config.Topic_PMUIN_MULTIPLE_INSUREDS:
                    await ProcessPMUIN_MULTIPLE_INSUREDSEvent(record.Message.Value);
                    break;
                case var _ when topicType == Config.Topic_PNALK:
                    await ProcessPNALKEvent(record.Message.Value, slowConsumer);
                    break;
                case var _ when topicType == Config.Topic_PPBEN_POLICY_BENEFITS:
                    await ProcessPPBEN_POLICY_BENEFITSEvent(record.Message.Value);
                    break;
                case var _ when topicType == Config.Topic_PPBEN_POLICY_BENEFITS_TYPES_BA_OR:
                    await ProcessPPBEN_POLICY_BENEFITS_TYPES_BA_OREvent(record.Message.Value);
                    break;
                case var _ when topicType == Config.Topic_PPBEN_POLICY_BENEFITS_TYPES_BF:
                    await ProcessPPBEN_POLICY_BENEFITS_TYPES_BFEvent(record.Message.Value);
                    break;
                case var _ when topicType == Config.Topic_PPBEN_POLICY_BENEFITS_TYPES_SL:
                    await ProcessPPBEN_POLICY_BENEFITS_TYPES_SLEvent(record.Message.Value);
                    break;
                case var _ when topicType == Config.Topic_PPBEN_POLICY_BENEFITS_TYPES_SP:
                    await ProcessPPBEN_POLICY_BENEFITS_TYPES_SPEvent(record.Message.Value);
                    break;
                case var _ when topicType == Config.Topic_PPBEN_POLICY_BENEFITS_TYPES_SU:
                    await ProcessPPBEN_POLICY_BENEFITS_TYPES_SUEvent(record.Message.Value);
                    break;
                case var _ when topicType == Config.Topic_PMEDR:
                    await ProcessPMEDREvent(record.Message.Value);
                    break;
                case var _ when topicType == Config.Topic_PRQRM:
                    await ProcessPRQRMEvent(record.Message.Value);
                    break;
                case var _ when topicType == Config.Topic_PRQRMTBL:
                    await ProcessPRQRMTBLEvent(record.Message.Value);
                    break;
                case var _ when topicType == Config.Topic_PPEND_NEW_BUS_PEND_UNDERWRITING:
                    await ProcessPPEND_NEW_BUS_PEND_UNDERWRITINGEvent(record.Message.Value);
                    break;
                case var _ when topicType == Config.Topic_PGRUP_GROUP_MASTER:
                    await ProcessPGRUP_GROUP_MASTEREvent(record.Message.Value);
                    break;
                case var _ when topicType == Config.Topic_SysNBRequirements:
                    await ProcessSysNBRequirementsEvent(record.Message.Value);
                    break;
                case var _ when topicType == Config.Topic_SysACAgentData:
                    await ProcessSysACAgentDataEvent(record.Message.Value);
                    break;
                case var _ when topicType == Config.Topic_SysACAgentMarketCodes:
                    await ProcessSysACAgentMarketCodesEvent(record.Message.Value);
                    break;
                case var _ when topicType == Config.Topic_SysZ9Process:
                    await ProcessSysZ9ProcessEvent(record.Message.Value);
                    break;
                case var _ when topicType == Config.Topic_QUEUES:
                    await ProcessQUEUESEvent(record.Message.Value);
                    break;
                case var _ when topicType == Config.Topic_PACTG:
                    await ProcessPACTGEvent(record.Message.Value);
                    break;
                case var _ when topicType == Config.Topic_PBDRV:
                    await ProcessPBDRVEvent(record.Message.Value);
                    break;
                case var _ when topicType == Config.Topic_PACON_ANNUITY_POLICY:
                    await ProcessPACON_ANNUITY_POLICYEvent(record.Message.Value);
                    break;
                default:
                    throw new InvalidOperationException($"Invalid Topic: {topicType}");
            }
        }

        [Trace]
        private async Task ProcessPPOLCEvent(GenericRecord record)
        {
            var eventData = Mapper.Map<PPOLC>(record);
            await PPOLCEventManager.ProcessEvent(eventData);
        }

        [Trace]
        private async Task ProcessPPOLM_POLICY_BENEFIT_MISCEvent(GenericRecord record)
        {
            var eventData = Mapper.Map<PPOLM_POLICY_BENEFIT_MISC>(record);
            await PPOLM_POLICY_BENEFIT_MISCEventManager.ProcessEvent(eventData);
        }

        [Trace]
        private async Task ProcessPNAMEEvent(GenericRecord record, bool slowConsumer = false)
        {
            var eventData = Mapper.Map<PNAME>(record);
            await PNAMEEventManager.ProcessEvent(eventData, slowConsumer);
        }

        [Trace]
        private async Task ProcessPADDREvent(GenericRecord record, bool slowConsumer = false)
        {
            var eventData = Mapper.Map<PADDR>(record);
            await PADDREventManager.ProcessEvent(eventData, slowConsumer);
        }

        [Trace]
        private async Task ProcessPPEND_NEW_BUSINESS_PENDINGEvent(GenericRecord record)
        {
            var eventData = Mapper.Map<PPEND_NEW_BUSINESS_PENDING>(record);
            await PPEND_NEW_BUSINESS_PENDINGEventManager.ProcessEvent(eventData);
        }

        [Trace]
        private async Task ProcessPRELA_RELATIONSHIP_MASTEREvent(GenericRecord record)
        {
            var eventData = Mapper.Map<PRELA_RELATIONSHIP_MASTER>(record);
            await PRELA_RELATIONSHIP_MASTEREventManager.ProcessEvent(eventData);
        }

        [Trace]
        private async Task ProcessPCOMC_COMMISSION_CONTROL_TYPE_SEvent(GenericRecord record)
        {
            var eventData = Mapper.Map<PCOMC_COMMISSION_CONTROL_TYPE_S>(record);
            await PCOMC_COMMISSION_CONTROL_TYPE_SEventManager.ProcessEvent(eventData);
        }

        [Trace]
        private async Task ProcessPCOMC_COMMISSION_CONTROLEvent(GenericRecord record)
        {
            var eventData = Mapper.Map<PCOMC_COMMISSION_CONTROL>(record);
            await PCOMC_COMMISSION_CONTROLEventManager.ProcessEvent(eventData);
        }

        [Trace]
        private async Task ProcessPHIER_AGENT_HIERARCHYEvent(GenericRecord record, bool slowConsumer = false)
        {
            var eventData = Mapper.Map<PHIER_AGENT_HIERARCHY>(record);
            record.TryGetValue(TopicFields.Operation, out object? changeType);
            record.TryGetValue(TopicFields.BeforeAgentNumber, out object? beforeAgentNum);
            await PHIER_AGENT_HIERARCHYEventManager.ProcessEvent(eventData, changeType?.ToString(), beforeAgentNum?.ToString(), slowConsumer);
        }

        private async Task ProcessPMUIN_MULTIPLE_INSUREDSEvent(GenericRecord record)
        {
            var eventData = Mapper.Map<PMUIN_MULTIPLE_INSUREDS>(record);
            record.TryGetValue(TopicFields.Operation, out object? changeType);
            await PMUIN_MULTIPLE_INSUREDSEventManager.ProcessEvent(eventData, changeType.ToString());
        }

        [Trace]
        private async Task ProcessPNALKEvent(GenericRecord record, bool slowConsumer = false)
        {
            var eventData = Mapper.Map<PNALK>(record);
            await PNALKEventManager.ProcessEvent(eventData, slowConsumer);
        }

        [Trace]
        private async Task ProcessPPBEN_POLICY_BENEFITSEvent(GenericRecord record)
        {
            var eventData = Mapper.Map<PPBEN_POLICY_BENEFITS>(record);
            record.TryGetValue(TopicFields.Operation, out object? changeType);
            await PPBEN_POLICY_BENEFITSEventManager.ProcessEvent(eventData, changeType.ToString());
        }

        private async Task ProcessPPBEN_POLICY_BENEFITS_TYPES_BA_OREvent(GenericRecord record)
        {
            var eventData = Mapper.Map<PPBEN_POLICY_BENEFITS_TYPES_BA_OR>(record);
            await PPBEN_POLICY_BENEFITS_TYPES_BA_OREventManager.ProcessEvent(eventData);
        }

        private async Task ProcessPPBEN_POLICY_BENEFITS_TYPES_BFEvent(GenericRecord record)
        {
            var eventData = Mapper.Map<PPBEN_POLICY_BENEFITS_TYPES_BF>(record);
            await PPBEN_POLICY_BENEFITS_TYPES_BFEventManager.ProcessEvent(eventData);
        }

        private async Task ProcessPPBEN_POLICY_BENEFITS_TYPES_SLEvent(GenericRecord record)
        {
            var eventData = Mapper.Map<PPBEN_POLICY_BENEFITS_TYPES_SL>(record);
            await PPBEN_POLICY_BENEFITS_TYPES_SLEventManager.ProcessEvent(eventData);
        }

        private async Task ProcessPPBEN_POLICY_BENEFITS_TYPES_SPEvent(GenericRecord record)
        {
            var eventData = Mapper.Map<PPBEN_POLICY_BENEFITS_TYPES_SP>(record);
            await PPBEN_POLICY_BENEFITS_TYPES_SPEventManager.ProcessEvent(eventData);
        }

        private async Task ProcessPPBEN_POLICY_BENEFITS_TYPES_SUEvent(GenericRecord record)
        {
            var eventData = Mapper.Map<PPBEN_POLICY_BENEFITS_TYPES_SU>(record);
            await PPBEN_POLICY_BENEFITS_TYPES_SUEventManager.ProcessEvent(eventData);
        }

        private async Task ProcessPMEDREvent(GenericRecord record)
        {
            var eventData = Mapper.Map<PMEDR>(record);
            await PMEDREventManager.ProcessEvent(eventData);
        }

        private async Task ProcessPRQRMEvent(GenericRecord record)
        {
            var eventData = Mapper.Map<PRQRM>(record);
            await PRQRMEventManager.ProcessEvent(eventData);
        }

        private async Task ProcessPRQRMTBLEvent(GenericRecord record)
        {
            var eventData = Mapper.Map<PRQRMTBL>(record);
            await PRQRMTBLEventManager.ProcessEvent(eventData);
        }

        private async Task ProcessPPEND_NEW_BUS_PEND_UNDERWRITINGEvent(GenericRecord record)
        {
            var eventData = Mapper.Map<PPEND_NEW_BUS_PEND_UNDERWRITING>(record);
            await PPEND_NEW_BUS_PEND_UNDERWRITINGEventManager.ProcessEvent(eventData);
        }

        private async Task ProcessPGRUP_GROUP_MASTEREvent(GenericRecord record)
        {
            var eventData = Mapper.Map<PGRUP_GROUP_MASTER>(record);
            await PGRUP_Group_MasterEventManager.ProcessEvent(eventData);
        }

        private async Task ProcessSysACAgentDataEvent(GenericRecord record)
        {
            var eventData = Mapper.Map<SysACAgentData>(record);
            await SysACAgentDataEventManager.ProcessEvent(eventData);
        }

        private async Task ProcessSysACAgentMarketCodesEvent(GenericRecord record)
        {
            var eventData = Mapper.Map<SysACAgentMarketCodes>(record);
            await SysACAgentMarketCodesEventManager.ProcessEvent(eventData);
        }

        private async Task ProcessSysZ9ProcessEvent(GenericRecord record)
        {
            var eventData = Mapper.Map<SysZ9Process>(record);
            await SysZ9ProcessEventManager.ProcessEvent(eventData);
        }

        private async Task ProcessSysNBRequirementsEvent(GenericRecord record)
        {
            var eventData = Mapper.Map<SysNBRequirements>(record);
            await SysNBRequirementsEventManager.ProcessEvent(eventData);
        }

        private async Task ProcessQUEUESEvent(GenericRecord record)
        {
            var eventData = Mapper.Map<QUEUES>(record);
            record.TryGetValue(TopicFields.BeforeGlobalQueue, out object? beforeQueue);
            await QUEUESEventManager.ProcessEvent(eventData, beforeQueue?.ToString());
        }

        private async Task ProcessPACTGEvent(GenericRecord record)
        {
            var eventData = Mapper.Map<PACTG>(record);
            await PACTGEventManager.ProcessEvent(eventData);
        }

        private async Task ProcessPBDRVEvent(GenericRecord record)
        {
            var eventData = Mapper.Map<PBDRV>(record);
            await PBDRVEventManager.ProcessEvent(eventData);
        }

        private async Task ProcessPACON_ANNUITY_POLICYEvent(GenericRecord record)
        {
            var eventData = Mapper.Map<PACON_ANNUITY_POLICY>(record);
            await PACON_ANNUITY_POLICYEventManager.ProcessEvent(eventData);
        }
    }
}