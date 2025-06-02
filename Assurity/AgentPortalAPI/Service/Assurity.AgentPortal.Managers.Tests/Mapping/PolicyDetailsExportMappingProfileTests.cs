namespace Assurity.AgentPortal.Managers.Tests.Mapping;

using System.Text;
using Assurity.AgentPortal.Contracts.PolicyInfo.FileExport;
using Assurity.AgentPortal.Managers.MappingExtensions;
using Assurity.AgentPortal.Managers.PolicyInfo.Mapping;
using Assurity.PolicyInfo.Contracts.V1.Enums;
using AutoBogus;
using AutoMapper;
using Xunit;
using PolicyInfoAPI = Assurity.PolicyInformation.Contracts.V1;

public class PolicyDetailsExportMappingProfileTests
{
    public PolicyDetailsExportMappingProfileTests()
    {
        Mapper = new MapperConfiguration(config =>
        {
            config.AddProfile<AddressMappingProfile>();
            config.AddProfile<BusinessMappingProfile>();
            config.AddProfile<NameMappingProfile>();
            config.AddProfile<ParticipantMappingProfile>();
            config.AddProfile<PersonMappingProfile>();
            config.AddProfile<SummaryMappingProfile>();
            config.AddProfile<PolicyDetailsExportMappingProfile>();
        }).CreateMapper();
    }

    private IMapper Mapper { get; }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void RequirementSummary_to_PendingPolicySummary_ShouldMap(bool isBusiness)
    {
        // Arrangement
        var policySummaryFaker = new AutoFaker<PolicyInfoAPI.RequirementSummary>();
        var policySummary = policySummaryFaker.Generate();
        policySummary.AssignedAgents[0].Participant.IsBusiness = isBusiness;
        policySummary.PrimaryInsured.Participant.IsBusiness = false;
        policySummary.PrimaryOwner.Participant.IsBusiness = false;
        policySummary.Requirement.AppliesTo.IsBusiness = false;
        foreach (var agent in policySummary.AssignedAgents)
        {
            agent.IsServicingAgent = false;
        }

        policySummary.AssignedAgents[0].IsServicingAgent = true;

        // Act
        var mappedResponse = Mapper.Map<PendingPolicyExport>(policySummary);

        // Assert
        var requirementMet = MappingExtensions.GetEnumDisplayName(policySummary.Requirement.Status);
        var requirementAppliesTo = PolicyDetailsExportMappingProfile.GetParticipantName(policySummary.Requirement.AppliesTo);
        var requirementComment = PolicyDetailsExportMappingProfile.GetRequirementComment(policySummary.Requirement);
        var actionNeeded = PolicyDetailsExportMappingProfile.GetActionNeeded(policySummary.Requirement);
        var agentLastName = isBusiness
            ? policySummary.AssignedAgents[0].Participant.Business.Name.BusinessName
            : policySummary.AssignedAgents[0].Participant.Person.Name.IndividualLast;

        Assert.Equal(policySummary.Requirement.AddedDate, mappedResponse.EntryDate);
        Assert.Equal(agentLastName, mappedResponse.AgentLastName);
        Assert.Equal(policySummary.AssignedAgents[0].Participant.Person.Name.IndividualFirst, mappedResponse.AgentFirstName);
        Assert.Equal(policySummary.AssignedAgents[0].AgentId, mappedResponse.AgentId);
        Assert.Equal(policySummary.PolicyNumber, mappedResponse.PolicyNumber);
        Assert.Equal(policySummary.ProductCategory, mappedResponse.ProductCategory);
        Assert.Equal(policySummary.PrimaryInsured.Participant.Person.Name.IndividualLast, mappedResponse.PrimaryInsuredLastName);
        Assert.Equal(policySummary.PrimaryInsured.Participant.Person.Name.IndividualFirst, mappedResponse.PrimaryInsuredFirstName);
        Assert.Equal(policySummary.Requirement.Name, mappedResponse.Requirement);

        // RequirementUpdated not implemented yet
        Assert.Equal(requirementMet, mappedResponse.Met);
        Assert.Equal(requirementAppliesTo, mappedResponse.RequirementAppliesTo);
        Assert.Equal(requirementComment, mappedResponse.RequirementComment);
        Assert.Equal(actionNeeded, mappedResponse.ActionNeeded);
        Assert.Equal(policySummary.ApplicationSignedDate, mappedResponse.ApplicationSignedDate);
        Assert.Equal(policySummary.PrimaryOwner.Participant.Person.Name.IndividualLast, mappedResponse.PrimaryOwnerLastName);
        Assert.Equal(policySummary.PrimaryOwner.Participant.Person.Name.IndividualFirst, mappedResponse.PrimaryOwnerFirstName);
        Assert.Equal(policySummary.PrimaryOwner.Participant.PhoneNumber, mappedResponse.PrimaryOwnerPhoneNumber);
        Assert.Equal(policySummary.PrimaryOwner.Participant.Person.EmailAddress, mappedResponse.PrimaryOwnerEmail);
        Assert.Equal(policySummary.EmployerName, mappedResponse.EmployerName);
        Assert.Equal(policySummary.EmployerNumber, mappedResponse.EmployerNumber);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void RequirementSummary_With_JustInTimeAgent_to_PendingPolicySummary_ShouldMap(bool isBusiness)
    {
        // Arrangement
        var policySummaryFaker = new AutoFaker<PolicyInfoAPI.RequirementSummary>();
        var policySummary = policySummaryFaker.Generate();
        policySummary.AssignedAgents[0].Participant.IsBusiness = isBusiness;
        policySummary.PrimaryInsured.Participant.IsBusiness = false;
        policySummary.PrimaryOwner.Participant.IsBusiness = false;
        policySummary.Requirement.AppliesTo.IsBusiness = false;
        foreach (var agent in policySummary.AssignedAgents)
        {
            agent.IsJustInTimeAgent = false;
        }

        policySummary.AssignedAgents[0].IsJustInTimeAgent = true;

        // Act
        var mappedResponse = Mapper.Map<PendingPolicyExport>(policySummary);

        // Assert
        var requirementMet = MappingExtensions.GetEnumDisplayName(policySummary.Requirement.Status);
        var requirementAppliesTo = PolicyDetailsExportMappingProfile.GetParticipantName(policySummary.Requirement.AppliesTo);
        var requirementComment = PolicyDetailsExportMappingProfile.GetRequirementComment(policySummary.Requirement);
        var actionNeeded = PolicyDetailsExportMappingProfile.GetActionNeeded(policySummary.Requirement);
        var agentLastName = isBusiness
            ? policySummary.AssignedAgents[0].Participant.Business.Name.BusinessName
            : policySummary.AssignedAgents[0].Participant.Person.Name.IndividualLast;

        Assert.Equal(policySummary.Requirement.AddedDate, mappedResponse.EntryDate);
        Assert.Equal(agentLastName, mappedResponse.AgentLastName);
        Assert.Equal(policySummary.AssignedAgents[0].Participant.Person.Name.IndividualFirst, mappedResponse.AgentFirstName);
        Assert.Equal(policySummary.AssignedAgents[0].AgentId, mappedResponse.AgentId);
        Assert.Equal(policySummary.PolicyNumber, mappedResponse.PolicyNumber);
        Assert.Equal(policySummary.ProductCategory, mappedResponse.ProductCategory);
        Assert.Equal(policySummary.PrimaryInsured.Participant.Person.Name.IndividualLast, mappedResponse.PrimaryInsuredLastName);
        Assert.Equal(policySummary.PrimaryInsured.Participant.Person.Name.IndividualFirst, mappedResponse.PrimaryInsuredFirstName);
        Assert.Equal(policySummary.Requirement.Name, mappedResponse.Requirement);

        // RequirementUpdated not implemented yet
        Assert.Equal(requirementMet, mappedResponse.Met);
        Assert.Equal(requirementAppliesTo, mappedResponse.RequirementAppliesTo);
        Assert.Equal(requirementComment, mappedResponse.RequirementComment);
        Assert.Equal(actionNeeded, mappedResponse.ActionNeeded);
        Assert.Equal(policySummary.ApplicationSignedDate, mappedResponse.ApplicationSignedDate);
        Assert.Equal(policySummary.PrimaryOwner.Participant.Person.Name.IndividualLast, mappedResponse.PrimaryOwnerLastName);
        Assert.Equal(policySummary.PrimaryOwner.Participant.Person.Name.IndividualFirst, mappedResponse.PrimaryOwnerFirstName);
        Assert.Equal(policySummary.PrimaryOwner.Participant.PhoneNumber, mappedResponse.PrimaryOwnerPhoneNumber);
        Assert.Equal(policySummary.PrimaryOwner.Participant.Person.EmailAddress, mappedResponse.PrimaryOwnerEmail);
        Assert.Equal(policySummary.EmployerName, mappedResponse.EmployerName);
        Assert.Equal(policySummary.EmployerNumber, mappedResponse.EmployerNumber);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void PolicySummary_to_ActivePolicySummary_ShouldMap(bool isBusiness)
    {
        // Arrangement
        var policySummaryFaker = new AutoFaker<PolicyInfoAPI.PolicySummary>();
        var policySummary = policySummaryFaker.Generate();
        policySummary.AssignedAgents[0].Participant.IsBusiness = isBusiness;
        policySummary.PrimaryInsured.Participant.IsBusiness = false;
        policySummary.PrimaryOwner.Participant.IsBusiness = false;
        foreach (var agent in policySummary.AssignedAgents)
        {
            agent.IsServicingAgent = false;
        }

        policySummary.AssignedAgents[0].IsServicingAgent = true;

        // Act
        var mappedResponse = Mapper.Map<ActivePolicyExport>(policySummary);

        // Assert
        var status = MappingExtensions.GetEnumDisplayName(policySummary.PolicyStatus);
        var pastDue = policySummary.PastDue ? "Past Due" : string.Empty;
        var billingMode = MappingExtensions.GetEnumDisplayName(policySummary.BillingMode);
        var issueState = MappingExtensions.GetEnumDisplayName(policySummary.IssueState);
        var primaryOwnerState = PolicyDetailsExportMappingProfile.GetParticipantState(policySummary.PrimaryOwner.Participant);

        var servicingAgent = policySummary.AssignedAgents.Where(agent => agent.IsServicingAgent == true).FirstOrDefault();
        var agentLastName = isBusiness
            ? servicingAgent.Participant.Business.Name.BusinessName
            : servicingAgent.Participant.Person.Name.IndividualLast;

        Assert.Equal(agentLastName, mappedResponse.AgentLastName);
        Assert.Equal(policySummary.AssignedAgents[0].Participant.Person.Name.IndividualFirst, mappedResponse.AgentFirstName);
        Assert.Equal(policySummary.AssignedAgents[0].AgentId, mappedResponse.AgentId);
        Assert.Equal(policySummary.PolicyNumber, mappedResponse.PolicyNumber);
        Assert.Equal(policySummary.PrimaryInsured.Participant.Person.Name.IndividualLast, mappedResponse.PrimaryInsuredLastName);
        Assert.Equal(policySummary.PrimaryInsured.Participant.Person.Name.IndividualFirst, mappedResponse.PrimaryInsuredFirstName);
        Assert.Equal(policySummary.ProductCategory, mappedResponse.ProductCategory);
        Assert.Equal(policySummary.ProductDescription, mappedResponse.ProductDescription);
        Assert.Equal(status, mappedResponse.Status);
        Assert.Equal(pastDue, mappedResponse.PastDue);
        Assert.Equal(policySummary.PaidToDate, mappedResponse.PaidToDate);
        Assert.Equal(billingMode, mappedResponse.BillingMode);
        Assert.Equal(policySummary.ModePremium, mappedResponse.ModePremium);
        Assert.Equal(policySummary.AnnualPremium, mappedResponse.AnnualPremium);
        Assert.Equal(policySummary.FaceAmount, mappedResponse.FaceAmount);
        Assert.Equal(issueState, mappedResponse.IssueState);
        Assert.Equal(policySummary.PrimaryOwner.Participant.Person.Name.IndividualLast, mappedResponse.PrimaryOwnerLastName);
        Assert.Equal(policySummary.PrimaryOwner.Participant.Person.Name.IndividualFirst, mappedResponse.PrimaryOwnerFirstName);
        Assert.Equal(policySummary.PrimaryOwner.Participant.PhoneNumber, mappedResponse.PrimaryOwnerPhoneNumber);
        Assert.Equal(policySummary.PrimaryOwner.Participant.Person.EmailAddress, mappedResponse.PrimaryOwnerEmail);
        Assert.Equal(policySummary.PrimaryOwner.Participant.Address.Line1, mappedResponse.PrimaryOwnerAddressLine1);
        Assert.Equal(policySummary.PrimaryOwner.Participant.Address.Line2, mappedResponse.PrimaryOwnerAddressLine2);
        Assert.Equal(policySummary.PrimaryOwner.Participant.Address.City, mappedResponse.PrimaryOwnerCity);
        Assert.Equal(primaryOwnerState, mappedResponse.PrimaryOwnerState);

        var fullZipCode = PolicyDetailsExportMappingProfile.GetParticipantZip(policySummary.PrimaryOwner.Participant);
        Assert.Equal(fullZipCode, mappedResponse.PrimaryOwnerZip);
        Assert.Equal(policySummary.EmployerName, mappedResponse.EmployerName);
        Assert.Equal(policySummary.EmployerNumber, mappedResponse.EmployerNumber);
        Assert.Equal(policySummary.ApplicationSignedDate, mappedResponse.ApplicationSignedDate);
        Assert.Equal(policySummary.IssueDate, mappedResponse.IssueDate);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void PolicySummary_to_TerminatedPolicySummary_ShouldMap(bool isBusiness)
    {
        // Arrangement
        var policySummaryFaker = new AutoFaker<PolicyInfoAPI.PolicySummary>();
        var policySummary = policySummaryFaker.Generate();
        policySummary.AssignedAgents[0].Participant.IsBusiness = isBusiness;
        policySummary.PrimaryInsured.Participant.IsBusiness = false;
        policySummary.PrimaryOwner.Participant.IsBusiness = false;
        policySummary.ReturnPaymentType = ReturnPaymentType.InitialPaymentCardDeclined;
        foreach (var agent in policySummary.AssignedAgents)
        {
            agent.IsServicingAgent = false;
        }

        policySummary.AssignedAgents[0].IsServicingAgent = true;

        // Act
        var mappedResponse = Mapper.Map<TerminatedPolicyExport>(policySummary);

        // Assert
        var status = MappingExtensions.GetEnumDisplayName(policySummary.PolicyStatus);
        var terminationReason = MappingExtensions.GetEnumDisplayName(policySummary.PolicyStatusReason);
        var terminationDetail = MappingExtensions.GetEnumDisplayName(policySummary.PolicyStatusDetail);
        var billingMode = MappingExtensions.GetEnumDisplayName(policySummary.BillingMode);
        var issueState = MappingExtensions.GetEnumDisplayName(policySummary.IssueState);
        var primaryOwnerState = PolicyDetailsExportMappingProfile.GetParticipantState(policySummary.PrimaryOwner.Participant);
        var assignedAgentLastName = isBusiness
            ? policySummary.AssignedAgents[0].Participant.Business.Name.BusinessName
            : policySummary.AssignedAgents[0].Participant.Person.Name.IndividualLast;

        Assert.Equal(assignedAgentLastName, mappedResponse.AgentLastName);
        Assert.Equal(policySummary.AssignedAgents[0].Participant.Person.Name.IndividualFirst, mappedResponse.AgentFirstName);
        Assert.Equal(policySummary.AssignedAgents[0].AgentId, mappedResponse.AgentId);
        Assert.Equal(policySummary.PolicyNumber, mappedResponse.PolicyNumber);
        Assert.Equal(policySummary.PrimaryInsured.Participant.Person.Name.IndividualLast, mappedResponse.PrimaryInsuredLastName);
        Assert.Equal(policySummary.PrimaryInsured.Participant.Person.Name.IndividualFirst, mappedResponse.PrimaryInsuredFirstName);
        Assert.Equal(policySummary.ProductCategory, mappedResponse.ProductCategory);
        Assert.Equal(policySummary.ProductDescription, mappedResponse.ProductDescription);
        Assert.Equal(status, mappedResponse.Status);
        Assert.Equal(policySummary.TerminationDate, mappedResponse.TerminationDate);
        Assert.Equal(terminationReason, mappedResponse.TerminationReason);
        Assert.Equal(terminationDetail, mappedResponse.TerminationDetail);
        Assert.Equal(policySummary.PaidToDate, mappedResponse.PaidToDate);
        Assert.Equal(billingMode, mappedResponse.BillingMode);
        Assert.Equal(policySummary.ModePremium, mappedResponse.ModePremium);
        Assert.Equal(policySummary.AnnualPremium, mappedResponse.AnnualPremium);
        Assert.Equal(policySummary.FaceAmount, mappedResponse.FaceAmount);
        Assert.Equal(issueState, mappedResponse.IssueState);
        Assert.Equal(policySummary.PrimaryOwner.Participant.Person.Name.IndividualLast, mappedResponse.PrimaryOwnerLastName);
        Assert.Equal(policySummary.PrimaryOwner.Participant.Person.Name.IndividualFirst, mappedResponse.PrimaryOwnerFirstName);
        Assert.Equal(policySummary.PrimaryOwner.Participant.PhoneNumber, mappedResponse.PrimaryOwnerPhoneNumber);
        Assert.Equal(policySummary.PrimaryOwner.Participant.Person.EmailAddress, mappedResponse.PrimaryOwnerEmail);
        Assert.Equal(policySummary.PrimaryOwner.Participant.Address.Line1, mappedResponse.PrimaryOwnerAddressLine1);
        Assert.Equal(policySummary.PrimaryOwner.Participant.Address.Line2, mappedResponse.PrimaryOwnerAddressLine2);
        Assert.Equal(policySummary.PrimaryOwner.Participant.Address.City, mappedResponse.PrimaryOwnerCity);
        Assert.Equal(primaryOwnerState, mappedResponse.PrimaryOwnerState);
        Assert.Equal("First Payment Fail", mappedResponse.FirstPaymentFail);

        var fullZipCode = PolicyDetailsExportMappingProfile.GetParticipantZip(policySummary.PrimaryOwner.Participant);
        Assert.Equal(fullZipCode, mappedResponse.PrimaryOwnerZip);
        Assert.Equal(policySummary.EmployerName, mappedResponse.EmployerName);
        Assert.Equal(policySummary.EmployerNumber, mappedResponse.EmployerNumber);
        Assert.Equal(policySummary.ApplicationSignedDate, mappedResponse.ApplicationSignedDate);
        Assert.Equal(policySummary.IssueDate, mappedResponse.IssueDate);
    }

    [Theory]
    [InlineData("0000", "1111")]
    [InlineData("00001", "1111")]
    [InlineData("00001", "11110")]
    [InlineData("", "1111")]
    [InlineData("0000", "")]
    [InlineData(null, "1111")]
    [InlineData("0000", null)]
    public void PolicySummary_to_ActivePolicySummary_ShouldMapZipCode(string? zipExtension, string? boxNumber)
    {
        // Arrangement
        var policySummaryFaker = new AutoFaker<PolicyInfoAPI.PolicySummary>();
        var policySummary = policySummaryFaker.Generate();
        policySummary.PrimaryOwner.Participant.Address.ZipExtension = zipExtension;
        policySummary.PrimaryOwner.Participant.Address.BoxNumber = boxNumber;
        var expectedZipCode = new StringBuilder(policySummary.PrimaryOwner.Participant.Address.ZipCode);

        if (!string.IsNullOrEmpty(zipExtension) && zipExtension.Length == 4)
        {
            expectedZipCode.Append("-" + zipExtension);
        }
        else if (!string.IsNullOrEmpty(boxNumber) && boxNumber.Length == 4)
        {
            expectedZipCode.Append("-" + boxNumber);
        }

        // Act
        var mappedResponse = Mapper.Map<ActivePolicyExport>(policySummary);

        // Assert
        Assert.Equal(expectedZipCode.ToString(), mappedResponse.PrimaryOwnerZip);
    }

    [Theory]
    [InlineData("0000", "1111")]
    [InlineData("00001", "1111")]
    [InlineData("00001", "11110")]
    [InlineData("", "1111")]
    [InlineData("0000", "")]
    [InlineData(null, "1111")]
    [InlineData("0000", null)]
    public void PolicySummary_to_TerminatedPolicySummary_ShouldMapZipCode(string? zipExtension, string? boxNumber)
    {
        // Arrangement
        var policySummaryFaker = new AutoFaker<PolicyInfoAPI.PolicySummary>();
        var policySummary = policySummaryFaker.Generate();
        policySummary.PrimaryOwner.Participant.Address.ZipExtension = zipExtension;
        policySummary.PrimaryOwner.Participant.Address.BoxNumber = boxNumber;
        var expectedZipCode = new StringBuilder(policySummary.PrimaryOwner.Participant.Address.ZipCode);

        if (!string.IsNullOrEmpty(zipExtension) && zipExtension.Length == 4)
        {
            expectedZipCode.Append("-" + zipExtension);
        }
        else if (!string.IsNullOrEmpty(boxNumber) && boxNumber.Length == 4)
        {
            expectedZipCode.Append("-" + boxNumber);
        }

        // Act
        var mappedResponse = Mapper.Map<TerminatedPolicyExport>(policySummary);

        // Assert
        Assert.Equal(expectedZipCode.ToString(), mappedResponse.PrimaryOwnerZip);
    }

    [Theory]
    [InlineData(null, "")]
    [InlineData(BillingReason.PolicyChange, "")]
    [InlineData(BillingReason.PaidUp, "Paid Up")]
    [InlineData(BillingReason.VanishBase, "")]
    [InlineData(BillingReason.Vanish, "")]
    [InlineData(BillingReason.WaiverDisability, "")]
    [InlineData(BillingReason.Adjustment, "")]
    [InlineData(BillingReason.BillingError, "")]
    [InlineData(BillingReason.DeathPending, "")]
    [InlineData(BillingReason.EarlyPaidUp, "")]
    [InlineData(BillingReason.ExtendedTerm, "Extended Term")]
    [InlineData(BillingReason.NonForfeiture, "")]
    [InlineData(BillingReason.ReducedPaidUp, "Reduced Paid Up")]
    [InlineData(BillingReason.StoppedPremium, "")]
    [InlineData(BillingReason.WaiverPending, "")]
    [InlineData(BillingReason.None, "")]
    [InlineData(BillingReason.Unknown, "")]
    public void PolicySummary_to_ActivePolicySummary_ShouldMapBillingReason(BillingReason? billingReason, string expectedBillingReason)
    {
        // Arrangement
        var policySummaryFaker = new AutoFaker<PolicyInfoAPI.PolicySummary>();
        var policySummary = policySummaryFaker.Generate();
        policySummary.BillingReason = billingReason;

        // Act
        var mappedResponse = Mapper.Map<ActivePolicyExport>(policySummary);

        // Assert
        Assert.Equal(expectedBillingReason, mappedResponse.BillingReason);
    }

    [Theory]
    [InlineData(null, "")]
    [InlineData(BillingReason.PolicyChange, "")]
    [InlineData(BillingReason.PaidUp, "Paid Up")]
    [InlineData(BillingReason.VanishBase, "")]
    [InlineData(BillingReason.Vanish, "")]
    [InlineData(BillingReason.WaiverDisability, "")]
    [InlineData(BillingReason.Adjustment, "")]
    [InlineData(BillingReason.BillingError, "")]
    [InlineData(BillingReason.DeathPending, "")]
    [InlineData(BillingReason.EarlyPaidUp, "")]
    [InlineData(BillingReason.ExtendedTerm, "Extended Term")]
    [InlineData(BillingReason.NonForfeiture, "")]
    [InlineData(BillingReason.ReducedPaidUp, "Reduced Paid Up")]
    [InlineData(BillingReason.StoppedPremium, "")]
    [InlineData(BillingReason.WaiverPending, "")]
    [InlineData(BillingReason.None, "")]
    [InlineData(BillingReason.Unknown, "")]
    public void PolicySummary_to_TerminatedPolicySummary_ShouldMapBillingReason(BillingReason? billingReason, string expectedBillingReason)
    {
        // Arrangement
        var policySummaryFaker = new AutoFaker<PolicyInfoAPI.PolicySummary>();
        var policySummary = policySummaryFaker.Generate();
        policySummary.BillingReason = billingReason;

        // Act
        var mappedResponse = Mapper.Map<TerminatedPolicyExport>(policySummary);

        // Assert
        Assert.Equal(expectedBillingReason, mappedResponse.BillingReason);
    }
}
