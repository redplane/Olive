using System;
using System.Linq.Expressions;

namespace Shared.Helpers
{
    public class MethodHelper
    {
        private static MethodHelper _instance;

        public static MethodHelper Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new MethodHelper();

                return _instance;
            }
        }

        public string RetrievePropertyName<T>(Expression<Func<T>> propertyLambda)
        {
            var property = propertyLambda.Body as MemberExpression;

            if (property == null)
                throw new ArgumentException(
                    "You must pass a lambda of the form: '() => Class.Property' or '() => object.Property'");

            return property.Member.Name;
        }
    }
}