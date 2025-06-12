# McpServerToolGenerator.FastTrack.Common

This package provides common attributes and utilities for MCP Server Tool generators and consumers.

## Features
- `McpServerToolNameAttribute`: Annotate classes with a tool name for MCP server integration.
- `McpServerToolTypeDescriptionAttribute`: Annotate methods with descriptions for MCP server tools.

## Usage

Add a reference to this package in your MCP server tool or generator project:

```xml
<PackageReference Include="McpServerToolGenerator.FastTrack.Common" Version="0.1.0" />
```

Annotate your classes and methods:

```csharp
using McpServerToolGenerator.FastTrack.Common;

[McpServerToolName("Calculator")]
public class CalculatorService
{
    [McpServerToolTypeDescription("Adds two integers together.")]
    public int Add(int a, int b) => a + b;
}
```

## License

MIT License
