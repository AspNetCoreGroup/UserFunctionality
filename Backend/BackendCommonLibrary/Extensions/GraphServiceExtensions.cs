using BackendCommonLibrary.Interfaces.Services;
using ModelLibrary.Requests;
using ModelLibrary.Responses;

namespace CommonLibrary.Extensions
{
    public static class GraphServiceExtensions
    {
        public static Task<GraphResponseWrapper> GetGraph(this IGraphsService graphService, DateTime minDateTime, DateTime maxDateTime, int networkID)
        {
            var request = new GraphRequestWrapper()
            {
                MinDateTime = minDateTime,
                MaxDateTime = maxDateTime,
                NetworkID = networkID
            };

            return graphService.GetGraph(request);
        }

        public static Task<GraphResponseWrapper> GetGraph(this IGraphsService graphService, DateTime minDateTime, DateTime maxDateTime, int networkID, int deviceID)
        {
            var request = new GraphRequestWrapper()
            {
                MinDateTime = minDateTime,
                MaxDateTime = maxDateTime,
                NetworkID = networkID,
                DeviceID = deviceID
            };

            return graphService.GetGraph(request);
        }
    }
}