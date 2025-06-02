namespace Assurity.AgentPortal.Utilities;

using System;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;

using Microsoft.Extensions.Logging;

public class EnumUtility
{
    public static string GetEnumDescription(Enum value)
    {
        FieldInfo fieldInfo = value.GetType().GetField(value.ToString());

        DescriptionAttribute[] attributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

        if (attributes != null && attributes.Any())
        {
            return attributes.First().Description;
        }

        return value.ToString();
    }
}
