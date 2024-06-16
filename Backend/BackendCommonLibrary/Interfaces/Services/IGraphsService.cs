using System;
using System.Threading.Tasks;
using BackendModelLibrary.Model;
using BackendModelLibrary.Requests;
using BackendModelLibrary.Responses;

namespace BackendCommonLibrary.Interfaces.Services
{
    public interface IGraphsService
    {
        Task<GraphResponseWrapper> GetGraph(GraphRequestWrapper request);
    }
}