using System;
using System.Threading.Tasks;
using ModelLibrary.Model;
using ModelLibrary.Requests;
using ModelLibrary.Responses;

namespace BackendCommonLibrary.Interfaces.Services
{
    public interface IGraphsService
    {
        Task<GraphResponseWrapper> GetGraph(GraphRequestWrapper request);
    }
}