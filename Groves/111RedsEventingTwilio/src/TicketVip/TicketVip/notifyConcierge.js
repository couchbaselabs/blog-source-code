// The curl construct is still in Development. This feature is intended for Development purposes only and should not be used in Production environments.

function OnUpdate(doc, meta) {
    if (meta.id.indexOf("ticketscan::") !== -1) {
        let url = "https://api.twilio.com/2010-04-01/Accounts/AC02852279e7ea0d831fb93b7ca802f0f5/Messages.json";
        let data = "To=+16142142474&From=+16144685219&Body=testmessage1";
        let auth = "AC02852279e7ea0d831fb93b7ca802f0f5: c87e0d477ad79443d52be303c6d25ac9";
        var result = curl(url, { "data": data, "header": ["Content-Type: x-www-form-urlencoded"], "method": "POST", "auth": auth });
    }
}
function OnDelete(meta) {
}
