using System;
namespace BackendModelLibrary.Responses
{
    public class MessageResponseWrapper<T>
    {
        public required T Content { get; set; }
    }
}