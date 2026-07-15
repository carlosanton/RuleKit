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
    }
}