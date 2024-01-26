namespace gigadr3w.msauthflow.authenticator.iterator.Configurations
{
    public class JwtTokenConfiguration
    {
        public const string DEFAULT_SCHEMA = "ApiKey";
        /// <summary>
        /// Used for token encryption
        /// </summary>
        public string SecretKey { get; set; }
        /// <summary>
        /// Api Key Header Name
        /// </summary>
        public string ApiKeyHeader { get; set; } = "ApiKey";
        /// <summary>
        /// Minutes to token expiration
        /// </summary>
        public TimeSpan ValidFor { get; set; }

    }
}
