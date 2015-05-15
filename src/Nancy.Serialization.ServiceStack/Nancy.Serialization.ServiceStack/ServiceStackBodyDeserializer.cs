namespace Nancy.Serializers.Json.ServiceStack
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Nancy.ModelBinding;

    using global::ServiceStack.Text;

    public class ServiceStackBodyDeserializer : IBodyDeserializer
    {
        /// <summary>
        /// Whether the deserializer can deserialize the content type
        /// </summary>
        /// <param name="contentType">Content type to deserialize</param>
        /// <param name="context">Current <see cref="BindingContext"/>.</param>
        /// <returns>True if supported, false otherwise</returns>
        public bool CanDeserialize(string contentType, BindingContext context)
        {
            return Helpers.IsJsonType(contentType);
        }

        /// <summary>
        /// Deserialize the request body to a model
        /// </summary>
        /// <param name="contentType">Content type to deserialize</param>
        /// <param name="bodyStream">Request body stream</param>
        /// <param name="context">Current context</param>
        /// <returns>Model instance</returns>
        public object Deserialize(string contentType, Stream bodyStream, BindingContext context)
        {
            var deserializedObject = JsonSerializer.DeserializeFromStream(context.DestinationType, bodyStream);
            if (deserializedObject == null)
            {
                return null;
            }

            IEnumerable<BindingMemberInfo> properties = Enumerable.Empty<BindingMemberInfo>();
            IEnumerable<BindingMemberInfo> fields = Enumerable.Empty<BindingMemberInfo>();

            if (context.DestinationType.IsGenericType())
            {
                var genericType = context.DestinationType.GetGenericArguments().FirstOrDefault();

                properties =
                    genericType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Select(p => new BindingMemberInfo(p));

                fields =
                    genericType.GetFields(BindingFlags.Public | BindingFlags.Instance)
                        .Select(p => new BindingMemberInfo(p));
            }
            else
            {
                properties =
                    context.DestinationType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Select(p => new BindingMemberInfo(p));

                fields =
                   context.DestinationType.GetFields(BindingFlags.Public | BindingFlags.Instance)
                       .Select(p => new BindingMemberInfo(p));
            }


            if (properties.Concat(fields).Except(context.ValidModelBindingMembers).Any())
            {
                return this.CreateObjectWithBlacklistExcluded(context, deserializedObject);
            }

            return deserializedObject;
        }

        private object CreateObjectWithBlacklistExcluded(BindingContext context, object deserializedObject)
        {
            var returnObject = Activator.CreateInstance(context.DestinationType, true);

            foreach (var property in context.ValidModelBindingMembers)
            {
                this.CopyPropertyValue(property, deserializedObject, returnObject);
            }

            return returnObject;
        }

        private void CopyPropertyValue(BindingMemberInfo property, object sourceObject, object destinationObject)
        {
            property.SetValue(destinationObject, property.GetValue(sourceObject));
        }
    }
}
