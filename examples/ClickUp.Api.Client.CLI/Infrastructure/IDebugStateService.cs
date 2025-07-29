namespace ClickUp.Api.Client.CLI.Infrastructure
{
    /// <summary>
    /// Service to track debug state across the application.
    /// </summary>
    public interface IDebugStateService
    {
        /// <summary>
        /// Gets whether debug mode is enabled.
        /// </summary>
        bool IsDebugEnabled { get; }
        
        /// <summary>
        /// Sets the debug state.
        /// </summary>
        /// <param name="enabled">Whether debug mode should be enabled.</param>
        void SetDebugEnabled(bool enabled);
    }
}