/* eslint-disable */

/**
 * Name: Return Original Payload
 * key: return_original_payload
 * Type: Run Script
 */


// Code: 
module.exports = async function(data) {
	// No slug updates and since filter hook, we just return original payload
    //     - For Action filter prevents error message
    //     - For Filter filter it will apply original payload instead of erroring
	return data.$trigger.payload
}

