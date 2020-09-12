using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KiwiProg.EnvironmentVariables.Models;

namespace KiwiProg.EnvironmentVariables
{
    public static class LoaderExtensions
    {
        public static void LoadEnvironmentVariables(this object obj)
        {
            InjectDotEnvFileIntoEnvironment();

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

        private static void InjectDotEnvFileIntoEnvironment()
        {
            if (!File.Exists(".env"))
            {
                return;
            }

            var dotEnvFileLines = File.ReadAllLines(".env");

            foreach (var envVarDefinition in dotEnvFileLines)
            {
                var blankLine = string.IsNullOrWhiteSpace(envVarDefinition);
                var commentLine = envVarDefinition.TrimStart().StartsWith('#');

                if (blankLine || commentLine)
                {
                    continue;
                }

                var separatorIndex = envVarDefinition.IndexOf('=');
                var envVarName = envVarDefinition.Substring(0, separatorIndex).Trim();
                var envVarValue = envVarDefinition.Substring(separatorIndex + 1).Trim();

                var variableNotInEnvironment = Environment.GetEnvironmentVariable(envVarName) is null;

                if (variableNotInEnvironment)
                {
                    Environment.SetEnvironmentVariable(envVarName, envVarValue);
                }
            }
        }

        private static IEnumerable<PropertyInfoWithAttribute> ExtractPropertiesWithEnvVarAttribute(
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
