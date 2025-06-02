/* eslint-disable */

/**
 * Name: Set Item ID
 * Key: set_item_id
 * Type: Run Script 
 */

// Run Script
module.exports = async function (data) {
    const filter_fields = []

    // Do we need to update slug?
    if (data.$trigger.collection === "agent_center_pages") {
        // Only change when name or hierarchy or both changes
        if (data.$trigger.payload.page_name === undefined
            && data.$trigger.payload.page_hierarchy === undefined) throw "No Change"
        filter_fields.push("page_name", "slug", "page_hierarchy.id", "page_hierarchy.name", "page_hierarchy.slug_full_path")
    } else { // agent_center_page_hierarchy
        // Only change when name or parent or both changes
        if (data.$trigger.payload.name === undefined
            && data.$trigger.payload.parent === undefined
            && data.$trigger.payload.slug_full_path === undefined) throw "No Change"
        filter_fields.push("name", "slug", "slug_full_path", "parent.slug_full_path", "child.id", "child.slug", "child.slug_full_path")
    }

    //TBD
    // Determine if this case should be handled
    //     - payload may include manual slug input
    //     - eg user/api/... entered manual slug
    // Is this allowed?
    //     - If not, ensure permissions lock the slug field(s), etc.
    //     - If yes, determine logic and processing -- this will currently cause error in final runscript operation payload
    //if (data.$trigger.payload.slug !== undefined) return data.$trigger.payload

    const item_id = data.$trigger.key ? data.$trigger.key : data.$trigger.keys[0]

    return {
        "item_id": item_id,
        "filter_fields": filter_fields
    }
}