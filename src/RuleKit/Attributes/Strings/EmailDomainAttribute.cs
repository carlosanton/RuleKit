using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace RuleKit
{
    /// <summary>
    /// Validates that a string is an email address whose domain is included in a configured list.
    /// </summary>
    /// <remarks>
    /// Domain comparisons ignore differences between uppercase and lowercase characters. Subdomains are rejected unless
    /// <see cref="AllowSubdomains"/> is enabled. <see langword="null"/> values are considered valid.
    /// Applying this attribute to a value other than a string causes an <see cref="InvalidOperationException"/>.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property)]
    public sealed class EmailDomainAttribute : ValidationAttribute
    {
        private const string DefaultErrorMessage = "The field {0} must be a valid email address with one of these domains: {1}.";
        private static readonly EmailAddressAttribute EmailAddressValidator = new();
        private readonly string formattedDomains;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailDomainAttribute"/> class.
        /// </summary>
        /// <param name="domains">The email domains that are allowed.</param>
        /// <exception cref="ArgumentException">
        /// No domains are provided, or one of the domains is empty, contains white space or is not a valid ASCII domain name.
        /// </exception>
        public EmailDomainAttribute(params string[] domains) : base(DefaultErrorMessage)
        {
            ArgumentNullException.ThrowIfNull(domains);

            if (domains.Length == 0)
            {
                throw new ArgumentException("At least one email domain must be provided.", nameof(domains));
            }

            foreach (var domain in domains)
            {
                if (!IsValidDomain(domain))
                {
                    throw new ArgumentException($"'{domain}' is not a valid ASCII email domain.", nameof(domains));
                }
            }

            Domains = Array.AsReadOnly(domains.ToArray());
            formattedDomains = string.Join(", ", Domains.Select(domain => $"'{domain}'"));
        }

        /// <summary>
        /// Gets the email domains that are allowed.
        /// </summary>
        public IReadOnlyList<string> Domains { get; }

        /// <summary>
        /// Gets or sets whether addresses from subdomains of the configured domains are allowed.
        /// </summary>
        public bool AllowSubdomains { get; set; }

        /// <summary>
        /// Formats the validation error message.
        /// </summary>
        /// <param name="name">The name of the field that caused the validation failure.</param>
        /// <returns>The formatted validation error message.</returns>
        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, formattedDomains);
        }

        /// <summary>
        /// Validates that the value is an email address and that its domain is allowed.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context in which validation is performed.</param>
        /// <returns><see cref="ValidationResult.Success"/> when the value is valid; otherwise, a validation error.</returns>
        /// <exception cref="InvalidOperationException">The attribute is applied to a value that is not a string.</exception>
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // RequiredAttribute is responsible for missing values. This attribute only checks values that are present.
            if (value is null)
            {
                return ValidationResult.Success;
            }

            if (value is not string email)
            {
                throw new InvalidOperationException($"{nameof(EmailDomainAttribute)} cannot validate values of type '{value.GetType().FullName}'.");
            }

            // Keep the received value untouched and reject surrounding spaces instead of silently trimming them.
            if (email.Length == email.Trim().Length && EmailAddressValidator.IsValid(email))
            {
                var domain = email[(email.LastIndexOf('@') + 1)..];

                if (IsValidDomain(domain) && Domains.Any(allowedDomain => IsAllowedDomain(domain, allowedDomain)))
                {
                    return ValidationResult.Success;
                }
            }

            var memberNames = validationContext.MemberName is null
                ? null
                : new[] { validationContext.MemberName };

            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName), memberNames);
        }

        private bool IsAllowedDomain(string domain, string allowedDomain)
        {
            if (string.Equals(domain, allowedDomain, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return AllowSubdomains && domain.EndsWith($".{allowedDomain}", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsValidDomain(string? domain)
        {
            if (string.IsNullOrWhiteSpace(domain) || domain.Length > 253 || domain.Any(character => character > 127))
            {
                return false;
            }

            var labels = domain.Split('.');

            return labels.All(label => label.Length is > 0 and <= 63
                && char.IsAsciiLetterOrDigit(label[0])
                && char.IsAsciiLetterOrDigit(label[^1])
                && label.All(character => char.IsAsciiLetterOrDigit(character) || character == '-'));
        }
    }
}
