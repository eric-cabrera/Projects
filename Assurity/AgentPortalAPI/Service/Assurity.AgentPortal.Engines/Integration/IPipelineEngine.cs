namespace Assurity.AgentPortal.Engines.Integration
{
    using System;
    using System.Security.Cryptography.X509Certificates;
    using System.Web;
    using Assurity.AgentPortal.Contracts.Integration;
    using Assurity.AgentPortal.Utilities.Configs;
    using Assurity.AgentPortal.Utilities.Integration;
    using ComponentSpace.SAML2.Assertions;
    using ComponentSpace.SAML2.Protocols;
    using ComponentSpace.SAML2.Utility;

    using static Microsoft.IdentityModel.Tokens.Saml2.Saml2Constants;
    using static Org.BouncyCastle.Math.EC.ECCurve;

    public class IPipelineEngine : IIPipelineEngine
    {
        public IPipelineEngine(
            IConfigurationManager configurationManager)
        {
            ConfigurationManager = configurationManager;
        }

        private IConfigurationManager ConfigurationManager { get; }

        public string GetBrowserPostSamlSignature(
            IPipelineSsoInfo agentInfo)
        {
            var samlResponse = new SAMLResponse();
            samlResponse.Destination = ConfigurationManager.IPipelineConnectionString;
            samlResponse.Status = new Status("urn:oasis:names:tc:SAML:2.0:status:Success", "Success");

            var assuritySamlIssuer = ConfigurationManager.IPipelineAssuritySamlIssuer;
            samlResponse.Issuer = new Issuer(assuritySamlIssuer);

            samlResponse.Assertions.Add(CreateAssertion(ConfirmationMethods.BearerString, agentInfo, false));

            var responseXml = samlResponse.ToXml();
            X509Certificate2 cert = GetCertificateFile();
            SAMLMessageSignature.Generate(responseXml, cert.PrivateKey, cert);

            var samlResponseBase64Str = SAML.ToBase64String(responseXml);

            var signedSaml = HttpUtility.HtmlEncode(samlResponseBase64Str);
            return signedSaml;
        }

        private X509Certificate2 GetCertificateFile()
        {
            X509Certificate2 cert = null;

            try
            {
                cert = new X509Certificate2(
                    ConfigurationManager.Environment == "PROD"
                        ? Properties.Resources.ipipelineprod
                        : Properties.Resources.ipipelinetest,
                    "@ssurity1",
                    X509KeyStorageFlags.MachineKeySet);
            }
            catch (Exception)
            {
                throw;
            }

            return cert;
        }

        private SAMLAssertion CreateAssertion(string confirmationMethod, IPipelineSsoInfo agentInfo, bool noZander)
        {
            var assertion = new SAMLAssertion();
            assertion.Issuer = new Issuer(ConfigurationManager.IPipelineAssuritySamlIssuer);

            var subjectConfirmation = new SubjectConfirmation
            {
                NameID = new NameID
                {
                    Format = "urn:oasis:names:tc:SAML:2.0:nameid-format:transient"
                },
                SubjectConfirmationData = new SubjectConfirmationData
                {
                    Recipient = ConfigurationManager.IPipelineConnectionString,
                    NotOnOrAfter = DateTime.Now.AddHours(1).ToUniversalTime()
                },
                Method = confirmationMethod
            };

            assertion.Subject = new Subject()
            {
                NameID = new NameID
                {
                    Format = NameIdentifierFormats.X509SubjectNameString,
                    NameQualifier = ConfigurationManager.IPipelineDomain,
                    NameIdentifier = "uid=" + agentInfo.AgentNumber
                },

                SubjectConfirmations = new List<SubjectConfirmation>
                {
                    subjectConfirmation
                }
            };

            var audienceRestriction = new AudienceRestriction
            {
                Audiences = new List<Audience>
                {
                    new Audience
                    {
                        URI = ConfigurationManager.IPipelineAudienceString
                    }
                }
            };

            assertion.Conditions = new Conditions(new TimeSpan(1, 0, 0));
            assertion.Conditions.ConditionsList.Add(audienceRestriction);

            var authnStatement = new AuthnStatement
            {
                AuthnContext = new AuthnContext
                {
                    AuthnContextClassRef = new AuthnContextClassRef
                    {
                        URI = "urn:oasis:names:tc:SAML:2.0:ac:classes:unspecified"
                    }
                },
                AuthnInstant = DateTime.Now.ToUniversalTime()
            };

            assertion.Statements.Add(authnStatement);

            var attributeStatement = new AttributeStatement();
            attributeStatement.Attributes.Add(new SAMLAttribute("ChannelName", "urn:oasis:names:tc:SAML:2.0:attrname-format:basic", null, "xs:string", "ASS"));
            attributeStatement.Attributes.Add(new SAMLAttribute("Action", "urn:oasis:names:tc:SAML:2.0:attrname-format:basic", null, "xs:string", "CREATE"));

            var zanderCompanyId = ConfigurationManager.IPipelineZanderCompanyId;
            var assurityCompanyId = ConfigurationManager.IPipelineAssurityCompanyId;
            attributeStatement.Attributes.Add(new SAMLAttribute(
                "CompanyIdentifier",
                "urn:oasis:names:tc:SAML:2.0:attrname-format:basic",
                null,
                "xs:string",
                IsZander(agentInfo.AgentNumber, noZander) ? zanderCompanyId : assurityCompanyId));

            var assurityGroupName = ConfigurationManager.IPipelineAssurityGroupName;
            var zanderGroupName = ConfigurationManager.IPipelineZanderGroupName;
            attributeStatement.Attributes.Add(new SAMLAttribute(
                "Groups",
                "urn:oasis:names:tc:SAML:2.0:attrname-format:basic",
                null,
                "xs:string",
                IsZander(agentInfo.AgentNumber, noZander) ? zanderGroupName : assurityGroupName));

            var timeoutUrl = ConfigurationManager.BaseAgentCenterUrl + "/public/ipipeline-timeout";
            attributeStatement.Attributes.Add(new SAMLAttribute(
                "TimeoutURL",
                "urn:oasis:names:tc:SAML:2.0:attrname-format:basic",
                null,
                "xs:string",
                timeoutUrl));

            attributeStatement.Attributes.Add(new SAMLAttribute(
                "ApplicationData",
                "urn:oasis:names:tc:SAML:2.0:attrname-format:basic",
                null,
                "xs:string",
                XmlHelper.SerializeIPipelineXml(agentInfo, IsZander(agentInfo.AgentNumber, noZander) ? "Zander" : string.Empty)));

            assertion.Statements.Add(attributeStatement);

            return assertion;
        }

        private bool IsZander(string agentId, bool noZander)
        {
            return agentId == ConfigurationManager.IPipelineZanderAgentId && !noZander;
        }
    }
}
