namespace Nancy.Serializers.Json.ServiceStack
{
    using System;
    using System.IO;

    using global::ServiceStack.Text;

    public class ServiceStackJsonSerializer : ISerializer
    {
        public bool CanSerialize(string contentType)
        {
            return Helpers.IsJsonType(contentType);
        }

        public void Serialize<TModel>(string contentType, TModel model, Stream outputStream)
        {
            JsonSerializer.SerializeToStream(model, outputStream);
        }
    }
}