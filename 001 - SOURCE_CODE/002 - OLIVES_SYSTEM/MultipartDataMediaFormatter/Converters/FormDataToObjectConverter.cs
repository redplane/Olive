using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MultipartDataMediaFormatter.Infrastructure;
using MultipartDataMediaFormatter.Infrastructure.Extensions;

namespace MultipartDataMediaFormatter.Converters
{
    public class FormDataToObjectConverter
    {
        private readonly FormData _sourceData;

        public FormDataToObjectConverter(FormData sourceData)
        {
            if (sourceData == null)
                throw new NotImplementedException("sourceData is required");

            _sourceData = sourceData;
        }

        public object Convert(Type destinitionType)
        {
            if (destinitionType == null)
                throw new ArgumentNullException("destinitionType");

            if (destinitionType == typeof (FormData))
                return _sourceData;

            var objResult = CreateObject(destinitionType);
            return objResult;
        }

        private object CreateObject(Type type, string propertyName = "")
        {
            object buffer;
            if (TryGetFromFormData(type, propertyName, out buffer)
                || TryGetAsGenericDictionary(type, propertyName, out buffer)
                || TryGetAsGenericListOrArray(type, propertyName, out buffer)
                || TryGetAsCustomType(type, propertyName, out buffer))
                return buffer;

            throw new Exception($"Cannot parse type '{type.FullName}'.");
        }

        private bool TryGetFromFormData(Type destinitionType, string propertyName, out object propValue)
        {
            propValue = null;

            if (destinitionType == typeof (HttpFile))
            {
                HttpFile httpFile;
                if (!_sourceData.TryGetValue(propertyName, out httpFile)) return false;

                propValue = httpFile;

                return true;
            }

            string val;
            if (!_sourceData.TryGetValue(propertyName, out val)) return false;

            var typeConverter = destinitionType.GetFromStringConverter();
            if (typeConverter == null)
                throw new Exception($"Cannot find type converter for field - {propertyName}");

            propValue = typeConverter.ConvertFromString(null, CultureInfo.CurrentCulture, val);


            return true;
        }

        private bool TryGetAsGenericDictionary(Type destinitionType, string propertyName, out object propValue)
        {
            propValue = null;
            Type keyType, valueType;
            var isGenericDictionary = IsGenericDictionary(destinitionType, out keyType, out valueType);
            if (isGenericDictionary)
            {
                var dictType = typeof (Dictionary<,>).MakeGenericType(keyType, valueType);
                var add = dictType.GetMethod("Add");

                var pValue = Activator.CreateInstance(dictType);

                var index = 0;
                var origPropName = propertyName;
                var isFilled = false;
                while (true)
                {
                    string propertyKeyName = $"{origPropName}[{index}].Key";
                    var objKey = CreateObject(keyType, propertyKeyName);
                    if (objKey != null)
                    {
                        string propertyValueName = $"{origPropName}[{index}].Value";
                        var objValue = CreateObject(valueType, propertyValueName);

                        if (objValue != null)
                        {
                            add.Invoke(pValue, new[] {objKey, objValue});
                            isFilled = true;
                        }

                        index++;
                        continue;
                    }

                    break;
                }

                if (isFilled)
                {
                    propValue = pValue;
                }
            }

            return isGenericDictionary;
        }

        private bool TryGetAsGenericListOrArray(Type destinitionType, string propertyName, out object propValue)
        {
            propValue = null;
            Type genericListItemType;
            var isGenericList = IsGenericListOrArray(destinitionType, out genericListItemType);
            if (isGenericList)
            {
                var listType = typeof (List<>).MakeGenericType(genericListItemType);

                var add = listType.GetMethod("Add");
                var pValue = Activator.CreateInstance(listType);

                var index = 0;
                var origPropName = propertyName;
                var isFilled = false;
                while (true)
                {
                    propertyName = $"{origPropName}[{index}]";
                    var objValue = CreateObject(genericListItemType, propertyName);
                    if (objValue != null)
                    {
                        add.Invoke(pValue, new[] {objValue});
                        isFilled = true;
                    }
                    else
                    {
                        break;
                    }

                    index++;
                }

                if (isFilled)
                {
                    if (destinitionType.IsArray)
                    {
                        var toArrayMethod = listType.GetMethod("ToArray");
                        propValue = toArrayMethod.Invoke(pValue, new object[0]);
                    }
                    else
                    {
                        propValue = pValue;
                    }
                }
            }

            return isGenericList;
        }

        private bool TryGetAsCustomType(Type destinitionType, string propertyName, out object propValue)
        {
            propValue = null;
            var isCustomNonEnumerableType = destinitionType.IsCustomNonEnumerableType();
            if (isCustomNonEnumerableType)
            {
                if (string.IsNullOrWhiteSpace(propertyName)
                    ||
                    _sourceData.AllKeys()
                        .Any(m => m.StartsWith(propertyName + ".", StringComparison.CurrentCultureIgnoreCase)))
                {
                    var obj = Activator.CreateInstance(destinitionType);
                    var isFilled = false;
                    foreach (var propertyInfo in destinitionType.GetPublicAccessibleProperties())
                    {
                        var propName = (!string.IsNullOrEmpty(propertyName) ? propertyName + "." : "") +
                                       propertyInfo.Name;
                        var objValue = CreateObject(propertyInfo.PropertyType, propName);
                        if (objValue != null)
                        {
                            propertyInfo.SetValue(obj, objValue);
                            isFilled = true;
                        }
                    }
                    if (isFilled)
                    {
                        propValue = obj;
                    }
                }
            }
            return isCustomNonEnumerableType;
        }


        private bool IsGenericDictionary(Type type, out Type keyType, out Type valueType)
        {
            var iDictType = type.GetInterface(typeof (IDictionary<,>).Name);
            var types = iDictType?.GetGenericArguments();
            if (types?.Length == 2)
            {
                keyType = types[0];
                valueType = types[1];
                return true;
            }

            keyType = null;
            valueType = null;
            return false;
        }

        private bool IsGenericListOrArray(Type type, out Type itemType)
        {
            if (type.GetInterface(typeof (IDictionary<,>).Name) == null) //not a dictionary
            {
                if (type.IsArray)
                {
                    itemType = type.GetElementType();
                    return true;
                }

                var iListType = type.GetInterface(typeof (ICollection<>).Name);
                var genericArguments = iListType?.GetGenericArguments();
                if (genericArguments?.Length == 1)
                {
                    itemType = genericArguments[0];
                    return true;
                }
            }

            itemType = null;
            return false;
        }
    }
}