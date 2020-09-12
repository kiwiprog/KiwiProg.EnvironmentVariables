using System;
using System.Collections;
using FluentAssertions;
using Xunit;

namespace KiwiProg.EnvironmentVariables.Tests
{
    public class LoaderExtensionsTests
    {
        public LoaderExtensionsTests()
        {
            ClearEnvironmentVariables();
        }

        [EnvironmentVariable]
        public string EnvVarNormal { get; set; }

        [EnvironmentVariable("ENV_VARIABLE")]
        public string EnvVarWithDifferentName { get; set; }

        [EnvironmentVariable(defaultValue: "default value")]
        public string EnvVarWithDefaultValue { get; set; }

        [Fact]
        public void Loads_Environment_Variables_With_Matching_Name()
        {
            const string expectedValue = "expected value";

            Environment.SetEnvironmentVariable("EnvVarNormal", expectedValue);

            this.LoadEnvironmentVariables();

            EnvVarNormal.Should().Be(expectedValue);
        }

        [Fact]
        public void Loads_Environment_Variables_Using_Provided_Name()
        {
            const string expectedValue = "expected value";

            Environment.SetEnvironmentVariable("ENV_VARIABLE", expectedValue);
            Environment.SetEnvironmentVariable("EnvVarWithDifferentName", "unexpected value");

            this.LoadEnvironmentVariables();

            EnvVarWithDifferentName.Should().Be(expectedValue);
        }

        [Fact]
        public void Load_Default_Value_If_Environment_Variable_Not_Defined()
        {
            this.LoadEnvironmentVariables();

            EnvVarWithDefaultValue.Should().Be("default value");
        }

        [Fact]
        public void Not_Load_Undefined_Environment_Variables_With_No_Default()
        {
            this.LoadEnvironmentVariables();

            EnvVarNormal.Should().BeNull();
            EnvVarWithDifferentName.Should().BeNull();
        }

        private void ClearEnvironmentVariables()
        {
            foreach (DictionaryEntry variable in Environment.GetEnvironmentVariables())
            {
                Environment.SetEnvironmentVariable((string)variable.Key, null);
            }
        }
    }
}
