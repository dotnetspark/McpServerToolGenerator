namespace McpServerToolGenerator.FastTrack.Common
{
    /// <summary>
    /// Attribute to annotate a class with a McpServer tool name.
    /// Can only be applied once per class and is not inherited by derived classes.
    /// 
    /// Example usage:
    /// <code>
    /// [McpServerToolName("ExampleTool")]
    /// public class ExampleToolClass
    /// {
    ///     // Implementation
    /// }
    /// </code>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class McpServerToolNameAttribute : Attribute
    {
        public string Name { get; }

        public McpServerToolNameAttribute(string name)
        {
            Name = name;
        }
    }
}
