// The curl construct is still in Development. This feature is intended for Development purposes only and should not be used in Production environments.

// src = the alias I've assigned to the 'source' bucket (readonly)

function OnUpdate(doc, meta) {
    if (meta.id.indexOf("ticketscan::") !== -1) {
        // query if this user is a VIP (the document id will be in a concierge document)
        let customerId = doc.customerId;
        let stmt = SELECT t.cellNumber, t.name
                   FROM tickets t
                   WHERE ANY v IN t.vips SATISFIES v == $customerId END;

        //get the concierge (there should only be one)
        let concierge = null;
        for (var record of stmt) {
            concierge = record;
        }

        // only proceed with notification if
        // the customer has a concierge
        if (concierge) {
            // lookup the customer (for their name)
            let customer = src[customerId];

            // get twilio url/credentials
            let twilioCredentials = src["twilio::credentials"];

            // construct the message to be sent to concierge
            let message = "Hello '" + concierge.name + "'. A VIP assigned to you just checked in. '" + customer.name + "' will be in '" + doc.seat + "'";
            let from = twilioCredentials.fromNumber;
            let data = "To=" + concierge.cellNumber + "&From=" + from + "&Body=" + message;

            // make a request to the twilio API
            let url = twilioCredentials.url;
            let auth = twilioCredentials.username + ": " + twilioCredentials.password;
            var result = curl(url, { "data": data, "header": ["Content-Type: x-www-form-urlencoded"], "method": "POST", "auth": auth });
        }
    }
}
function OnDelete(meta) {
}
