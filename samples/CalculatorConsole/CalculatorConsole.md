# CalculatorConsole

This project demonstrates a simple calculator server that can be run as an MCP (Model Context Protocol) tool. It exposes basic arithmetic operations via the MCP protocol.

## How to Run

To start the server, use the following command:

```sh
dotnet run --project C:\Users\ylrre\source\repos\McpServerToolGenerator\samples\CalculatorConsole\CalculatorConsole.csproj
```

Or, if using the `mcp.json` configuration, the server can be started by an MCP client using the following configuration:

```jsonc
{
    "servers": {
        "calculator-mcp": {
            "type": "stdio",
            "command": "dotnet",
            "args": [
                "run",
                "--project",
                "C:\\Users\\ylrre\\source\\repos\\McpServerToolGenerator\\samples\\CalculatorConsole\\CalculatorConsole.csproj"
            ]
        }
    }
}
```

## Available MCP Tools (after compilation)

Once compiled and run, the following MCP tools are available:

- **Add**: Adds two numbers.
- **Subtract**: Subtracts the second number from the first.
- **Multiply**: Multiplies two numbers.
- **Divide**: Divides the first number by the second (returns 0 if dividing by zero).

### Example Tool Snapshots

#### Add Tool
```
{
  "tool": "Add",
  "params": { "a": 2, "b": 3 },
  "result": 5
}
```

#### Subtract Tool
```
{
  "tool": "Subtract",
  "params": { "a": 5, "b": 2 },
  "result": 3
}
```

#### Multiply Tool
```
{
  "tool": "Multiply",
  "params": { "a": 4, "b": 6 },
  "result": 24
}
```

#### Divide Tool
```
{
  "tool": "Divide",
  "params": { "a": 10, "b": 2 },
  "result": 5
}
```

---

For more details, see the source code in `CalculatorService.cs`.
