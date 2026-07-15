using System.ComponentModel.DataAnnotations;
using RuleKit.Tests.Helpers;
using RuleKit.Tests.Models;

namespace RuleKit.Tests.Common
{
    public class NotDefaultAttributeTests
    {
        /// <summary>
        /// Validates a non-default value through standard object validation.
        /// </summary>
        [Fact]
        public void Validate_Should_Return_Success_When_Value_Is_Not_Default()
        {
            var model = new NotDefaultModel
            {
                Value = Guid.NewGuid()
            };

            var validationResults = ValidationTestHelper.Validate(model);

            Assert.Empty(validationResults);
        }

        /// <summary>
        /// Validates a default value through standard object validation.
        /// </summary>
        [Fact]
        public void Validate_Should_Return_Error_When_Value_Is_Default()
        {
            var model = new NotDefaultModel
            {
                Value = Guid.Empty
            };

            var validationResults = ValidationTestHelper.Validate(model);

            Assert.Single(validationResults);
        }

        /// <summary>
        /// Validates a null value.
        /// </summary>
        [Fact]
        public void Validate_Should_Return_Success_When_Value_Is_Null()
        {
            var model = new NotDefaultModel
            {
                Value = null
            };

            var validationResults = ValidationTestHelper.Validate(model);

            Assert.Empty(validationResults);
        }

        /// <summary>
        /// Validates default values from representative value types.
        /// </summary>
        [Fact]
        public void Validate_Should_Return_Error_For_Default_Value_Types()
        {
            var values = new object[]
            {
                (byte)0,
                0,
                0L,
                0M,
                '\0',
                Guid.Empty,
                default(DateTime),
                DateOnly.MinValue,
                TimeOnly.MinValue,
                default(TestStatus),
                default(CustomValue)
            };
            var attribute = new NotDefaultAttribute();

            foreach (var value in values)
            {
                var validationResult = attribute.GetValidationResult(value, CreateValidationContext());

                Assert.NotNull(validationResult);
            }
        }

        /// <summary>
        /// Validates non-default values from representative value types.
        /// </summary>
        [Fact]
        public void Validate_Should_Return_Success_For_Non_Default_Value_Types()
        {
            var values = new object[]
            {
                (byte)1,
                1,
                1L,
                0.1M,
                'A',
                Guid.NewGuid(),
                DateTime.UnixEpoch,
                new DateOnly(2000, 1, 1),
                new TimeOnly(1, 0),
                TestStatus.Active,
                new CustomValue()
            };
            var attribute = new NotDefaultAttribute();

            foreach (var value in values)
            {
                var validationResult = attribute.GetValidationResult(value, CreateValidationContext());

                Assert.Null(validationResult);
            }
        }

        /// <summary>
        /// Validates that default values do not invoke a custom parameterless struct constructor.
        /// </summary>
        [Fact]
        public void Validate_Should_Compare_With_Zero_Initialized_Value_When_Struct_Has_Parameterless_Constructor()
        {
            var attribute = new NotDefaultAttribute();

            var defaultResult = attribute.GetValidationResult(default(CustomValue), CreateValidationContext());
            var constructedResult = attribute.GetValidationResult(new CustomValue(), CreateValidationContext());

            Assert.NotNull(defaultResult);
            Assert.Null(constructedResult);
        }

        /// <summary>
        /// Validates that unsupported types are treated as developer errors.
        /// </summary>
        [Fact]
        public void Validate_Should_Throw_When_Value_Type_Is_Not_Supported()
        {
            var attribute = new NotDefaultAttribute();
            var unsupportedValues = new object[]
            {
                false,
                true,
                "value",
                new object()
            };

            foreach (var value in unsupportedValues)
            {
                var exception = Assert.Throws<InvalidOperationException>(() => attribute.GetValidationResult(value, CreateValidationContext()));

                Assert.Contains(nameof(NotDefaultAttribute), exception.Message);
                Assert.Contains(value.GetType().FullName!, exception.Message);
            }
        }

        /// <summary>
        /// Validates the default error message and affected member name.
        /// </summary>
        [Fact]
        public void Validate_Should_Return_Default_Error_Message_When_Value_Is_Invalid()
        {
            var attribute = new NotDefaultAttribute();

            var validationResult = attribute.GetValidationResult(Guid.Empty, CreateValidationContext());

            Assert.NotNull(validationResult);
            Assert.Equal("The field Value must not have its default value.", validationResult.ErrorMessage);
            Assert.Equal(["Value"], validationResult.MemberNames);
        }

        /// <summary>
        /// Validates a custom localized error message.
        /// </summary>
        [Fact]
        public void Validate_Should_Return_Custom_Error_Message_When_Configured()
        {
            var attribute = new NotDefaultAttribute
            {
                ErrorMessage = "El campo {0} no puede contener su valor predeterminado."
            };

            var validationResult = attribute.GetValidationResult(Guid.Empty, CreateValidationContext());

            Assert.NotNull(validationResult);
            Assert.Equal("El campo Value no puede contener su valor predeterminado.", validationResult.ErrorMessage);
        }

        /// <summary>
        /// Validates an error message provided by a resource type.
        /// </summary>
        [Fact]
        public void Validate_Should_Return_Resource_Error_Message_When_Configured()
        {
            var attribute = new NotDefaultAttribute
            {
                ErrorMessageResourceName = nameof(TestResources.NotDefaultErrorMessage),
                ErrorMessageResourceType = typeof(TestResources)
            };

            var validationResult = attribute.GetValidationResult(Guid.Empty, CreateValidationContext());

            Assert.NotNull(validationResult);
            Assert.Equal("Resource message for Value.", validationResult.ErrorMessage);
        }

        private static ValidationContext CreateValidationContext()
        {
            return new ValidationContext(new object())
            {
                DisplayName = "Value",
                MemberName = "Value"
            };
        }

        private enum TestStatus
        {
            None,
            Active
        }

        private readonly struct CustomValue
        {
            public CustomValue()
            {
                Value = 1;
            }

            public int Value { get; }
        }

        private static class TestResources
        {
            public static string NotDefaultErrorMessage => "Resource message for {0}.";
        }
    }
}
