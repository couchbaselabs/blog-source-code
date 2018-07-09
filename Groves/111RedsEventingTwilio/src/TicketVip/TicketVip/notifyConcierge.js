// The curl construct is still in Development. This feature is intended for Development purposes only and should not be used in Production environments.

// src = the alias I've assigned to the 'source' bucket (readonly)

function OnUpdate(doc, meta) {
    // tag::isticketscan[]
    if (meta.id.indexOf("ticketscan::") !== -1) {
    // end::isticketscan[]
        // query if this user is a VIP (the document id will be in a concierge document)
        // tag::isvip[]
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
        // end::isvip[]
            // lookup the customer (for their name)
            // tag::getvipinfo[]
            let customer = src[customerId];
            // end::getvipinfo[]

            // get twilio url/credentials
            // tag::gettwilio[]
            let twilioCredentials = src["twilio::credentials"];
            // end::gettwilio[]

            // construct the message to be sent to concierge
            // tag::message[]
            let message = "Hello '" + concierge.name + "'. A VIP assigned to you just checked in. '" + customer.name + "' will be in '" + doc.seat + "'";
            let from = twilioCredentials.fromNumber;
            let data = "To=" + concierge.cellNumber + "&From=" + from + "&Body=" + message;
            // end::message[]

            // make a request to the twilio API
            // tag::twilio[]
            let url = twilioCredentials.url;
            let auth = twilioCredentials.username + ": " + twilioCredentials.password;
            var result = curl(url, { "data": data, "header": ["Content-Type: x-www-form-urlencoded"], "method": "POST", "auth": auth });
            // end::twilio[]
        }
    }
}
function OnDelete(meta) {
}
