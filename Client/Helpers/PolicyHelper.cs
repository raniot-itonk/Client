using System;
using Flurl.Http;
using Polly;
using Polly.Retry;
using Polly.Timeout;

namespace Client.Helpers
{
    public class PolicyHelper
    {
        public static AsyncTimeoutPolicy ThreeRetriesAsync()
        {
            return Policy.TimeoutAsync(TimeSpan.FromSeconds(120));
            //return Policy.Handle<FlurlHttpException>()
            //    .WaitAndRetryAsync(new[]
            //    {
            //        TimeSpan.FromSeconds(1),
            //        TimeSpan.FromSeconds(2),
            //        TimeSpan.FromSeconds(3),
            //    });
        }
    }
}
