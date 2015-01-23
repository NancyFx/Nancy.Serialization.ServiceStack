﻿namespace Nancy.Serializers.Json.ServiceStack
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using ModelBinding;

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

            var properties = context.DestinationType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(p => new BindingMemberInfo(p));

            var fields = context.DestinationType
                .GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Select(f => new BindingMemberInfo(f));

            if (properties.Concat(fields).Except(context.ValidModelBindingMembers).Any())
            {
                return CreateObjectWithBlacklistExcluded(context, deserializedObject);
            }

            return deserializedObject;
        }

        private object CreateObjectWithBlacklistExcluded(BindingContext context, object deserializedObject)
        {
            var returnObject = Activator.CreateInstance(context.DestinationType);

            foreach (var member in context.ValidModelBindingMembers)
            {
                CopyPropertyValue(member, deserializedObject, returnObject);
            }

            return returnObject;
        }

        private static void CopyPropertyValue(BindingMemberInfo member, object sourceObject, object destinationObject)
        {
            member.SetValue(destinationObject, member.GetValue(sourceObject));
        }
    }
}
