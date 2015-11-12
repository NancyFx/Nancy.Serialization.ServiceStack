namespace Nancy.Serialization.ServiceStack
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using global::ServiceStack.Text;
    using ModelBinding;
    using Responses.Negotiation;

    public class ServiceStackBodyDeserializer : IBodyDeserializer
    {
        /// <summary>
        /// Whether the deserializer can deserialize the content type
        /// </summary>
        /// <param name="mediaRange">Content type to deserialize</param>
        /// <param name="context">Current <see cref="BindingContext"/>.</param>
        /// <returns>True if supported, false otherwise</returns>
        public bool CanDeserialize(MediaRange mediaRange, BindingContext context)
        {
            return Helpers.IsJsonType(mediaRange);
        }

        /// <summary>
        /// Deserialize the request body to a model
        /// </summary>
        /// <param name="mediaRange">Content type to deserialize</param>
        /// <param name="bodyStream">Request body stream</param>
        /// <param name="context">Current context</param>
        /// <returns>Model instance</returns>
        public object Deserialize(MediaRange mediaRange, Stream bodyStream, BindingContext context)
        {
            var deserializedObject = JsonSerializer.DeserializeFromStream(context.DestinationType, bodyStream);
            if (deserializedObject == null)
            {
                return null;
            }

            IEnumerable<BindingMemberInfo> properties;
            IEnumerable<BindingMemberInfo> fields;

            if (context.DestinationType.IsGenericType())
            {
                var genericType = context.DestinationType.GetGenericArguments().FirstOrDefault();

                properties = genericType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(p => new BindingMemberInfo(p));
                fields = genericType.GetFields(BindingFlags.Public | BindingFlags.Instance).Select(p => new BindingMemberInfo(p));
            }
            else
            {
                properties = context.DestinationType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(p => new BindingMemberInfo(p));
                fields = context.DestinationType.GetFields(BindingFlags.Public | BindingFlags.Instance) .Select(p => new BindingMemberInfo(p));
            }

            return properties.Concat(fields).Except(context.ValidModelBindingMembers).Any()
                ? CreateObjectWithBlacklistExcluded(context, deserializedObject)
                : deserializedObject;
        }

        private static object CreateObjectWithBlacklistExcluded(BindingContext context, object deserializedObject)
        {
            var returnObject = Activator.CreateInstance(context.DestinationType, true);

            foreach (var property in context.ValidModelBindingMembers)
            {
                CopyPropertyValue(property, deserializedObject, returnObject);
            }

            return returnObject;
        }

        private static void CopyPropertyValue(BindingMemberInfo property, object sourceObject, object destinationObject)
        {
            property.SetValue(destinationObject, property.GetValue(sourceObject));
        }
    }
}
