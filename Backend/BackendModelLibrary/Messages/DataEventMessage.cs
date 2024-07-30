using BackendModelLibrary.Model.Enums;

namespace BackendModelLibrary.Messages
{
    public class DataEventMessage<T> where T : class
    {
        public DataEventOperationType Operation { get; set; }
        public T? Data { get; set; }
    }
}


content-type: User
{
    "Operation": "Add",
    "Data": .....
}