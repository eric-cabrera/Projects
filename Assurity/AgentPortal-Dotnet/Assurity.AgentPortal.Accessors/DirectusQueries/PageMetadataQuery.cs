namespace Assurity.AgentPortal.Accessors.DirectusQueries
{
    public class PageMetadataQuery : GraphQLQuery
    {
        public override string Query => @"
            query GetPageMetadata (
                $slug: String
            ) {
                agent_center_pages (
                    filter: {
                        slug: { _eq: $slug }
                    }
                ) {
                    id
                    slug
                    meta_data {
                        title
                        description
                        og_title
                        og_description
                    }
                }
            }";
    }
}
