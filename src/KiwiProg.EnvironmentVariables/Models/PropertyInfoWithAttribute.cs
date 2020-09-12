using System.Reflection;

namespace KiwiProg.EnvironmentVariables.Models
{
    internal class PropertyInfoWithAttribute
    {
        public PropertyInfo Info { get; set; }
        public EnvironmentVariableAttribute Attribute { get; set; }
    }
}
