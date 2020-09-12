using System;
namespace KiwiProg.EnvironmentVariables
{
    public class EnvironmentVariableAttribute : Attribute
    {
        public string Name { get; internal set; }
        public string DefaultValue { get; internal set; }

        public EnvironmentVariableAttribute(string name = null, string defaultValue = null)
        {
            Name = name;
            DefaultValue = defaultValue;
        }
    }
}
