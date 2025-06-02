namespace Assurity.AgentPortal.Managers;

using Assurity.AgentPortal.Utilities;

public class FormsManager : IFormsManager
{
    public FormsManager(IConfigurationManager configurationManager)
    {
        ConfigurationManager = configurationManager;
    }

    private IConfigurationManager ConfigurationManager { get; set; }

    public string GetIndividualForms()
    {
        return GetFormsResponse(ConfigurationManager.IPipelineFormsIndividualFormsCompanyIdentifier);
    }

    public string GetWorksiteForms()
    {
        return GetFormsResponse(ConfigurationManager.IPipelineFormsWorksiteFormsCompanyIdentifier);
    }

    private string GetFormsResponse(string companyIdentifier)
    {
        var applicationData = "<?xml version='1.0' ?><iPipelineApplicationData xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'><GeneralInformation><ApplicationIdentifier/><RedirectURL>";
        applicationData += ConfigurationManager.IPipelineFormsTargetString + companyIdentifier;
        applicationData += "</RedirectURL><CompanyUserName>";
        applicationData += ConfigurationManager.IPipelineFormsUserName;
        applicationData += "</CompanyUserName><CompanyPassword>";
        applicationData += ConfigurationManager.IPipelineFormsPassword;
        applicationData += "</CompanyPassword></GeneralInformation><CompanyIdentifier>";
        applicationData += companyIdentifier;
        applicationData += "</CompanyIdentifier><iGOForms/><CustomerData/></iPipelineApplicationData>";

        return applicationData;
    }
}