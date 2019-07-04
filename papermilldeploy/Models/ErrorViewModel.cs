using System;

namespace papermilldeploy.Models
{
    /// <summary>
    /// The error Model
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// The request id which errored
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// Returns the request id
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}