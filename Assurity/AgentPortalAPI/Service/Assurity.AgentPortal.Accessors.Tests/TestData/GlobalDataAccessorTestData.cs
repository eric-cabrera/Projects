namespace Assurity.AgentPortal.Accessors.Tests.TestData;

using System.Diagnostics.CodeAnalysis;
using Assurity.AgentPortal.Accessors.GlobalData.Entities;

[ExcludeFromCodeCoverage]
public static class GlobalDataAccessorTestData
{
    public static List<Attributes> TestAttributes => new()
    {
        new Attributes
        {
            ObjectId = "13058DEV1000020",
            ObjectType = 3,
            PolicyNumber = "1234567890",
            ObjectClass = "DSCAN01",
            DocType = "APPLICATION",
            PrintInclude = "Y"
        },
        new Attributes
        {
            ObjectId = "12057DEV1000010",
            ObjectType = 2,
            PolicyNumber = "1234567899",
            ObjectClass = "DSCAN01",
            DocType = "APPLICATION",
            PrintInclude = "Y"
        },
        new Attributes
        {
            ObjectId = "15032DEV2000010",
            ObjectType = 3,
            PolicyNumber = "9999999991",
            ObjectClass = "DSCAN01",
            DocType = "APPLICATION",
            PrintInclude = "Y"
        },
        new Attributes
        {
            ObjectId = " 1701DDEV1000010     ",
            ObjectType = 3,
            PolicyNumber = "9999999992",
            ObjectClass = "DSCAN01",
            DocType = "APPLICATION",
            PrintInclude = "Y"
        },
        new Attributes
        {
            ObjectId = "1701CDEV1000010  ",
            ObjectType = 2,
            PolicyNumber = "9999999992",
            ObjectClass = "DSCAN01",
            DocType = "APPLICATION",
            PrintInclude = "Y"
        }
    };
}