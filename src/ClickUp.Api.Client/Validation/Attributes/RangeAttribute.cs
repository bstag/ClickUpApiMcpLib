using System;
using System.Globalization;

namespace ClickUp.Api.Client.Validation.Attributes
{
    /// <summary>
    /// Specifies the numeric range constraints for the value of a property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class RangeAttribute : ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RangeAttribute"/> class.
        /// </summary>
        /// <param name="minimum">The minimum value.</param>
        /// <param name="maximum">The maximum value.</param>
        /// <param name="errorMessage">The error message template.</param>
        public RangeAttribute(int minimum, int maximum, string? errorMessage = null) : base(errorMessage)
        {
            Minimum = minimum;
            Maximum = maximum;
            OperandType = typeof(int);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeAttribute"/> class.
        /// </summary>
        /// <param name="minimum">The minimum value.</param>
        /// <param name="maximum">The maximum value.</param>
        /// <param name="errorMessage">The error message template.</param>
        public RangeAttribute(double minimum, double maximum, string? errorMessage = null) : base(errorMessage)
        {
            Minimum = minimum;
            Maximum = maximum;
            OperandType = typeof(double);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeAttribute"/> class.
        /// </summary>
        /// <param name="type">The type of the range values.</param>
        /// <param name="minimum">The minimum value as a string.</param>
        /// <param name="maximum">The maximum value as a string.</param>
        /// <param name="errorMessage">The error message template.</param>
        public RangeAttribute(Type type, string minimum, string maximum, string? errorMessage = null) : base(errorMessage)
        {
            OperandType = type ?? throw new ArgumentNullException(nameof(type));
            Minimum = ConvertValue(minimum, type);
            Maximum = ConvertValue(maximum, type);
        }

        /// <summary>
        /// Gets the minimum value for the range.
        /// </summary>
        public object Minimum { get; }

        /// <summary>
        /// Gets the maximum value for the range.
        /// </summary>
        public object Maximum { get; }

        /// <summary>
        /// Gets the type of the operand.
        /// </summary>
        public Type OperandType { get; }

        /// <summary>
        /// Validates the specified value.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="propertyName">The name of the property being validated.</param>
        /// <returns>A validation result.</returns>
        public override ValidationResult Validate(object? value, string propertyName)
        {
            if (value == null)
            {
                return ValidationResult.Success(); // Null values are handled by RequiredAttribute
            }

            try
            {
                var convertedValue = ConvertValue(value, OperandType);
                if (convertedValue is IComparable comparable)
                {
                    if (comparable.CompareTo(Minimum) < 0 || comparable.CompareTo(Maximum) > 0)
                    {
                        return ValidationResult.Failure(propertyName, FormatErrorMessage(propertyName), ErrorCode);
                    }
                }
                else
                {
                    return ValidationResult.Failure(propertyName, $"{propertyName} value cannot be compared.", ErrorCode);
                }
            }
            catch (Exception ex) when (ex is FormatException || ex is InvalidCastException || ex is OverflowException)
            {
                return ValidationResult.Failure(propertyName, $"{propertyName} is not a valid {OperandType.Name}.", ErrorCode);
            }

            return ValidationResult.Success();
        }

        /// <summary>
        /// Gets the default error message for the specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The default error message.</returns>
        protected override string GetDefaultErrorMessage(string propertyName)
        {
            return $"{propertyName} must be between {Minimum} and {Maximum}.";
        }

        /// <summary>
        /// Converts a value to the specified type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The target type.</param>
        /// <returns>The converted value.</returns>
        private static object ConvertValue(object value, Type targetType)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (targetType == typeof(int))
            {
                return Convert.ToInt32(value, CultureInfo.InvariantCulture);
            }
            if (targetType == typeof(double))
            {
                return Convert.ToDouble(value, CultureInfo.InvariantCulture);
            }
            if (targetType == typeof(decimal))
            {
                return Convert.ToDecimal(value, CultureInfo.InvariantCulture);
            }
            if (targetType == typeof(long))
            {
                return Convert.ToInt64(value, CultureInfo.InvariantCulture);
            }
            if (targetType == typeof(DateTime))
            {
                return Convert.ToDateTime(value, CultureInfo.InvariantCulture);
            }

            return Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
        }
    }
}