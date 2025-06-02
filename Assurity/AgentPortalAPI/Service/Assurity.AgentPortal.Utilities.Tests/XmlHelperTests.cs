namespace Assurity.AgentPortal.Utilities.Tests
{
    using System.Diagnostics.CodeAnalysis;
    using Assurity.AgentPortal.Contracts.Integration;
    using Assurity.AgentPortal.Utilities.Integration;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class XmlHelperTests
    {
        [Fact]
        public void SerializeIllustrationProXml_ShouldReturn_Credentials_XmlDocument()
        {
            // Arrange
            var illustrationSsoInfo = new IllustrationSsoInfo
            {
                ADDRESS1 = "123 ABC St",
                CITY = "Lincoln",
                EMAIL = "abc123@gmail.com",
                FIRSTNAME = "Jacob",
                LASTNAME = "Smith",
                PHONE = "4021234567",
                STATE = "NE",
                ZIP = "68510"
            };

            var distributionChannel = "IS";
            var username = "un1234";

            // Act
            var xmlDoc = XmlHelper.SerializeIllustrationProXml(illustrationSsoInfo, distributionChannel, username);

            // Assert
            var expectedXml = "<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\"><s:Body><agentSetup xmlns=\"http://wwww/stoneriver.com/services/AgentAccountService\"><requestObject><a:accountSetupRequestXML xmlns:a=\"http://schemas.datacontract.org/2004/07/LifeServer.DataModel.request\"><AgentAccountSetupData xmlns=\"\"><AGENT><UNIQUEID>un1234</UNIQUEID><UNIQUEPASS>TEST</UNIQUEPASS><FIRSTNAME>Jacob</FIRSTNAME><LASTNAME>Smith</LASTNAME><EMAILTO></EMAILTO><ROLECODE>Default_Agent</ROLECODE><LANGUAGE>en-us</LANGUAGE><PROFILES><PROFILE><DISTRIBUTION>DIST_ASR_IS</DISTRIBUTION><AGENTID /><AGENCYID /><AGENCYNAME /><ADDRESS1>123 ABC St</ADDRESS1><ADDRESS2 /><CITY>Lincoln</CITY><STATE>NE</STATE><ZIP>68510</ZIP><PHONE>4021234567</PHONE><EMAIL>abc123@gmail.com</EMAIL></PROFILE></PROFILES></AGENT></AgentAccountSetupData></a:accountSetupRequestXML></requestObject></agentSetup></s:Body></s:Envelope>";
            Assert.NotNull(xmlDoc);
            Assert.Equal(expectedXml, xmlDoc.InnerXml);
        }

        [Fact]
        public void SerializeLifePortraitsXml_ShouldReturn_Credentials()
        {
            // Arrange
            var lifePortraitsSsoInfo = new IllustrationSsoInfo
            {
                ADDRESS1 = "123 ABC St",
                CITY = "Lincoln",
                EMAIL = "abc123@gmail.com",
                FIRSTNAME = "Jacob",
                LASTNAME = "Smith",
                PHONE = "4021234567",
                STATE = "NE",
                ZIP = "68510"
            };

            var distributionChannel = "IS";
            var agentId = "1234";

            // Act
            var xmlDoc = XmlHelper.SerializeLifePortraitsXml(lifePortraitsSsoInfo, distributionChannel, agentId);

            // Assert
            var expectedXml = "<?xml version=\"1.0\" encoding=\"utf-8\"?><FIPINPUT><AGENT><AGENTID>1234</AGENTID><FIRSTNAME>Jacob</FIRSTNAME><LASTNAME>Smith</LASTNAME><MIDDLEINITIAL></MIDDLEINITIAL></AGENT><PROFILES><PROFILE><AGENCY><AGENCYID /><AGENCYNAME /><ADDRESS1>123 ABC St</ADDRESS1><ADDRESS2 /><CITY>Lincoln</CITY><STATE>NE</STATE><ZIP>68510</ZIP><PHONE>4021234567</PHONE><EMAIL>abc123@gmail.com</EMAIL><ISBANK /></AGENCY><DISTRIBUTION>IS</DISTRIBUTION></PROFILE></PROFILES></FIPINPUT>";
            Assert.NotNull(xmlDoc);
            Assert.Equal(expectedXml, xmlDoc.InnerXml);
        }

        [Fact]
        public void SerializeIPipelineXml_ShouldReturn_Credentials_XmlDocument()
        {
            // Arrange
            var agentInfo = new IPipelineSsoInfo
            {
                AgentNumber = "1234",
                Agentname = "Jacob Smith",
                FirstName = "Jacob",
                LastName = "Smith",
                Address1 = "123 ABC St",
                City = "Lincoln",
                State = "NE",
                ZipCode = "68510",
                Email = "abc123@gmail.com",
                Phone = "4021234567",
                Fax = "4023456789"
            };

            // Act
            var result = XmlHelper.SerializeIPipelineXml(agentInfo);

            // Assert
            var expectedResult = "<iGoApplicationData><UserData><Data Name=\"AgentNumber\">1234</Data><Data Name=\"Agentname\">Jacob Smith</Data><Data Name=\"FirstName\">Jacob</Data><Data Name=\"LastName\">Smith</Data><Data Name=\"Address1\">123 ABC St</Data><Data Name=\"City\">Lincoln</Data><Data Name=\"State\">NE</Data><Data Name=\"ZipCode\">68510</Data><Data Name=\"Phone\">4021234567</Data><Data Name=\"Fax\">4023456789</Data><Data Name=\"Email\">abc123@gmail.com</Data><Data Name=\"AgentPilot\">N</Data><Data Name=\"CarrierID\">194</Data><Data Name=\"UpdateUserProfile\">True</Data></UserData></iGoApplicationData>";

            Assert.NotNull(result);
            Assert.Equal(expectedResult, result);
        }
    }
}
