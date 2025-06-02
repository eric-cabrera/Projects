namespace Assurity.AgentPortal.Accessors.DirectusQueries.Contracting
{
    public class ContractsQuery : GraphQLQuery
    {
        public override string Query => @"
            query GetContracts (
              $marketCodes: [String]
            ) {
              agent_center_contracts(
                filter: { 
                  market_code: { _in: $marketCodes }
                }
              ) {
                display_name
                agent_level
                file {
                  id
                  filename_download
                }
              }
            }";
    }
}
