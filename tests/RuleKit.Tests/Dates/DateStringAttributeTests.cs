using System.ComponentModel.DataAnnotations;
using RuleKit.Tests.Helpers;
using RuleKit.Tests.Models;

namespace RuleKit.Tests.Dates
{
    public class DateStringAttributeTests
    {
        /// <summary>
        /// Validates real dates written in the configured format.
        /// </summary>
        /// <param name="value">The date string to validate.</param>
        [Theory]
        [InlineData("00010101")]
        [InlineData("20240229")]
        [InlineData("20260715")]
        [InlineData("99991231")]
        public void Validate_Should_Return_Success_When_Value_Matches_Format(string value)
        {
            var model = new DateStringModel
            {
                Value = value
            };

            var validationResults = ValidationTestHelper.Validate(model);

            Assert.Empty(validationResults);
        }

        /// <summary>
        /// Validates values that do not match the configured format.
        /// </summary>
        /// <param name="value">The date string to validate.</param>
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("2026-07-15")]
        [InlineData("15072026")]
        [InlineData("20260715000000")]
        public void Validate_Should_Return_Error_When_Value_Does_Not_Match_Format(string value)
        {
            var model = new DateStringModel
            {
                Value = value
            };

            var validationResults = ValidationTestHelper.Validate(model);

            Assert.Single(validationResults);
        }

        /// <summary>
        /// Validates dates that match the structure but do not exist in the calendar.
        /// </summary>
        /// <param name="value">The invalid calendar date.</param>
        [Theory]
        [InlineData("20230229")]
        [InlineData("20260230")]
        [InlineData("20261301")]
        [InlineData("20260001")]
        public void Validate_Should_Return_Error_When_Date_Does_Not_Exist(string value)
        {
            var model = new DateStringModel
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
            var model = new DateStringModel
            {
                Value = null
            };

            var validationResults = ValidationTestHelper.Validate(model);

            Assert.Empty(validationResults);
        }

        /// <summary>
        /// Validates other supported date formats.
        /// </summary>
        /// <param name="format">The exact date format expected.</param>
        /// <param name="value">The date string to validate.</param>
        [Theory]
        [InlineData("yyyy-MM-dd", "2026-07-15")]
        [InlineData("ddMMyyyy", "15072026")]
        [InlineData("dd/MM/yyyy", "15/07/2026")]
        [InlineData("yyyyddMM", "20261507")]
        [InlineData("dd-MMM-yyyy", "15-Jul-2026")]
        public void Validate_Should_Return_Success_When_Custom_Format_Matches(string format, string value)
        {
            var attribute = new DateStringAttribute(format);

            var validationResult = attribute.GetValidationResult(value, CreateValidationContext());

            Assert.Null(validationResult);
        }

        /// <summary>
        /// Validates that unsupported data types are treated as developer errors.
        /// </summary>
        [Fact]
        public void Validate_Should_Throw_When_Value_Type_Is_Not_Supported()
        {
            var attribute = new DateStringAttribute("yyyyMMdd");
            var unsupportedValues = new object[]
            {
                new DateOnly(2026, 7, 15),
                new DateTime(2026, 7, 15),
                20260715
            };

            foreach (var value in unsupportedValues)
            {
                var exception = Assert.Throws<InvalidOperationException>(() => attribute.GetValidationResult(value, CreateValidationContext()));

                Assert.Contains(nameof(DateStringAttribute), exception.Message);
                Assert.Contains(value.GetType().FullName!, exception.Message);
            }
        }

        /// <summary>
        /// Validates missing date formats.
        /// </summary>
        /// <param name="format">The missing format.</param>
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Constructor_Should_Throw_When_Format_Is_Missing(string? format)
        {
            var exception = Assert.ThrowsAny<ArgumentException>(() => new DateStringAttribute(format!));

            Assert.Equal("format", exception.ParamName);
        }

        /// <summary>
        /// Validates malformed date formats.
        /// </summary>
        [Fact]
        public void Constructor_Should_Throw_When_Format_Is_Malformed()
        {
            var exception = Assert.Throws<ArgumentException>(() => new DateStringAttribute("yyyyMMdd'"));

            Assert.Equal("format", exception.ParamName);
            Assert.IsType<FormatException>(exception.InnerException);
        }

        /// <summary>
        /// Exposes the configured date format.
        /// </summary>
        [Fact]
        public void Constructor_Should_Set_Format()
        {
            var attribute = new DateStringAttribute("yyyyMMdd");

            Assert.Equal("yyyyMMdd", attribute.Format);
        }

        /// <summary>
        /// Validates the default error message and affected member name.
        /// </summary>
        [Fact]
        public void Validate_Should_Return_Default_Error_Message_When_Value_Is_Invalid()
        {
            var attribute = new DateStringAttribute("yyyyMMdd");

            var validationResult = attribute.GetValidationResult("15/07/2026", CreateValidationContext());

            Assert.NotNull(validationResult);
            Assert.Equal("The field Value must be a valid date in the format 'yyyyMMdd'.", validationResult.ErrorMessage);
            Assert.Equal(["Value"], validationResult.MemberNames);
        }

        /// <summary>
        /// Validates a custom localized error message.
        /// </summary>
        [Fact]
        public void Validate_Should_Return_Custom_Error_Message_When_Configured()
        {
            var attribute = new DateStringAttribute("yyyyMMdd")
            {
                ErrorMessage = "El campo {0} debe ser una fecha válida con formato '{1}'."
            };

            var validationResult = attribute.GetValidationResult("15/07/2026", CreateValidationContext());

            Assert.NotNull(validationResult);
            Assert.Equal("El campo Value debe ser una fecha válida con formato 'yyyyMMdd'.", validationResult.ErrorMessage);
        }

        /// <summary>
        /// Validates an error message provided by a resource type.
        /// </summary>
        [Fact]
        public void Validate_Should_Return_Resource_Error_Message_When_Configured()
        {
            var attribute = new DateStringAttribute("yyyyMMdd")
            {
                ErrorMessageResourceName = nameof(TestResources.DateStringErrorMessage),
                ErrorMessageResourceType = typeof(TestResources)
            };

            var validationResult = attribute.GetValidationResult("15/07/2026", CreateValidationContext());

            Assert.NotNull(validationResult);
            Assert.Equal("Resource message for Value with format 'yyyyMMdd'.", validationResult.ErrorMessage);
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
            public static string DateStringErrorMessage => "Resource message for {0} with format '{1}'.";
        }
    }
}
