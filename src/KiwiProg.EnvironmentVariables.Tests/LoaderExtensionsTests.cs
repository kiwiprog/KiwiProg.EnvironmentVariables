using System;
using System.Collections;
using System.IO;
using FluentAssertions;
using Xunit;

namespace KiwiProg.EnvironmentVariables.Tests
{
    public class LoaderExtensionsTests
    {
        private const string DotEnvFileName = ".env";
        private const string BackupDotEnvFileName = "copy.env";

        public LoaderExtensionsTests()
        {
            DeleteBackupDotEnv();
            ClearEnvironmentVariables();
        }

        [EnvironmentVariable]
        public string EnvVarNormal { get; set; }

        [EnvironmentVariable("ENV_VARIABLE")]
        public string EnvVarWithDifferentName { get; set; }

        [EnvironmentVariable(defaultValue: "default value")]
        public string EnvVarWithDefaultValue { get; set; }

        [EnvironmentVariable]
        public string EnvFileVariable { get; set; }

        [EnvironmentVariable("OTHER_ENV_FILE_VARIABLE")]
        public string OtherEnvFileVariable { get; set; }

        [EnvironmentVariable("ENV_FILE_VAR_WITH_SPACES")]
        public string EnvFileVarWithSpaces { get; set; }

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
        public void Loads_Default_Value_If_Environment_Variable_Not_Defined()
        {
            this.LoadEnvironmentVariables();

            EnvVarWithDefaultValue.Should().Be("default value");
        }

        [Fact]
        public void Does_Not_Load_Undefined_Environment_Variables_With_No_Default()
        {
            this.LoadEnvironmentVariables();

            EnvVarNormal.Should().BeNull();
            EnvVarWithDifferentName.Should().BeNull();
        }

        [Fact]
        public void Does_Nothing_For_Classes_Without_Any_Attributes()
        {
            var classWithoutEnvironmentVariables = new ClassWithoutEnvironmentVariables();

            classWithoutEnvironmentVariables.LoadEnvironmentVariables();

            classWithoutEnvironmentVariables.TestProperty.Should().BeNull();
        }

        [Fact]
        public void Does_Not_Load_Env_File_If_Not_Exists()
        {
            File.Copy(".env", "copy.env");
            File.Delete(".env");

            this.LoadEnvironmentVariables();

            EnvFileVariable.Should().BeNull();
            OtherEnvFileVariable.Should().BeNull();
            EnvFileVarWithSpaces.Should().BeNull();

            File.Move("copy.env", ".env");
        }

        [Fact]
        public void Loads_DotEnv_File_Variables()
        {
            this.LoadEnvironmentVariables();

            EnvFileVariable.Should().Be("This is an environment variable value");
            OtherEnvFileVariable.Should().Be("ThisIsAnotherVariable");
            EnvFileVarWithSpaces.Should().Be("This is a spaced environment variable");
        }

        [Fact]
        public void Does_Not_Override_Existing_Environment_Variables_With_DotEnv_Variables()
        {
            const string expectedValue1 = "expected value 1";
            const string expectedValue2 = "expected value 2";

            Environment.SetEnvironmentVariable("EnvFileVariable", expectedValue1);
            Environment.SetEnvironmentVariable("OTHER_ENV_FILE_VARIABLE", expectedValue2);

            this.LoadEnvironmentVariables();

            EnvFileVariable.Should().Be(expectedValue1);
            OtherEnvFileVariable.Should().Be(expectedValue2);
            EnvFileVarWithSpaces.Should().Be("This is a spaced environment variable");
        }

        private void DeleteBackupDotEnv()
        {
            if (File.Exists(BackupDotEnvFileName))
            {
                File.Delete(BackupDotEnvFileName);
            }
        }

        private void ClearEnvironmentVariables()
        {
            foreach (DictionaryEntry variable in Environment.GetEnvironmentVariables())
            {
                Environment.SetEnvironmentVariable((string)variable.Key, null);
            }
        }
    }

    public class ClassWithoutEnvironmentVariables
    {
        public string TestProperty { get; set; }
    }
}
