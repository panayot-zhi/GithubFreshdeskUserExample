using System.Net;

namespace GithubFreshdeskUserExampleUnitTests
{
    public class MockHttpMessageHandler : DelegatingHandler
    {
        public Queue<HttpResponseMessage> ResponsesQueue { get; } = new();

        public Stack<HttpRequestMessage> RequestsQueue { get; } = new();

        public int NumberOfCalls { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            NumberOfCalls++;

            RequestsQueue.Push(request);

            return await Task.FromResult(ResponsesQueue.Count > 0 ? ResponsesQueue.Dequeue() : new HttpResponseMessage(HttpStatusCode.NotFound));

        }
    }

}
