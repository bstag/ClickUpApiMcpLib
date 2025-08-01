using System;
using System.Collections.Generic;
using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Abstractions.Plugins;

namespace ClickUp.Api.Client.Plugins
{
    /// <summary>
    /// Concrete implementation of plugin execution context.
    /// </summary>
    public class PluginContext : IPluginContext
    {
        private readonly Dictionary<string, object> _requestData;
        private readonly Dictionary<string, object> _responseData;
        private readonly Dictionary<string, object> _additionalData;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginContext"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection instance.</param>
        /// <param name="operationType">The operation type being performed.</param>
        /// <param name="serviceName">The service name that initiated the plugin execution.</param>
        /// <param name="requestData">The request data.</param>
        /// <param name="responseData">The response data.</param>
        /// <param name="additionalData">Additional context data.</param>
        public PluginContext(
            IApiConnection apiConnection,
            string operationType,
            string serviceName,
            IDictionary<string, object> requestData = null,
            IDictionary<string, object> responseData = null,
            IDictionary<string, object> additionalData = null)
        {
            ApiConnection = apiConnection ?? throw new ArgumentNullException(nameof(apiConnection));
            OperationType = operationType ?? throw new ArgumentNullException(nameof(operationType));
            ServiceName = serviceName ?? throw new ArgumentNullException(nameof(serviceName));
            
            _requestData = new Dictionary<string, object>(requestData ?? new Dictionary<string, object>());
            _responseData = new Dictionary<string, object>(responseData ?? new Dictionary<string, object>());
            _additionalData = new Dictionary<string, object>(additionalData ?? new Dictionary<string, object>());
        }

        /// <inheritdoc />
        public IApiConnection ApiConnection { get; }

        /// <inheritdoc />
        public IReadOnlyDictionary<string, object> RequestData => _requestData;

        /// <inheritdoc />
        public IReadOnlyDictionary<string, object> ResponseData => _responseData;

        /// <inheritdoc />
        public string OperationType { get; }

        /// <inheritdoc />
        public string ServiceName { get; }

        /// <inheritdoc />
        public IReadOnlyDictionary<string, object> AdditionalData => _additionalData;

        /// <inheritdoc />
        public T GetValue<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return default(T);

            // Try request data first
            if (_requestData.TryGetValue(key, out var requestValue))
                return ConvertValue<T>(requestValue);

            // Try response data
            if (_responseData.TryGetValue(key, out var responseValue))
                return ConvertValue<T>(responseValue);

            // Try additional data
            if (_additionalData.TryGetValue(key, out var additionalValue))
                return ConvertValue<T>(additionalValue);

            return default(T);
        }

        /// <inheritdoc />
        public T GetValue<T>(string key, T defaultValue)
        {
            if (string.IsNullOrWhiteSpace(key))
                return defaultValue;

            // Try request data first
            if (_requestData.TryGetValue(key, out var requestValue))
                return ConvertValue(requestValue, defaultValue);

            // Try response data
            if (_responseData.TryGetValue(key, out var responseValue))
                return ConvertValue(responseValue, defaultValue);

            // Try additional data
            if (_additionalData.TryGetValue(key, out var additionalValue))
                return ConvertValue(additionalValue, defaultValue);

            return defaultValue;
        }

        /// <summary>
        /// Adds or updates request data.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void SetRequestData(string key, object value)
        {
            if (!string.IsNullOrWhiteSpace(key))
                _requestData[key] = value;
        }

        /// <summary>
        /// Adds or updates response data.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void SetResponseData(string key, object value)
        {
            if (!string.IsNullOrWhiteSpace(key))
                _responseData[key] = value;
        }

        /// <summary>
        /// Adds or updates additional data.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void SetAdditionalData(string key, object value)
        {
            if (!string.IsNullOrWhiteSpace(key))
                _additionalData[key] = value;
        }

        private static T ConvertValue<T>(object value)
        {
            if (value is T directValue)
                return directValue;

            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return default(T);
            }
        }

        private static T ConvertValue<T>(object value, T defaultValue)
        {
            if (value is T directValue)
                return directValue;

            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}