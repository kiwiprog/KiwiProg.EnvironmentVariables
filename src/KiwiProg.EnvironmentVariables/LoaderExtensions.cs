using System;
using System.Collections.Generic;
using System.Linq;
using KiwiProg.EnvironmentVariables.Models;

namespace KiwiProg.EnvironmentVariables
{
    public static class LoaderExtensions
    {
        public static void LoadEnvironmentVariables(this object obj)
        {
            var propertiesWithEnvVarAttribute = obj.ExtractPropertiesWithEnvVarAttribute();

            foreach (var property in propertiesWithEnvVarAttribute)
            {
                var environmentVariableName = property.Attribute.Name
                    ?? property.Info.Name;

                var environmentVariableValue =
                    Environment.GetEnvironmentVariable(environmentVariableName)
                    ?? property.Attribute.DefaultValue;

                property.Info.SetValue(obj, environmentVariableValue);
            }
        }

        internal static IEnumerable<PropertyInfoWithAttribute> ExtractPropertiesWithEnvVarAttribute(
            this object obj)
        {
            var objType = obj.GetType();
            var objProperties = objType.GetProperties();

            return objProperties
                .SelectMany(
                    propertyInfo => propertyInfo.GetCustomAttributes(true),
                    (propertyInfo, attribute) => new PropertyInfoWithAttribute
                    {
                        Info = propertyInfo,
                        Attribute = attribute as EnvironmentVariableAttribute
                    })
                .Where(p => p.Attribute != null);
        }
    }
}
