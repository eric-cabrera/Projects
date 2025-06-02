namespace Assurity.AgentPortal.Utilities.Integration;

using System.Text;
using System.Xml;
using Assurity.AgentPortal.Contracts.Integration;

public static class XmlHelper
{
    public static string SerializeIPipelineXml(object data, string groupName = "")
    {
        var setDisconnectMode = false;
        StringWriter stringWriter = new StringWriter();
        XmlWriterSettings settings = new XmlWriterSettings()
        {
            OmitXmlDeclaration = true,
            Indent = false,
            Encoding = Encoding.UTF8
        };

        var properties = data.GetType().GetProperties();

        XmlWriter writer = XmlTextWriter.Create(stringWriter, settings);
        writer.WriteStartElement("iGoApplicationData");
        writer.WriteStartElement("UserData");

        foreach (var property in properties)
        {
            if (property.GetValue(data) != null)
            {
                WriteSamlXmlElement(ref writer, property.Name, property.GetValue(data).ToString());
                if (property.Name == "Initial_DisconnectedPassword" && !string.IsNullOrEmpty(property.GetValue(data).ToString()))
                {
                    setDisconnectMode = true;
                }
            }
        }

        if (!string.IsNullOrEmpty(groupName))
        {
            WriteSamlXmlElement(ref writer, "GroupName", groupName);
        }

        WriteSamlXmlElement(ref writer, "CarrierID", "194");

        WriteSamlXmlElement(ref writer, "UpdateUserProfile", "True");
        if (setDisconnectMode)
        {
            writer.WriteEndElement();
            writer.WriteStartElement("Session");
            WriteSamlXmlElement(ref writer, "ForceDisconnectedReset", "True");
        }

        writer.WriteEndElement();
        writer.WriteEndElement();
        writer.Close();

        return stringWriter.ToString();
    }

    public static XmlDocument SerializeIllustrationProXml(IllustrationSsoInfo lifePortraitsInfo, string distributionChannel, string username)
    {
        var xml = new XmlDocument();

        var soapEnvelope = xml.CreateElement("s", "Envelope", "http://schemas.xmlsoap.org/soap/envelope/");
        xml.AppendChild(soapEnvelope);

        var soapBody = xml.CreateElement("s", "Body", "http://schemas.xmlsoap.org/soap/envelope/");
        soapEnvelope.AppendChild(soapBody);

        var agentSetup = xml.CreateElement("agentSetup");
        agentSetup.SetAttribute("xmlns", "http://wwww/stoneriver.com/services/AgentAccountService");
        soapBody.AppendChild(agentSetup);

        var requestObject = xml.CreateElement("requestObject");
        agentSetup.AppendChild(requestObject);

        var accountSetupRequestXML = xml.CreateElement("a", "accountSetupRequestXML", "http://schemas.datacontract.org/2004/07/LifeServer.DataModel.request");
        requestObject.AppendChild(accountSetupRequestXML);

        var agentAccountSetupData = xml.CreateElement("AgentAccountSetupData");
        agentAccountSetupData.SetAttribute("xmlns", string.Empty);
        accountSetupRequestXML.AppendChild(agentAccountSetupData);

        var agent = xml.CreateElement("AGENT");
        agentAccountSetupData.AppendChild(agent);

        WriteLifePortraitsXmlElement(ref xml, ref agent, "UNIQUEID", username);
        WriteLifePortraitsXmlElement(ref xml, ref agent, "UNIQUEPASS", "TEST");
        WriteLifePortraitsXmlElement(ref xml, ref agent, "FIRSTNAME", lifePortraitsInfo.FIRSTNAME);
        WriteLifePortraitsXmlElement(ref xml, ref agent, "LASTNAME", lifePortraitsInfo.LASTNAME);
        WriteLifePortraitsXmlElement(ref xml, ref agent, "EMAILTO", string.Empty);
        WriteLifePortraitsXmlElement(ref xml, ref agent, "ROLECODE", "Default_Agent");
        WriteLifePortraitsXmlElement(ref xml, ref agent, "LANGUAGE", "en-us");

        var profiles = xml.CreateElement("PROFILES");
        agent.AppendChild(profiles);

        var profile = xml.CreateElement("PROFILE");
        profiles.AppendChild(profile);

        WriteLifePortaitsXmlNode(ref xml, ref profile, "DISTRIBUTION", string.Concat("DIST_ASR_", distributionChannel));
        WriteLifePortaitsXmlNode(ref xml, ref profile, "AGENTID", lifePortraitsInfo.AGENCYID);
        WriteLifePortaitsXmlNode(ref xml, ref profile, "AGENCYID", lifePortraitsInfo.AGENCYID);
        WriteLifePortaitsXmlNode(ref xml, ref profile, "AGENCYNAME", lifePortraitsInfo.AGENCYNAME);
        WriteLifePortaitsXmlNode(ref xml, ref profile, "ADDRESS1", lifePortraitsInfo.ADDRESS1);
        WriteLifePortaitsXmlNode(ref xml, ref profile, "ADDRESS2", lifePortraitsInfo.ADDRESS2);
        WriteLifePortaitsXmlNode(ref xml, ref profile, "CITY", lifePortraitsInfo.CITY);
        WriteLifePortaitsXmlNode(ref xml, ref profile, "STATE", lifePortraitsInfo.STATE);
        WriteLifePortaitsXmlNode(ref xml, ref profile, "ZIP", lifePortraitsInfo.ZIP);
        WriteLifePortaitsXmlNode(ref xml, ref profile, "PHONE", lifePortraitsInfo.PHONE);
        WriteLifePortaitsXmlNode(ref xml, ref profile, "EMAIL", lifePortraitsInfo.EMAIL);

        return xml;
    }

    public static XmlDocument SerializeLifePortraitsXml(IllustrationSsoInfo lifePortraitsInfo, string distributionChannel, string username)
    {
        var xml = new XmlDocument();
        var root = xml.CreateElement("FIPINPUT");
        var agent = xml.CreateElement("AGENT");
        var agency = xml.CreateElement("AGENCY");
        var profiles = xml.CreateElement("PROFILES");
        var profile = xml.CreateElement("PROFILE");

        xml.AppendChild(xml.CreateXmlDeclaration("1.0", "utf-8", null));
        xml.AppendChild(root);
        root.AppendChild(agent);

        WriteLifePortraitsXmlElement(ref xml, ref agent, "AGENTID", username);
        WriteLifePortraitsXmlElement(ref xml, ref agent, "FIRSTNAME", !string.IsNullOrEmpty(lifePortraitsInfo?.FIRSTNAME) ? FirstName(lifePortraitsInfo?.FIRSTNAME) : FirstName(lifePortraitsInfo.AGENCYNAME));
        WriteLifePortraitsXmlElement(ref xml, ref agent, "LASTNAME", !string.IsNullOrEmpty(lifePortraitsInfo.LASTNAME) ? LastName(lifePortraitsInfo.LASTNAME) : LastName(lifePortraitsInfo.AGENCYNAME));
        WriteLifePortraitsXmlElement(ref xml, ref agent, "MIDDLEINITIAL", string.Empty);

        root.AppendChild(profiles);
        profiles.AppendChild(profile);
        profile.AppendChild(agency);

        WriteLifePortaitsXmlNode(ref xml, ref agency, "AGENCYID", lifePortraitsInfo.AGENCYID);
        WriteLifePortaitsXmlNode(ref xml, ref agency, "AGENCYNAME", lifePortraitsInfo.AGENCYNAME);
        WriteLifePortaitsXmlNode(ref xml, ref agency, "ADDRESS1", lifePortraitsInfo.ADDRESS1);
        WriteLifePortaitsXmlNode(ref xml, ref agency, "ADDRESS2", lifePortraitsInfo.ADDRESS2);
        WriteLifePortaitsXmlNode(ref xml, ref agency, "CITY", lifePortraitsInfo.CITY);
        WriteLifePortaitsXmlNode(ref xml, ref agency, "STATE", lifePortraitsInfo.STATE);
        WriteLifePortaitsXmlNode(ref xml, ref agency, "ZIP", lifePortraitsInfo.ZIP);
        WriteLifePortaitsXmlNode(ref xml, ref agency, "PHONE", lifePortraitsInfo.PHONE);
        WriteLifePortaitsXmlNode(ref xml, ref agency, "EMAIL", lifePortraitsInfo.EMAIL);
        WriteLifePortaitsXmlNode(ref xml, ref agency, "ISBANK", lifePortraitsInfo.ISBANK);

        WriteLifePortraitsXmlElement(ref xml, ref profile, "DISTRIBUTION", distributionChannel);

        return xml;
    }

    private static void WriteLifePortraitsXmlElement(ref XmlDocument doc, ref XmlElement parent, string attributeName, string attributeValue)
    {
        var element = doc.CreateElement(attributeName);
        element.InnerText = attributeValue;
        parent.AppendChild(element);
    }

    private static void WriteLifePortaitsXmlNode(ref XmlDocument doc, ref XmlElement parent, string attributeName, string attributeValue)
    {
        XmlNode node = doc.CreateElement(attributeName);

        if (attributeValue != null)
        {
            node.InnerText = attributeValue;
        }

        parent.AppendChild(node);
    }

    private static void WriteSamlXmlElement(ref XmlWriter writer, string attributeName, string attributeValue)
    {
        writer.WriteStartElement("Data");
        writer.WriteAttributeString("Name", attributeName);
        writer.WriteValue(attributeValue);
        writer.WriteEndElement();
    }

    private static string FirstName(string val)
    {
        if (val != null)
        {
            val = val.Trim();

            if (val.Length > 30 && val.Contains(" "))
            {
                val = val.Substring(0, val.IndexOf(" "));
            }
        }

        return Left(val, 30);
    }

    private static string LastName(string val)
    {
        if (val != null)
        {
            val = val.Trim();

            if (val.Length > 30 && val.Contains(" "))
            {
                val = val.Substring(val.IndexOf(" ") + 1);
            }
        }

        return Left(val, 30);
    }

    private static string Left(string val, int len)
    {
        string valReturn = string.Empty;

        if (val != null)
        {
            val = val.Trim();

            if (val.Length > len)
            {
                valReturn = val.Substring(0, len);
            }
            else
            {
                valReturn = val;
            }
        }

        return valReturn;
    }
}