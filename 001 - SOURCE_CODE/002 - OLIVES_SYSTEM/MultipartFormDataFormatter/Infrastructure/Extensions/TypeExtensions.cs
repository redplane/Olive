using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using MultipartFormDataMediaFormatter.Models;

namespace MultipartFormDataMediaFormatter.Infrastructure.Extensions
{
    internal static class TypeExtensions
    {
        internal static TypeConverter GetFromStringConverter(this Type type)
        {
            var typeConverter = TypeDescriptor.GetConverter(type);
            if (!typeConverter.CanConvertFrom(typeof (string)))
            {
                typeConverter = null;
            }
            return typeConverter;
        }

        internal static TypeConverter GetToStringConverter(this Type type)
        {
            var typeConverter = TypeDescriptor.GetConverter(type);
            if (typeConverter is DateTimeConverter)
            {
                //replace default datetime converter for serializing datetime in ISO 8601 format
                typeConverter = new DateTimeConverterIso8601();
            }
            if (!typeConverter.CanConvertTo(typeof (string)))
            {
                typeConverter = null;
            }
            return typeConverter;
        }

        internal static IEnumerable<PropertyInfo> GetPublicAccessibleProperties(this Type type)
        {
            foreach (var propertyInfo in type.GetProperties())
            {
                if (!propertyInfo.CanRead || !propertyInfo.CanWrite || propertyInfo.SetMethod == null ||
                    propertyInfo.SetMethod.IsPrivate)
                    continue;
                yield return propertyInfo;
            }
        }

        internal static bool IsCustomNonEnumerableType(this Type type)
        {
            var nullType = Nullable.GetUnderlyingType(type);
            if (nullType != null)
            {
                type = nullType;
            }
            if (type.IsGenericType)
            {
                type = type.GetGenericTypeDefinition();
            }
            return type != typeof (object)
                   && Type.GetTypeCode(type) == TypeCode.Object
                   && type != typeof (HttpFileModel)
                   && type != typeof (HttpImageModel)
                   && type != typeof (Guid)
                   && type.GetInterface(typeof (IEnumerable).Name) == null;
        }
    }
}