using System.ComponentModel.DataAnnotations;
using System.Globalization;
using RuleKit.Tests.Helpers;
using RuleKit.Tests.Models;

namespace RuleKit.Tests.Numbers
{
    public class DecimalDigitsAttributeTests
    {
        /// <summary>
        /// Validates values within the configured integer and decimal digit limits.
        /// </summary>
        /// <param name="value">The value to validate using invariant culture.</param>
        [Theory]
        [InlineData("0")]
        [InlineData("0.00")]
        [InlineData("1")]
        [InlineData("123")]
        [InlineData("123.45")]
        [InlineData("-123.45")]
        [InlineData("0.12")]
        [InlineData("-0.12")]
        public void Validate_Should_Return_Success_When_Value_Is_Within_Limits(string value)
        {
            var model = new DecimalDigitsModel
            {
                Value = decimal.Parse(value, CultureInfo.InvariantCulture)
            };

            var validationResults = ValidationTestHelper.Validate(model);

            Assert.Empty(validationResults);
        }

        /// <summary>
        /// Validates values that exceed the configured number of integer digits.
        /// </summary>
        /// <param name="value">The value to validate using invariant culture.</param>
        [Theory]
        [InlineData("1234")]
        [InlineData("-1234")]
        [InlineData("1234.12")]
        public void Validate_Should_Return_Error_When_Integer_Digits_Exceed_Limit(string value)
        {
            var model = new DecimalDigitsModel
            {
                Value = decimal.Parse(value, CultureInfo.InvariantCulture)
            };

            var validationResults = ValidationTestHelper.Validate(model);

            Assert.Single(validationResults);
        }

        /// <summary>
        /// Validates values that exceed the configured number of decimal digits.
        /// </summary>
        /// <param name="value">The value to validate using invariant culture.</param>
        [Theory]
        [InlineData("1.2300")]
        [InlineData("1.2323")]
        [InlineData("123.450")]
        [InlineData("-0.123")]
        public void Validate_Should_Return_Error_When_Decimal_Digits_Exceed_Limit(string value)
        {
            var model = new DecimalDigitsModel
            {
                Value = decimal.Parse(value, CultureInfo.InvariantCulture)
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
            var model = new DecimalDigitsModel
            {
                Value = null
            };

            var validationResults = ValidationTestHelper.Validate(model);

            Assert.Empty(validationResults);
        }

        /// <summary>
        /// Validates that zero decimal digits can be configured.
        /// </summary>
        [Fact]
        public void Validate_Should_Respect_Zero_Decimal_Digits()
        {
            var attribute = new DecimalDigitsAttribute(3, 0);

            var integerResult = attribute.GetValidationResult(123M, CreateValidationContext());
            var decimalResult = attribute.GetValidationResult(123.0M, CreateValidationContext());

            Assert.Null(integerResult);
            Assert.NotNull(decimalResult);
        }

        /// <summary>
        /// Validates the largest negative decimal without overflowing.
        /// </summary>
        [Fact]
        public void Validate_Should_Return_Success_When_Value_Is_Decimal_MinValue()
        {
            var attribute = new DecimalDigitsAttribute(29, 0);

            var validationResult = attribute.GetValidationResult(decimal.MinValue, CreateValidationContext());

            Assert.Null(validationResult);
        }

        /// <summary>
        /// Validates that unsupported data types are treated as developer errors.
        /// </summary>
        [Fact]
        public void Validate_Should_Throw_When_Value_Type_Is_Not_Supported()
        {
            var attribute = new DecimalDigitsAttribute(3, 2);
            var unsupportedValues = new object[]
            {
                123,
                123.45D,
                123.45F,
                "123.45"
            };

            foreach (var value in unsupportedValues)
            {
                var exception = Assert.Throws<InvalidOperationException>(() => attribute.GetValidationResult(value, CreateValidationContext()));

                Assert.Contains(nameof(DecimalDigitsAttribute), exception.Message);
                Assert.Contains(value.GetType().FullName!, exception.Message);
            }
        }

        /// <summary>
        /// Validates invalid maximum integer digit arguments.
        /// </summary>
        /// <param name="maxIntegerDigits">The invalid maximum number of integer digits.</param>
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Constructor_Should_Throw_When_MaxIntegerDigits_Is_Not_Positive(int maxIntegerDigits)
        {
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new DecimalDigitsAttribute(maxIntegerDigits, 2));

            Assert.Equal("maxIntegerDigits", exception.ParamName);
        }

        /// <summary>
        /// Validates invalid maximum decimal digit arguments.
        /// </summary>
        [Fact]
        public void Constructor_Should_Throw_When_MaxDecimalDigits_Is_Negative()
        {
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new DecimalDigitsAttribute(3, -1));

            Assert.Equal("maxDecimalDigits", exception.ParamName);
        }

        /// <summary>
        /// Exposes the configured integer and decimal digit limits.
        /// </summary>
        [Fact]
        public void Constructor_Should_Set_Digit_Limits()
        {
            var attribute = new DecimalDigitsAttribute(3, 2);

            Assert.Equal(3, attribute.MaxIntegerDigits);
            Assert.Equal(2, attribute.MaxDecimalDigits);
        }

        /// <summary>
        /// Validates the default error message and affected member name.
        /// </summary>
        [Fact]
        public void Validate_Should_Return_Default_Error_Message_When_Value_Is_Invalid()
        {
            var attribute = new DecimalDigitsAttribute(3, 2);

            var validationResult = attribute.GetValidationResult(1234.567M, CreateValidationContext());

            Assert.NotNull(validationResult);
            Assert.Equal("The field Value must not exceed 3 integer digits and 2 decimal digits.", validationResult.ErrorMessage);
            Assert.Equal(["Value"], validationResult.MemberNames);
        }

        /// <summary>
        /// Validates a custom localized error message.
        /// </summary>
        [Fact]
        public void Validate_Should_Return_Custom_Error_Message_When_Configured()
        {
            var attribute = new DecimalDigitsAttribute(3, 2)
            {
                ErrorMessage = "El campo {0} admite {1} enteros y {2} decimales."
            };

            var validationResult = attribute.GetValidationResult(1234.567M, CreateValidationContext());

            Assert.NotNull(validationResult);
            Assert.Equal("El campo Value admite 3 enteros y 2 decimales.", validationResult.ErrorMessage);
        }

        /// <summary>
        /// Validates an error message provided by a resource type.
        /// </summary>
        [Fact]
        public void Validate_Should_Return_Resource_Error_Message_When_Configured()
        {
            var attribute = new DecimalDigitsAttribute(3, 2)
            {
                ErrorMessageResourceName = nameof(TestResources.DecimalDigitsErrorMessage),
                ErrorMessageResourceType = typeof(TestResources)
            };

            var validationResult = attribute.GetValidationResult(1234.567M, CreateValidationContext());

            Assert.NotNull(validationResult);
            Assert.Equal("Resource message for Value with 3 integer and 2 decimal digits.", validationResult.ErrorMessage);
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
            public static string DecimalDigitsErrorMessage => "Resource message for {0} with {1} integer and {2} decimal digits.";
        }
    }
}
