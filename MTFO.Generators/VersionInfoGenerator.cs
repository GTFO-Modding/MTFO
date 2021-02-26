using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Text;

namespace MTFO.Generators
{
    [Generator]
    public class VersionInfoGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context) { }

        public void Execute(GeneratorExecutionContext context)
        {
            if (!context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.RootNamespace", out var rootNs))
            {
                throw new InvalidOperationException("Missing RootNamespace property");
            }

            if (!context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.Version", out var version))
            {
                throw new InvalidOperationException("Missing Version property");
            }

            var hintName = $"{rootNs}.VersionInfo";

            var src = $@"
using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;

namespace {rootNs} {{
    [GeneratedCode(""{hintName}"", ""0.1"")]
    [CompilerGenerated]
    internal static class VersionInfo {{
        public const string Version = ""{version}"";
        public const string RootNamespace = ""{rootNs}"";
    }}
}}
";

            context.AddSource(hintName, SourceText.From(src, Encoding.UTF8));
        }
    }
}
