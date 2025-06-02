namespace Assurity.AgentPortal.Accessors.DirectusQueries
{
    public class PageQuery : GraphQLQuery
    {
        public override string Query => @"
            query GetPage (
              $slug: String
              $statuses: [String]
            ) {
              agent_center_pages(
                filter: {
                  slug: { _eq: $slug }
                  status: { _in: $statuses }
                }
            ) {
              id
              heading
              table_of_contents
              back_button_url
              back_button_text
              protected
              page_content {
                collection
                item {
                  ... on agent_center_link_anchor {
                    id
                    name
                  }
                  ... on agent_center_table_of_contents {
                    id
                    item
                  }
                  ... on agent_center_layout_column {
                    id
                    size
                    content {
                        id
                        collection
                        item {
                        ... on agent_center_call_to_action_component {
                          id
                          size
                          heading
                          subheading
                          description
                          image {
                            id
                            description
                          }
                          icon {
                            id
                          }
                          cta {
                            item
                          }
                        }
                        ... on agent_center_container {
                              status
                              size
                              heading
                              subheading
                              is_modal
                              modal_identifier
                              content {
                                collection
                                item {

                                  ... on agent_center_contact_form {
                                     id
                                     name
                                     heading
                                     description
                                     image {
                                       id
                                     }
                                     contact {
                                       id
                                       first_name
                                       last_name
                                       phone
                                       email
                                       image {
                                         id
                                       }
                                     }
                                  }
                                  ... on agent_center_text {
                                    id
                                    status
                                    heading
                                    checkbox_bullet_points
                                    html
                                  }
                                  ... on agent_center_icon_block {
                                    id
                                    heading
                                    subheading
                                    highlighted
                                    tooltip
                                    horizontal_rule
                                    icon {
                                      id
                                    }
                                    alignment
                                    content_alignment
                                    full_bleed
                                    content {
                                        collection
                                        item {
                                            ... on agent_center_accordion_block {
                                                id
                                                heading
                                                heading_description
                                                checkbox_bullet_points
                                                description
                                                icon {
                                                    id
                                                }
                                            }
                                            ... on agent_center_call_to_action {
                                                id
                                                status
                                                item
                                            }
                                            ... on agent_center_horizontal_rule {
                                                id
                                            }
                                            ... on agent_center_link_list {
                                                id
                                                status
                                                item
                                            }
                                            ... on agent_center_text {
                                                id
                                                status
                                                heading
                                                checkbox_bullet_points
                                                html
                                            }
                                         }
                                      }
                                  }
                                  ... on agent_center_accordion_block {
                                    id
                                    heading
                                    heading_description
                                    checkbox_bullet_points
                                    description
                                    icon {
                                        id
                                    }
                                  }

                                  ... on agent_center_call_to_action {
                                    id
                                    status
                                    item
                                  }
              
                                  ... on agent_center_content_block {
                                    status
                                    heading
                                    size
                                    vertical_rule
                                    highlighted
                                    icon {
                                      id
                                    }
                                    content {
                                      collection
                                      item {
                                        ... on agent_center_call_to_action {
                                          id
                                          status
                                          item
                                        }
                                        ... on agent_center_horizontal_rule {
                                          id
                                        }
                                        ... on agent_center_link_list {
                                          id
                                          status
                                          item
                                        }
                                        ... on agent_center_text {
                                          id
                                          status
                                          heading
                                          checkbox_bullet_points
                                          html
                                        }
                                      }
                                    }
                                  }
                                  ... on agent_center_horizontal_rule {
                                    id
                                  }
                                  ... on agent_center_media {
                                    id
                                    size
                                    heading
                                    image {
                                      id
                                    }
                                    alt_text
                                    video {
                                      ... on videos {
                                        video
                                        name
                                      }
                                    }
                                    content {
                                      collection
                                      item {
                                        ... on agent_center_call_to_action {
                                          id
                                          status
                                          item
                                        }
                                        ... on agent_center_text {
                                          id
                                          status
                                          heading
                                          checkbox_bullet_points
                                          html
                                        }
                                      }
                                    }
                                  }
                                }
                              }
                            }
                            ... on agent_center_document_block {
                                id
                                name
                                file {
                                    id
                                }
                                image {
                                    id
                                }
                                alt_text
                                size
                                external_file_link
                            }
                            ... on agent_center_heading {
                                id
                                heading
                                size
                            }
                        }
                    }
                  }
                  ... on agent_center_prospect_card {
                    id
                    image {
                      id
					}
                    size
                    heading
                    description
                    list_heading
                    links {
                      id
                      text
                      URL
                      icon {
                        id
                      }
                    }
                  }
                  ... on agent_center_heading {
                    id
                    heading
                    sub_text
                    size
                  }
                  ... on agent_center_marketing_materials {
					heading
					subheading
					items {
                      id
					  name
					  description
					  link
					  marked
					  embed
					  image {
						id
					  }
					  file {
						id
					  }
					}
				  }
				  ... on agent_center_contact {
                    size
					contact {
                      id
					  image {
						id
					  }
					  first_name
					  last_name
					  title
                      phone
                      email
                      description
					  contact_types {
						agent_center_contact_types_id {
						  id
						  name
						}
				      }
				    }
			      }
			      ... on agent_center_contact_type_component {
                    size 
					contact_types {
					  agent_center_contact_types_id {
						id
						name
					  }
					}
					default {
					  first_name
					  last_name
					  title
                      phone
                      email
                      description
					  image {
						id
					  }
					  contact_types {
						agent_center_contact_types_id {
					      id
						  name
						}
					  }
					}
				  }
                  ... on agent_center_call_to_action_component {
                    id
                    size
                    heading
                    subheading
                    description
                    image {
                      id
                      description
                    }
                    icon {
                      id
                    }
                    cta {
                      item
                    }
                  }
                  ... on agent_center_state_availability {
                    id
                    states {
                      agent_center_product_states_id {
                        name
                        abbreviation
                      }
                    }
                    riders {
                      agent_center_product_riders_id {
                        name
                        state_availability {
                          agent_center_product_states_id {
                            name
                            abbreviation
                          }
                        }
                      }
                    }
                    content {
                      collection
                      item {
                        ... on agent_center_call_to_action {
                          id
                          status
                          item
                        }
                      }
                    }
                  }
                  ... on agent_center_document_block {
                    id
                    name
                    file {
                      id
                    }
                    image {
                      id
                    }
                    alt_text
                    size
                    external_file_link
                  }
                  ... on agent_center_container {
                    status
                    size
                    heading
                    subheading
                    is_modal
                    modal_identifier
                    icon {
                      id
                    }
                    icon_label
                    highlighted
                    content {
                      collection
                      item {
                       ... on agent_center_contact_form {
                         id
                         name
                         heading
                         description
                         image {
                           id
                         }
                         contact {
                           id
                           first_name
                           last_name
                           phone
                           email
                           image {
                             id
                           }
                         }
                       }
                       ... on agent_center_text {
                                id
                                status
                                heading
                                checkbox_bullet_points
                                html
                        }
                        ... on agent_center_call_to_action {
                          id
                          status
                          item
                        }
                        ... on agent_center_contracting_form {
                          id
                        }
                        ... on agent_center_vertafore_redirect_button {
                          id
                        }
                        ... on agent_center_accordion_block {
                          id
                          heading
                          heading_description
                          description
                          checkbox_bullet_points
                          icon {
                            id
                          }
                        }
                        ... on agent_center_icon_block {
                          id
                          heading
                          subheading
                          highlighted
                          tooltip
                          horizontal_rule
                          icon {
                            id
                          }
                          alignment
                          content_alignment
                          full_bleed
                          content {
                            collection
                            item {
                              ... on agent_center_accordion_block {
                                id
                                heading
                                heading_description
                                checkbox_bullet_points
                                description
                                icon {
                                  id
                                }
                              }
                              ... on agent_center_call_to_action {
                                id
                                status
                                item
                              }
                              ... on agent_center_horizontal_rule {
                                id
                              }
                              ... on agent_center_link_list {
                                id
                                status
                                item
                              }
                              ... on agent_center_text {
                                id
                                status
                                heading
                                checkbox_bullet_points
                                html
                              }
                            }
                          }
                        }
                        ... on agent_center_content_block {
                          status
                          heading
                          subheading
                          tooltip
                          size
                          vertical_rule
                          horizontal_rule
                          horizontal_rule_after_header
                          highlighted
                          icon_left
                          icon {
                            id
                          }
                          content {
                            collection
                            item {
                              ... on agent_center_accordion_block {
                                id
                                heading
                                heading_description
                                checkbox_bullet_points
                                description
                                icon {
                                  id
                                }
                              }
                              ... on agent_center_call_to_action {
                                id
                                status
                                item
                              }
                              ... on agent_center_horizontal_rule {
                                id
                              }
                              ... on agent_center_link_list {
                                id
                                status
                                item
                              }
                              ... on agent_center_text {
                                id
                                status
                                heading
                                checkbox_bullet_points
                                html
                              }
                            }
                          }
                        }
                        ... on agent_center_horizontal_rule {
                          id
                        }
                        ... on agent_center_media {
                          id
                          size
                          heading
                          image {
                            id
                          }
                          alt_text
                          video {
                            video
                            name
                          }
                          content {
                            collection
                            item {
                              ... on agent_center_call_to_action {
                                id
                                status
                                item
                              }
                              ... on agent_center_text {
                                id
                                status
                                heading
                                checkbox_bullet_points
                                html
                              }
                            }
                          }
                        }
                        ... on agent_center_tabs {
                          heading
                          tabs {
                            label
                            content {
                              collection
                              item {
                                ... on agent_center_container {
                                  status
                                  size
                                  heading
                                  subheading
                                  content {
                                    collection
                                    item {
                                      ... on agent_center_accordion_block {
                                        id
                                        heading
                                        heading_description
                                        description
                                        checkbox_bullet_points
                                        icon {
                                          id
                                        }
                                      }
                                      ... on agent_center_call_to_action {
                                        id
                                        status
                                        item
                                      }
                                      ... on agent_center_text {
                                        id
                                        status
                                        heading
                                        checkbox_bullet_points
                                        html
                                      }                               
                                    }
                                  }
                                }
                              }
                            }
                          }
                        }
                      }
                    }
                  }
                  ... on agent_center_load_more {
                    heading
                    content {
                      collection
                      item {
                        ... on agent_center_prospect_card {
                          id
                          image {
                              id
					      }
                          size
                          heading
                          description
                          list_heading
                          links {
                            id
                            text
                            URL
                            icon {
                            id
                            }
                          }
                        }
                        ... on agent_center_container {
                          status
                          size
                          heading
                          subheading
                          is_modal
                          modal_identifier
                          content {
                            collection
                            item {
                              ... on agent_center_content_block {
                                status
                                heading
                                size
                                vertical_rule
                                highlighted
                                icon {
                                  id
                                }
                                content {
                                  collection
                                  item {
                                    ... on agent_center_call_to_action {
                                      id
                                      status
                                      item
                                    }
                                    ... on agent_center_horizontal_rule {
                                      id
                                    }
                                    ... on agent_center_link_list {
                                      id
                                      status
                                      item
                                    }
                                    ... on agent_center_text {
                                      id
                                      status
                                      heading
                                      checkbox_bullet_points
                                      html
                                    }
                                  }
                                }
                              }
                              ... on agent_center_horizontal_rule {
                                id
                              }
                              ... on agent_center_media {
                                id
                                size
                                heading
                                image {
                                  id
                                }
                                alt_text
                                video {
                                  ... on videos {
                                    video
                                    name
                                  }
                                }
                                content {
                                  collection
                                  item {
                                    ... on agent_center_call_to_action {
                                      id
                                      status
                                      item
                                    }
                                    ... on agent_center_text {
                                      id
                                      status
                                      heading
                                      checkbox_bullet_points
                                      html
                                    }
                                  }
                                }
                              }
                            }
                          }
                        }
                      }
                    }
                  }
                  ... on agent_center_tabs_external {
                    heading
                    tabs {
                      id
                      label
                      URL
                      show_lock_icon
                      page {
                        slug
                      }
                    }
                  }
                  ... on agent_center_tabs {
                    heading
                    tabs {
                      label
                      content {
                        collection
                        item {
                          ... on agent_center_container {
                            status
                            size
                            heading
                            subheading
                            content {
                              collection
                              item {
                                ... on agent_center_accordion_block {
                                  id
                                  heading
                                  heading_description
                                  checkbox_bullet_points
                                  description
                                  icon {
                                    id
                                  }
                                }
                                ... on agent_center_content_block {
                                  status
                                  heading
                                  size
                                  vertical_rule
                                  highlighted
                                  icon {
                                    id
                                  }
                                  content {
                                    collection
                                    item {
                                      ... on agent_center_accordion_block {
                                        id
                                        heading
                                        heading_description
                                        checkbox_bullet_points
                                        description
                                        icon {
                                          id
                                        }
                                      }
                                      ... on agent_center_call_to_action {
                                        id
                                        status
                                        item
                                      }
                                      ... on agent_center_horizontal_rule {
                                        id
                                      }
                                      ... on agent_center_link_list {
                                        id
                                        status
                                        item
                                      }
                                      ... on agent_center_text {
                                        id
                                        status
                                        heading
                                        checkbox_bullet_points
                                        html
                                      }
                                    }
                                  }
                                }
                                ... on agent_center_horizontal_rule {
                                  id
                                }
                                ... on agent_center_media {
                                  id
                                  size
                                  heading
                                  image {
                                    id
                                  }
                                  alt_text
                                  video {
                                    ... on videos {
                                      video
                                      name
                                    }
                                  }
                                  content {
                                    collection
                                    item {
                                      ... on agent_center_call_to_action {
                                        id
                                        status
                                        item
                                      }
                                      ... on agent_center_text {
                                        id
                                        status
                                        heading
                                        checkbox_bullet_points
                                        html
                                      }
                                    }
                                  }
                                }
                              }
                            }
                          }
                        }
                      }
                    }
                  }
                  ... on agent_center_video_block {
                    heading
                    size
                    video {
                      video
                      name
                    }
                  }
                }
              }
            }
          }";
    }
}
