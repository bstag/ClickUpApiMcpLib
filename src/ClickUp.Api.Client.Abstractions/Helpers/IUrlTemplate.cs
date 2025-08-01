using System.Collections.Generic;

namespace ClickUp.Api.Client.Abstractions.Helpers
{
    /// <summary>
    /// Interface for URL template processing with parameter substitution.
    /// </summary>
    public interface IUrlTemplate
    {
        /// <summary>
        /// Sets a parameter value for the template.
        /// </summary>
        /// <param name="name">The parameter name (without braces).</param>
        /// <param name="value">The parameter value.</param>
        /// <returns>The URL template instance for method chaining.</returns>
        IUrlTemplate WithParameter(string name, string value);

        /// <summary>
        /// Sets a parameter value for the template if the value is not null or empty.
        /// </summary>
        /// <param name="name">The parameter name (without braces).</param>
        /// <param name="value">The parameter value.</param>
        /// <returns>The URL template instance for method chaining.</returns>
        IUrlTemplate WithParameterIfNotEmpty(string name, string? value);

        /// <summary>
        /// Sets multiple parameters from a dictionary.
        /// </summary>
        /// <param name="parameters">The parameters to set.</param>
        /// <returns>The URL template instance for method chaining.</returns>
        IUrlTemplate WithParameters(Dictionary<string, string> parameters);

        /// <summary>
        /// Sets an integer parameter value for the template.
        /// </summary>
        /// <param name="name">The parameter name (without braces).</param>
        /// <param name="value">The integer value.</param>
        /// <returns>The URL template instance for method chaining.</returns>
        IUrlTemplate WithParameter(string name, int value);

        /// <summary>
        /// Sets a long parameter value for the template.
        /// </summary>
        /// <param name="name">The parameter name (without braces).</param>
        /// <param name="value">The long value.</param>
        /// <returns>The URL template instance for method chaining.</returns>
        IUrlTemplate WithParameter(string name, long value);

        /// <summary>
        /// Validates that all required parameters have been provided.
        /// </summary>
        /// <returns>The URL template instance for method chaining.</returns>
        /// <exception cref="System.InvalidOperationException">Thrown if required parameters are missing.</exception>
        IUrlTemplate Validate();

        /// <summary>
        /// Builds the final URL by substituting all parameters.
        /// </summary>
        /// <returns>The URL with all parameters substituted.</returns>
        string Build();

        /// <summary>
        /// Gets the list of parameter names found in the template.
        /// </summary>
        /// <returns>A list of parameter names (without braces).</returns>
        IReadOnlyList<string> GetParameterNames();

        /// <summary>
        /// Gets the list of missing parameters that haven't been set.
        /// </summary>
        /// <returns>A list of missing parameter names.</returns>
        IReadOnlyList<string> GetMissingParameters();
    }
}