using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Couchbase.Core;
using Couchbase.N1QL;
using RestSharp;

namespace ViberChatbot
{
    public class ViberProcessor
    {
        private static string ViberKey = "put your viber token here";
        private static readonly Random Rand = new Random();
        private readonly IBucket _bucket;

        public ViberProcessor(IBucket bucket)
        {
            _bucket = bucket;
        }

        // tag::Process[]
        public void Process(ViberIncoming incoming)
        {
            if (incoming?.Message?.Type == "text")
            {
                LogIncoming(incoming);
                ProcessMessage(incoming);
            }
        }
        // end::Process[]

        private void LogIncoming(ViberIncoming incoming)
        {
            // generate a unique key of the form "incoming::{random string}"
            var key = "incoming::" + Path.GetRandomFileName().Replace(".", "");

            // log the timestampe, the message, and the sender
            _bucket.Insert(key, new
            {
                Timestamp = DateTime.Now,
                Message = incoming.Message,
                Sender = incoming.Sender
            });
        }

        private void ProcessMessage(ViberIncoming incoming)
        {
            // chat logic here
            // a more robust chat/natural language framework/tool would be
            // a better fit for complex chat logic

            // if message is "help" then provide help
            if (incoming.Message.Text.ToLower() == "help")
                SendTextMessage(HelpMessage, incoming.Sender.Id);
            // get "metrics" when asked for
            else if (incoming.Message.Text.ToLower().Contains("metrics"))
                SendTextMessage(GetMetrics(), incoming.Sender.Id);
            // get some flight information
            else if (incoming.Message.Text.ToLower().Contains("flights"))
                ProcessFlightRequest(incoming.Message.Text, incoming.Sender.Id);
            // if message contains "twitter" then return a random suggestion of who to follow on twitter
            else if (incoming.Message.Text.ToLower().Contains("twitter"))
                SendTextMessage("I think you should follow https://twitter.com/" + TwitterSuggestions[Rand.Next(0, TwitterSuggestions.Count - 1)] + " on Twitter!", incoming.Sender.Id);
            // tag::snippet[]
            // if the message contains "hi", "hello", etc say "howdy"
            else if (HelloStrings.Any(incoming.Message.Text.ToLower().Contains))
                SendTextMessage("Howdy!", incoming.Sender.Id);
            // if message contains "?" then link to the forums
            else if (incoming.Message.Text.Contains("?"))
                SendTextMessage("If you have a Couchbase question, please ask on the forums! http://forums.couchbase.com", incoming.Sender.Id);
            else
                SendTextMessage("I'm sorry, I don't understand you. Type 'help' for help!", incoming.Sender.Id);
            // end::snippet[]
        }

        private void ProcessFlightRequest(string incomingMessage, string id)
        {
            /*
            // expecting messages that contain "{flight} from {airport} to {airport2}"
            //var sourceairport = Regex.Match(incomingMessage, "from (.*?)").Value;
            //var destinationairport = Regex.Match(incomingMessage, "to (.*?)").Value;
            var dayOfTheWeek = (int)DateTime.Now.DayOfWeek;

            var n1ql = @"SELECT COUNT(*) FROM (
                select r.sourceairport, r.destinationairport, a.name, s.flight, s.utc
                from `travel-sample` r
                unnest r.schedule s
                inner join `travel-sample` a ON KEYS r.airlineid
                where r.type = 'route'
                and r.sourceairport = 'CMH'
                and r.destinationairport = 'ATL'
                and s.day = 0
                order by s.utc
                ) flights;";
            var query = QueryRequest.Create(n1ql);
            query.AddNamedParameter("sourceairport", sourceairport);
            query.AddNamedParameter("destinationairport", destinationairport);
            query.AddNamedParameter("dayOfTheWeek", dayOfTheWeek);

            var result = _bucket.Query<int>(query);

            var count = result.Rows.First();

            var resultMessage = $"There are {count} flights from {sourceairport} to {destinationairport} on {DateTime.Now.DayOfWeek.ToString()}";
            SendTextMessage(resultMessage, id);
            */
        }

        // tag::GetMetrics[]
        private string GetMetrics()
        {
            var n1ql = @"select value count(*) as totalIncoming
                        from ViberChatBot b
                        where meta(b).id like 'incoming::%';";
            var query = QueryRequest.Create(n1ql);
            var response = _bucket.Query<int>(query);
            if (response.Success)
                return $"I have received {response.Rows.First()} incoming messages so far!";
            return "Sorry, I'm having trouble getting metrics right now.";
        }
        // end::GetMetrics[]

        // tag::SendTextMessage[]
        private void SendTextMessage(string message, string senderId)
        {
            var client = new RestClient("https://chatapi.viber.com/pa/send_message");
            var request = new RestRequest(RestSharp.Method.POST);
            request.AddJsonBody(new
            {
                receiver = senderId,    // receiver	(Unique Viber user id, required)
                type = "text",          // type	(Message type, required) Available message types: text, picture, etc
                text = message
            });
            request.AddHeader("X-Viber-Auth-Token", ViberKey);
            var response = client.Execute(request);

            // log to Couchbase
            _bucket.Insert("resp::" + Guid.NewGuid(), response.Content);
        }
        // end::SendTextMessage[]

        private static string HelpMessage =
            @"Welcome! Here are some things you can do: 
* Say 'hi'
* Get 'metrics' about me
* Ask a '?'
* Ask about 'flights from CMH to ATL' (or other airports)
* Ask for a 'twitter' recommendation.";

        private static readonly List<string> HelloStrings = new List<string> {
            "hi",
            "hello",
            "greetings",
            "howdy"
        };

        private static readonly List<string> TwitterSuggestions = new List<string>
        {
            "mgroves",
            "nraboy",
            "couchbase",
            "couchbasedev",
            "N1QL",
            "HodGreeley",
            "rajagp",
            "Czajkowski",
            "deniswsrosa"
        };
    }
}