## Improved security in Couchbase 4.5: SCRAM-SHA

Security is important to us, here at [Couchbase](http://developer.couchbase.com/documentation/server/current/introduction/intro.html?utm_source=blogs&utm_medium=link&utm_campaign=blogs). I'd like to draw your attention to a new security feature in Couchbase 4.5 that might otherwise go unnoticed: SCRAM-SHA (pronounced like 'scram-shaw').

![Scram! Licensed through Create Commons via Michael Pereckas - https://www.flickr.com/people/53332339@N00](https://dl.dropboxusercontent.com/u/224582/Couchbase%20Blog%20Posts/008pics/scram.jpg)

([Scram Image Licensed through Create Commons via Michael Pereckas](https://www.flickr.com/photos/beigephotos/11468976183/in/photolist-ittv3F-itsSwq))

SCRAM ([Salted Challenge Response Authentication Mechanism](https://en.wikipedia.org/wiki/Salted_Challenge_Response_Authentication_Mechanism)) is a password-based way of authenticating a user. It provides additional security against brute-force attacks, in the case that your servers are ever compromised. Previous versions of Couchbase used a CRAM-MD5 login scheme, which are more vulnerable to such attacks.

I found a great [white paper that summarizes SCRAM](http://www.isode.com/whitepapers/scram.html) and its benefits over CRAM-MD5. My summary of that summary on how SCRAM is an improvement:

- SCRAM specifies a format for a secret: hashed data value, salt value, iteration count
- This secret is stored on the server
- This secret on its own cannot be used to trick the authentication system
- SCRAM exchanges hashed items between client and server, which cannot be "played back"
- SCRAM can be used with any hash algorithm (like SHA1)

**So, what do I have to do to use SCRAM-SHA?**

Upgrade to Couchbase 4.5. The SDK will handle the details. It will use SCRAM-SHA if it can, and it will fall back to CRAM-MD5 if you are running an older version of Couchbase. Using TLS is still recommended to maximize security.

One more thing: SCRAM supports many hash algorithms. Couchbase Server supports [SHA1, SHA-256 and SHA-512](https://en.wikipedia.org/wiki/SHA-2). The SDK picks the "highest", so SHA-512 is always used.

That's the quick intro! Any questions? Leave a comment, or ask a question in the [Couchbase Forums](https://forums.couchbase.com/?utm_source=blogs&utm_medium=link&utm_campaign=blogs)

