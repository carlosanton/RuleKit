using System.ComponentModel.DataAnnotations;
using System.Numerics;
using RuleKit.Tests.Helpers;
using RuleKit.Tests.Models;

namespace RuleKit.Tests.Numbers
{
    public class MaxDigitsAttributeTests
    {
        /// <summary>
        /// Validates values that contain no more than five digits.
        /// </summary>
        /// <param name="value">Value to validate.</param>
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(123)]
        [InlineData(12345)]
        [InlineData(-1)]
        [InlineData(-12345)]
        public void Validate_Should_Return_Success_When_Value_Does_Not_Exceed_MaxDigits(long value)
        {
            var model = new MaxDigitsModel
            {
                Value = value
            };

            var validationResults = ValidationTestHelper.Validate(model);

            Assert.Empty(validationResults);
        }

        /// <summary>
        /// Validates values that contain more than five digits.
        /// </summary>
        /// <param name="value">Value to validate.</param>
        [Theory]
        [InlineData(123456)]
        [InlineData(-123456)]
        public void Validate_Should_Return_Error_When_Value_Exceeds_MaxDigits(long value)
        {
            var model = new MaxDigitsModel
            {
                Value = value
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
            var model = new MaxDigitsModel
            {
                Value = null
            };

            var validationResults = ValidationTestHelper.Validate(model);

            Assert.Empty(validationResults);
        }

        /// <summary>
        /// Validates every supported integer type.
        /// </summary>
        [Fact]
        public void Validate_Should_Return_Success_When_Integer_Type_Is_Supported()
        {
            var values = new object[]
            {
                (byte)123,
                (sbyte)-123,
                (short)-123,
                (ushort)123,
                -123,
                123U,
                -123L,
                123UL,
                (Int128)(-123),
                (UInt128)123,
                (nint)(-123),
                (nuint)123,
                new BigInteger(-123)
            };
            var attribute = new MaxDigitsAttribute(3);

            foreach (var value in values)
            {
                var validationResult = attribute.GetValidationResult(value, CreateValidationContext());

                Assert.Null(validationResult);
            }
        }

        /// <summary>
        /// Validates the minimum values of signed integer types without overflowing.
        /// </summary>
        [Fact]
        public void Validate_Should_Return_Success_When_Value_Is_Minimum_Signed_Integer()
        {
            var attribute = new MaxDigitsAttribute(39);
            var values = new object[]
            {
                sbyte.MinValue,
                short.MinValue,
                int.MinValue,
                long.MinValue,
                Int128.MinValue,
                nint.MinValue
            };

            foreach (var value in values)
            {
                var validationResult = attribute.GetValidationResult(value, CreateValidationContext());

                Assert.Null(validationResult);
            }
        }

        /// <summary>
        /// Validates that unsupported data types are treated as developer errors.
        /// </summary>
        [Fact]
        public void Validate_Should_Throw_When_Value_Type_Is_Not_Supported()
        {
            var attribute = new MaxDigitsAttribute(5);
            var unsupportedValues = new object[]
            {
                123.45M,
                123.45D,
                123.45F,
                "12345"
            };

            foreach (var value in unsupportedValues)
            {
                var exception = Assert.Throws<InvalidOperationException>(() => attribute.GetValidationResult(value, CreateValidationContext()));

                Assert.Contains(nameof(MaxDigitsAttribute), exception.Message);
                Assert.Contains(value.GetType().FullName!, exception.Message);
            }
        }

        /// <summary>
        /// Validates constructor arguments.
        /// </summary>
        /// <param name="maxDigits">The invalid maximum number of digits.</param>
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Constructor_Should_Throw_When_MaxDigits_Is_Not_Positive(int maxDigits)
        {
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new MaxDigitsAttribute(maxDigits));

            Assert.Equal("maxDigits", exception.ParamName);
        }

        /// <summary>
        /// Exposes the configured maximum number of digits.
        /// </summary>
        [Fact]
        public void Constructor_Should_Set_MaxDigits()
        {
            var attribute = new MaxDigitsAttribute(5);

            Assert.Equal(5, attribute.MaxDigits);
        }

        /// <summary>
        /// Validates the default error message and affected member name.
        /// </summary>
        [Fact]
        public void Validate_Should_Return_Default_Error_Message_When_Value_Is_Invalid()
        {
            var attribute = new MaxDigitsAttribute(3);

            var validationResult = attribute.GetValidationResult(1234, CreateValidationContext());

            Assert.NotNull(validationResult);
            Assert.Equal("The field Value cannot contain more than 3 digits.", validationResult.ErrorMessage);
            Assert.Equal(["Value"], validationResult.MemberNames);
        }

        /// <summary>
        /// Validates a custom localized error message.
        /// </summary>
        [Fact]
        public void Validate_Should_Return_Custom_Error_Message_When_Configured()
        {
            var attribute = new MaxDigitsAttribute(3)
            {
                ErrorMessage = "El campo {0} no puede contener más de {1} dígitos."
            };

            var validationResult = attribute.GetValidationResult(1234, CreateValidationContext());

            Assert.NotNull(validationResult);
            Assert.Equal("El campo Value no puede contener más de 3 dígitos.", validationResult.ErrorMessage);
        }

        /// <summary>
        /// Validates an error message provided by a resource type.
        /// </summary>
        [Fact]
        public void Validate_Should_Return_Resource_Error_Message_When_Configured()
        {
            var attribute = new MaxDigitsAttribute(3)
            {
                ErrorMessageResourceName = nameof(TestResources.MaxDigitsErrorMessage),
                ErrorMessageResourceType = typeof(TestResources)
            };

            var validationResult = attribute.GetValidationResult(1234, CreateValidationContext());

            Assert.NotNull(validationResult);
            Assert.Equal("Resource message for Value with a maximum of 3 digits.", validationResult.ErrorMessage);
        }

        private static ValidationContext CreateValidationContext()
        {
            return new ValidationContext(new object())
            {
                DisplayName = "Value",
                MemberName = "Value"
            };
        }

        private static class TestResources
        {
            public static string MaxDigitsErrorMessage => "Resource message for {0} with a maximum of {1} digits.";
        }
    }
}
