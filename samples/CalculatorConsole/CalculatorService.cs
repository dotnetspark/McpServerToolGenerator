using McpServerToolGenerator.FastTrack.Common;

[McpServerToolName("Calculator")]
public class CalculatorService
{
    [McpServerToolTypeDescription("Adds two integers together.")]
    public int Add(int a, int b)
    {
        return a + b;
    }

    [McpServerToolTypeDescription("Subtracts the second integer from the first.")]
    public int Subtract(int a, int b)
    {
        return a - b;
    }
    
    [McpServerToolTypeDescription("Multiplies two integers together.")]
    public int Multiply(int a, int b)
    {
        return a * b;
    }

    [McpServerToolTypeDescription("Divides the first integer by the second. Returns 0 if the second integer is zero.")]
    public int Divide(int a, int b)
    {
        return b != 0 ? a / b : 0;
    }
}