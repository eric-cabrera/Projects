/* eslint-disable */

/**
 * Name: Return  Payload with Slug
 * key: return_payload_with_slug
 * Type: Run Script
 */


// Code: 
const slugify = text =>
    text
        .toString()
        .normalize('NFD')
        .replace(/[\u0300-\u036f]/g, '')
        .toLowerCase()
        .trim()
        .replace(/&/g, 'and')
        .replace(/\s+/g, '-')
        .replace(/[^\w-]+/g, '')
        .replace(/--+/g, '-');

module.exports = async function (data) {
    const separator = "/"
    let slug = null
    let slug_full_path = null
    const child_creates = []
    const child_updates = []
    const child_deletes = []
    let payload = {}

    if (data.$trigger.collection === "agent_center_pages") {
        //filter_fields.push("page_name", "slug", "page_hierarchy.id", "page_hierarchy.name", "page_hierarchy.slug_full_path")
        slug = separator + slugify(data.get_item_details.page_name)
        if (data.get_item_details.page_hierarchy.slug_full_path !== undefined) {
            slug = separator + data.get_item_details.page_hierarchy.slug_full_path + slug
        }

        // Prevent infinite recursion:
        if (slug === data.get_item_details.slug) throw "No change"

        payload.slug = slug
    }
    else { // agent_center_page_hierarchy
        //filter_fields.push("name","slug","slug_full_path","parent.slug_full_path","child.id","child.slug","child.slug_full_path")
        slug = slugify(data.get_item_details.name)

        if (data.get_item_details.parent === null) {
            slug_full_path = slug
        } else {
            slug_full_path = data.get_item_details.parent.slug_full_path + separator + slug
        }

        if (data.get_item_details.child !== undefined
            && data.get_item_details.child.length > 0) {
            for (let child of data.get_item_details.child) {
                // Prevent infinite recursion
                if (child.slug_full_path === slug_full_path + separator + child.slug) continue

                // Update child
                child_updates.push({
                    "id": child.id,
                    "slug_full_path": slug_full_path + separator + child.slug
                })
            }

            if (child_updates.length > 0) {
                payload.child = { "create": child_creates, "update": child_updates, "delete": child_deletes }
            }
        }

        // Prevent infinite recursion:
        if (slug_full_path === data.get_item_details.slug_full_path && payload.child === undefined) throw "No change"

        payload.slug = slug
        payload.slug_full_path = slug_full_path
    }

    return payload
}
