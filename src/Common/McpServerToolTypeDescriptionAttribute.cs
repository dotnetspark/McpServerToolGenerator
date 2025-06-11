namespace Common
{
    /// <summary>
    /// Attribute to annotate methods with a descriptive string for Mcp Server tool types.
    /// Can be applied only one time to a method. Not inherited by derived classes.
    /// 
    /// Example usage:
    /// <code>
    /// [McpServerToolTypeDescription("Initializes the server tool.")]
    /// public void InitializeTool()
    /// {
    ///     // Implementation
    /// }
    /// </code>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class McpServerToolTypeDescriptionAttribute : Attribute
    {
        public string Description { get; }

        public McpServerToolTypeDescriptionAttribute(string description)
        {
            Description = description;
        }
    }
}