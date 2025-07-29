namespace ClickUp.Api.Client.CLI.Infrastructure
{
    /// <summary>
    /// Implementation of IDebugStateService to track debug state.
    /// </summary>
    public class DebugStateService : IDebugStateService
    {
        private bool _isDebugEnabled;
        
        /// <inheritdoc />
        public bool IsDebugEnabled => _isDebugEnabled;
        
        /// <inheritdoc />
        public void SetDebugEnabled(bool enabled)
        {
            _isDebugEnabled = enabled;
        }
    }
}