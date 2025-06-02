namespace Assurity.AgentPortal.Accessors.DirectusQueries;

public class CommissionDatesQuery : GraphQLQuery
{
    public CommissionDatesQuery()
    {
        AddArgument("today", DateTime.Now);
    }

    public override string Query => @"
        query GetCommissionDates($today: String, $status: [String])
        {
            agent_center_commission_dates(
                filter: 
                {
                    publish_date: { _lte: $today }
                    archive_date: { _gte: $today }
                    status: { _in: $status }
                })
                {
                    commissions_processed
                    statements_available
                    direct_deposit
                }
        }";
}
