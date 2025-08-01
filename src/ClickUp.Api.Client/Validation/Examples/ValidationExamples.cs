using System;
using ClickUp.Api.Client.Validation;
using ClickUp.Api.Client.Validation.Attributes;
using ValidationHelper = ClickUp.Api.Client.Helpers.ValidationHelper;

namespace ClickUp.Api.Client.Validation.Examples
{
    /// <summary>
    /// Examples demonstrating the new validation framework usage
    /// </summary>
    public static class ValidationExamples
    {
        /// <summary>
        /// Example model with validation attributes
        /// </summary>
        public class CreateTaskRequest
        {
            [Required(ErrorMessage = "Task name is required")]
            [StringLength(255, MinimumLength = 1, ErrorMessage = "Task name must be between 1 and 255 characters")]
            public string Name { get; set; } = string.Empty;

            [ClickUpId(ErrorMessage = "Invalid list ID format")]
            public string ListId { get; set; } = string.Empty;

            [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
            public string? Description { get; set; }

            [Range(1, 5, ErrorMessage = "Priority must be between 1 and 5")]
            public int? Priority { get; set; }
        }

        /// <summary>
        /// Example using attribute-based validation
        /// </summary>
        public static void AttributeBasedValidationExample()
        {
            var request = new CreateTaskRequest
            {
                Name = "", // Invalid - empty
                ListId = "invalid-id", // Invalid - wrong format
                Priority = 10 // Invalid - out of range
            };

            // Validate using the new framework
            var result = ValidationHelper.Validate(request);
            
            if (!result.IsValid)
            {
                Console.WriteLine("Validation failed:");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"- {error.PropertyName}: {error.ErrorMessage}");
                }
                
                // Throw validation exception if needed
                throw new ValidationException(result);
            }
        }

        /// <summary>
        /// Example using fluent validation
        /// </summary>
        public static void FluentValidationExample()
        {
            var request = new CreateTaskRequest();

            var result = ValidationHelper.For(request)
                .Property(x => x.Name)
                    .NotNull("Task name is required")
                    .Length(1, 255, "Task name must be between 1 and 255 characters")
                    .And()
                .Property(x => x.ListId)
                    .NotNull("List ID is required")
                    .ClickUpId("Invalid list ID format")
                    .And()
                .Property(x => x.Priority)
                    .Must(p => p == null || (p >= 1 && p <= 5), "Priority must be between 1 and 5")
                .Validate();

            if (!result.IsValid)
            {
                throw new ValidationException(result);
            }
        }

        /// <summary>
        /// Example showing backward compatibility with existing ValidationHelper methods
        /// </summary>
        public static void BackwardCompatibilityExample(string taskId, string taskName)
        {
            // These methods still work as before but now use the new framework internally
            ClickUp.Api.Client.Helpers.ValidationHelper.ValidateId(taskId, nameof(taskId));
            ClickUp.Api.Client.Helpers.ValidationHelper.ValidateRequiredString(taskName, nameof(taskName), 255);
        }

        /// <summary>
        /// Example of custom validation rules
        /// </summary>
        public static void CustomValidationExample()
        {
            var request = new CreateTaskRequest
            {
                Name = "Test Task",
                ListId = "123456789",
                Description = "Task description"
            };

            var result = ValidationHelper.For(request)
                .Property(x => x.Name)
                    .Must(name => !name.Contains("forbidden"), "Task name cannot contain 'forbidden'")
                .And()
                .Property(x => x.Description)
                    .Must(desc => desc == null || !desc.Contains("spam"), "Description cannot contain 'spam'")
                .Validate();

            if (!result.IsValid)
            {
                throw new ValidationException(result);
            }
        }
    }
}