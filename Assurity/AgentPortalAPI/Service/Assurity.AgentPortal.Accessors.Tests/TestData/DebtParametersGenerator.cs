namespace Assurity.AgentPortal.Accessors.Tests.TestData;

using System.Collections;
using Assurity.AgentPortal.Accessors.DTOs;
using Assurity.AgentPortal.Contracts.Enums;

public class DebtParametersGenerator : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[]
        {
            "AAXB",
            null,
        };

        yield return new object[]
        {
            "AAXB",
            new DebtParameters
            {
                AgentId = "abc123",
            },
        };

        yield return new object[]
        {
            "AAXB",
            new DebtParameters
            {
                Page = 13,
                PageSize = 100,
            },
        };

        yield return new object[]
        {
            "AAXB",
            new DebtParameters
            {
                PolicyNumber = "abasdfg234234",
            }
        };

        yield return new object[]
        {
            "AAXB",
            new DebtParameters
            {
                WritingAgentIds = new List<string>
                {
                    "sjfdo8",
                    "j9of8es",
                }
            }
        };

        yield return new object[]
        {
            "AAXB",
            new DebtParameters
            {
                Status = "Active",
            }
        };

        yield return new object[]
        {
            "AAXB",
            new DebtParameters
            {
                OrderBy = DebtOrderBy.AgentName,
            }
        };

        yield return new object[]
        {
            "AAXB",
            new DebtParameters
            {
                OrderBy = DebtOrderBy.AgentName,
                SortDirection = SortDirection.DESC
            }
        };

        yield return new object[]
        {
            "AAXB",
            new DebtParameters
            {
                OrderBy = DebtOrderBy.AgentId,
                SortDirection = SortDirection.ASC,
            }
        };

        yield return new object[]
        {
            "AAXB",
            new DebtParameters
            {
                OrderBy = DebtOrderBy.AgentId,
                SortDirection = SortDirection.DESC
            }
        };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
