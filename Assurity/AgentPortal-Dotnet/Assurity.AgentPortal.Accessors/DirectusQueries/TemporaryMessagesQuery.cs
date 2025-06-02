namespace Assurity.AgentPortal.Accessors.DirectusQueries;

public class TemporaryMessagesQuery : GraphQLQuery
{
    public TemporaryMessagesQuery()
    {
        AddArgument("today", DateTime.Now);
    }

    public override string Query => @"
            query GetTemporaryMessages($today: String, $status: [String])
            {
              temp_messages(
                filter: 
                {
                    publish_date: { _lte: $today }
                    archive_date: { _gte: $today }
                    status: { _in: $status }
                }) 
                {
                    id
                    heading
                    message
                    publish_date
                    cta_label
                    cta_link
                }
           }";
}
