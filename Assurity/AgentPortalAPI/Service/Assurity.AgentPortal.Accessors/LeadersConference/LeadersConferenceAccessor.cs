namespace Assurity.AgentPortal.Accessors.LeadersConference;

using Assurity.AgentPortal.Contracts.LeadersConference;

public class LeadersConferenceAccessor : ILeadersConferenceAccessor
{
    public LeadersConferenceAccessor()
    {
    }

    public QualificationStatusSummary GetQualificationStatus(string agentId, int year, QualiferType qualiferType, CancellationToken cancellationToken)
    {
        // TODO: Implement Database call, using agent id and qualifier type to determine the query for the data
        return new QualificationStatusSummary(DateTime.Now, 62971, 90000);
    }

    public LeadersQualifiersSummary GetLeadersQualifiers(string agentId, int year, CancellationToken cancellationToken)
    {
        // TODO: Implement Database call, using agent id and year to determine the query for the data
        var fakeData = new List<string>()
        {
            "Eileen M Biehn",
            "Erin K Brzozowski",
            "Wendell J Carter**",
            "Aaron Michael Clark",
            "T Bryce Craig",
            "Joel Daniels**",
            "Larry D Denny**",
            "Paul C Dorton",
            "Christina M Eades",
            "Vincent L Echols",
            "Gary W Fitch",
            "Jennifer Fitzgerald**",
            "Jennifer A Goedken**",
            "Jayne L Gratz",
            "Wendy J Hamilton**",
            "Lynn Marie Hilger",
            "Chelsea Johnson",
            "Sherri L Kalb",
            "Ryan S Kaufman",
            "Deborah L Loman",
            "Kimberly R Marquardt",
            "Richard L Miller",
            "Kristy A Moorehead",
            "Jude E Offiah**",
            "Rebecca L Pemble**",
            "Karen J Pines**",
            "Kris R Pojar",
            "Jeannett S Ramirez",
            "Treve Rasmussen**",
            "Matthew S Rednour",
            "Shane M Ridenhour",
            "Adam J Richter",
            "Donald Sewell**",
            "Steven Swyers**",
            "Judith C Tatton**",
            "Carolyn M Turen**",
            "Samuel M Ugarte",
            "Luz M Villarreal**",
            "Douglas A Wheeler",
            "Scott M Wood"
        };

        return new LeadersQualifiersSummary(DateTime.Now, fakeData, fakeData, fakeData);
    }
}
