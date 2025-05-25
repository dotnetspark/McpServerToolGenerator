using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Roslyn incremental source generator that automatically generates tools
/// in the ModelContextProtocol (MCP) ecosystem.
/// <para>
/// Scans for classes annotated with <c>[McpServerToolNameAttribute]</c> and methods with <c>[McpServerToolTypeDescriptionAttribute]</c>.
/// For each such class, generates a static class with tool methods that wrap the annotated methods,
/// including descriptions and proper parameter forwarding.
/// </para>
/// <para>
/// Usage:
/// <code>
/// [McpServerToolNameAttribute("Greeting")]
/// public class Greeting
/// {
///     [McpServerToolTypeDescriptionAttribute("Do something with the tool.")]
///     public void DoSomething() { /* ... */ }
/// }
/// </code>
/// </para>
/// </summary>
[Generator(LanguageNames.CSharp)]
public class FastTrackGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Step 1: Find class declarations with McpServerToolNameAttribute
        var classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: (node, _) => node is ClassDeclarationSyntax,
                transform: (ctx, _) => GetClassInfo(ctx))
            .Where(classInfo => classInfo != null);

        // Step 2: Register source output
        context.RegisterSourceOutput(classDeclarations, GenerateSource);
    }

    private static ClassInfo GetClassInfo(GeneratorSyntaxContext context)
    {
        var classSyntax = (ClassDeclarationSyntax)context.Node;
        var classSymbol = context.SemanticModel.GetDeclaredSymbol(classSyntax) as INamedTypeSymbol;
        if (classSymbol == null)
            return null;

        // Get the attribute symbols from the compilation using the full metadata name of the attributes
        var toolNameAttrSymbol = context.SemanticModel.Compilation.GetTypeByMetadataName("Common.McpServerToolNameAttribute");
        var toolTypeDescAttrSymbol = context.SemanticModel.Compilation.GetTypeByMetadataName("Common.McpServerToolTypeDescriptionAttribute");
        if (toolNameAttrSymbol == null || toolTypeDescAttrSymbol == null)
            return null;

        // Look for [McpServerToolNameAttribute] using symbol comparison
        var toolNameAttr = classSymbol.GetAttributes()
            .FirstOrDefault(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, toolNameAttrSymbol));
        if (toolNameAttr == null)
            return null;

        var toolName = toolNameAttr.ConstructorArguments.Length > 0
            ? toolNameAttr.ConstructorArguments[0].Value?.ToString() ?? classSymbol.Name
            : classSymbol.Name;

        // Find methods with [McpServerToolTypeDescriptionAttribute] using symbol comparison
        var methods = classSymbol.GetMembers()
            .OfType<IMethodSymbol>()
            .Where(m => m.GetAttributes().Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, toolTypeDescAttrSymbol)))
            .Select(m =>
            {
                var descAttr = m.GetAttributes().First(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, toolTypeDescAttrSymbol));
                var desc = descAttr.ConstructorArguments.Length > 0
                    ? descAttr.ConstructorArguments[0].Value?.ToString() ?? ""
                    : "";
                return new MethodInfo
                {
                    Name = m.Name,
                    ReturnType = m.ReturnType.ToDisplayString(),
                    Parameters = m.Parameters.Select(p => (p.Type.ToDisplayString(), p.Name)).ToList(),
                    Description = desc
                };
            })
            .ToList();

        if (methods.Count == 0)
            return null;

        return new ClassInfo
        {
            ToolName = toolName,
            ClassName = classSymbol.Name,
            Namespace = classSymbol.ContainingNamespace.IsGlobalNamespace ? null : classSymbol.ContainingNamespace.ToDisplayString(),
            Methods = methods
        };
    }

    private static void GenerateSource(SourceProductionContext context, ClassInfo classInfo)
    {
        var sb = new StringBuilder();
        sb.AppendLine("using System;");
        sb.AppendLine("using System.ComponentModel;");
        sb.AppendLine("using ModelContextProtocol.Server;");
        sb.AppendLine();
        if (!string.IsNullOrEmpty(classInfo.Namespace))
        {
            sb.AppendLine($"namespace {classInfo.Namespace};");
            sb.AppendLine();
        }
        sb.AppendLine("[McpServerToolType]");
        sb.AppendLine($"public static class {classInfo.ToolName}Tools");
        sb.AppendLine("{");
        foreach (var method in classInfo.Methods)
        {
            sb.AppendLine($"    [McpServerTool, Description(\"{method.Description}\")]");
            var paramList = new List<string> { $"{classInfo.ClassName} {FirstLower(classInfo.ClassName)}" };
            paramList.AddRange(method.Parameters.Select(p => $"{p.Item1} {p.Item2}"));
            var paramString = string.Join(", ", paramList);
            var callParams = string.Join(", ", method.Parameters.Select(p => p.Item2));
            sb.AppendLine($"    public static {method.ReturnType} {method.Name}({paramString}) => {FirstLower(classInfo.ClassName)}.{method.Name}({callParams});");
        }
        sb.AppendLine("}");

        context.AddSource($"{classInfo.ToolName}Tools.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
    }

    private static string FirstLower(string s) =>
        string.IsNullOrEmpty(s) ? s : char.ToLowerInvariant(s[0]) + s.Substring(1);

    private class ClassInfo
    {
        public string ToolName { get; set; }
        public string ClassName { get; set; }
        public string Namespace { get; set; }
        public List<MethodInfo> Methods { get; set; }
    }

    private class MethodInfo
    {
        public string Name { get; set; }
        public string ReturnType { get; set; }
        public List<(string, string)> Parameters { get; set; }
        public string Description { get; set; }
    }
}