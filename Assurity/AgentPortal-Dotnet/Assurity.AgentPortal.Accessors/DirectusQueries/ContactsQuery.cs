namespace Assurity.AgentPortal.Accessors.DirectusQueries;

public class ContactsQuery : GraphQLQuery
{
    public override string Query => """
        query GetContacts {
            agent_center_contacts (
                filter: {
                    status: { _eq: "published" }
                }
            ) {
                id
                first_name
                last_name
                title
                phone
                email
                image {
                    id
                }
                description
                contact_types {
                    agent_center_contact_types_id {
                        name
                    }
                }
                region {
                    display_name
                    states
                }
            }
        }
        """;
}
