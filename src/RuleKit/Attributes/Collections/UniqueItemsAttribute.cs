using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace RuleKit
{
    /// <summary>
    /// Validates that a collection does not contain duplicate items.
    /// </summary>
    /// <remarks>
    /// Items use their standard .NET equality rules by default. String collections can ignore differences in casing or
    /// diacritical marks by enabling <see cref="IgnoreCase"/> or <see cref="IgnoreDiacritics"/>.
    /// <see langword="null"/> values are considered valid.
    /// Applying this attribute to a value that is not a collection causes an <see cref="InvalidOperationException"/>.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property)]
    public sealed class UniqueItemsAttribute : ValidationAttribute
    {
        private const string DefaultErrorMessage = "The field {0} must contain unique items.";

        /// <summary>
        /// Initializes a new instance of the <see cref="UniqueItemsAttribute"/> class.
        /// </summary>
        public UniqueItemsAttribute() : base(DefaultErrorMessage)
        {
        }

        /// <summary>
        /// Gets or sets whether string items ignore differences between uppercase and lowercase characters.
        /// </summary>
        public bool IgnoreCase { get; set; }

        /// <summary>
        /// Gets or sets whether string items ignore diacritical marks such as accents.
        /// </summary>
        public bool IgnoreDiacritics { get; set; }

        /// <summary>
        /// Validates that every item in the collection is unique.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context in which validation is performed.</param>
        /// <returns><see cref="ValidationResult.Success"/> when the value is valid; otherwise, a validation error.</returns>
        /// <exception cref="InvalidOperationException">
        /// The attribute is applied to a value that is not a collection, or string comparison options are used with non-string items.
        /// </exception>
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // RequiredAttribute is responsible for missing values. This attribute only checks collections that are present.
            if (value is null)
            {
                return ValidationResult.Success;
            }

            if (value is string || value is not IEnumerable items)
            {
                throw new InvalidOperationException($"{nameof(UniqueItemsAttribute)} cannot validate values of type '{value.GetType().FullName}'.");
            }

            IEqualityComparer<object?> comparer = IgnoreCase || IgnoreDiacritics
                ? new StringItemComparer(IgnoreCase, IgnoreDiacritics)
                : EqualityComparer<object?>.Default;
            var uniqueItems = new HashSet<object?>(comparer);

            // Stop as soon as the first duplicate is found; the attribute only needs to report whether the collection is valid.
            foreach (var item in items)
            {
                if (!uniqueItems.Add(item))
                {
                    var memberNames = validationContext.MemberName is null
                        ? null
                        : new[] { validationContext.MemberName };

                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName), memberNames);
                }
            }

            return ValidationResult.Success;
        }

        private sealed class StringItemComparer : IEqualityComparer<object?>
        {
            private readonly CompareInfo compareInfo = CultureInfo.InvariantCulture.CompareInfo;
            private readonly CompareOptions compareOptions;

            internal StringItemComparer(bool ignoreCase, bool ignoreDiacritics)
            {
                compareOptions = CompareOptions.None;

                if (ignoreCase)
                {
                    compareOptions |= CompareOptions.IgnoreCase;
                }

                if (ignoreDiacritics)
                {
                    compareOptions |= CompareOptions.IgnoreNonSpace;
                }
            }

            public new bool Equals(object? first, object? second)
            {
                if (first is null || second is null)
                {
                    return first is null && second is null;
                }

                return compareInfo.Compare(GetString(first), GetString(second), compareOptions) == 0;
            }

            public int GetHashCode(object? item)
            {
                return item is null
                    ? 0
                    : compareInfo.GetHashCode(GetString(item), compareOptions);
            }

            private static string GetString(object item)
            {
                if (item is not string stringItem)
                {
                    throw new InvalidOperationException($"{nameof(UniqueItemsAttribute)} can only use {nameof(IgnoreCase)} and {nameof(IgnoreDiacritics)} with string collections. The collection contains a value of type '{item.GetType().FullName}'.");
                }

                return stringItem;
            }
        }
    }
}
