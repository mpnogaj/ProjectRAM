using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;

namespace ProjectRAM.Core.Generators;

[Generator]
public class CommandsGenerator : IIncrementalGenerator
{
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		//attribute
		try
		{
			context.RegisterPostInitializationOutput(callback
				=> callback.AddSource(
					SourceGeneratorHelper.AttributeHint,
					SourceText.From(SourceGeneratorHelper.ClassNameAttribute, Encoding.UTF8))
			);

			IncrementalValuesProvider<ClassDeclarationSyntax> classDeclarations = context.SyntaxProvider
				.CreateSyntaxProvider(
					static (syntaxNode, _) => IsTargeted(syntaxNode),
					static (syntaxContext, _) => GetSemanticTargetForGeneration(syntaxContext))
				.Where(x => x is not null)!;

			IncrementalValueProvider<(Compilation, ImmutableArray<ClassDeclarationSyntax>)> compilationAndClasses
				= context.CompilationProvider.Combine(classDeclarations.Collect());

			context.RegisterSourceOutput(compilationAndClasses,
				static (productionContext, tuple) => Execute(tuple.Item1, tuple.Item2, productionContext));
		}
		catch (Exception ex)
		{
#if DEBUG
			if (!Debugger.IsAttached)
			{
				Debugger.Launch();
			}
#endif
		}

	}

	private static void Execute(Compilation compilation, ImmutableArray<ClassDeclarationSyntax> classes, SourceProductionContext productionContext)
	{
		if (classes.IsDefaultOrEmpty)
		{
			return;
		}

		var distinctClasses = classes.Distinct();
		List<CommandToAdd> commandToAdds =
			GetTypesToGenerate(compilation, distinctClasses, productionContext.CancellationToken);

		if (commandToAdds.Count > 0)
		{
			var res = SourceGeneratorHelper.GenerateCommandHelperClass(commandToAdds);
			productionContext.AddSource(SourceGeneratorHelper.HelperClassHint, SourceText.From(res, Encoding.UTF8));
		}
	}

	private static List<CommandToAdd> GetTypesToGenerate(Compilation compilation, IEnumerable<ClassDeclarationSyntax> distinctClasses, CancellationToken cancellationToken)
	{
		var commandsToAdd = new List<CommandToAdd>();
		if (compilation.GetTypeByMetadataName(SourceGeneratorHelper.AttributeFullName) == null)
		{
			return commandsToAdd;
		}

		foreach (var classDeclarationSyntax in distinctClasses)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var semanticModel = compilation.GetSemanticModel(classDeclarationSyntax.SyntaxTree);
			if (semanticModel.GetDeclaredSymbol(classDeclarationSyntax) is not INamedTypeSymbol classSymbol)
			{
				continue;
			}


			var attribute = classSymbol
				.GetAttributes()
				.FirstOrDefault(attr => attr.AttributeClass is { Name: SourceGeneratorHelper.AttributeName });

			if (attribute == null)
			{
				continue;
			}

			var fullTypeString = $"{classSymbol.ContainingNamespace}.{classSymbol.Name}";
			var commandName = (string)attribute.ConstructorArguments[0].Value!;
			commandsToAdd.Add(new CommandToAdd(commandName, fullTypeString));
		}

		return commandsToAdd;
	}

	private static ClassDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext syntaxContext)
	{
		var classDeclarationSyntax = (ClassDeclarationSyntax)syntaxContext.Node;

		foreach (var attributeListSyntax in classDeclarationSyntax.AttributeLists)
		{
			foreach (var attributeSyntax in attributeListSyntax.Attributes)
			{
				if (syntaxContext.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol
					attributeSymbol)
				{
					continue;
				}

				var attributeContainingTypeSymbol = attributeSymbol.ContainingType;
				var fullName = attributeContainingTypeSymbol.ToDisplayString();

				if (fullName == SourceGeneratorHelper.AttributeFullName)
				{
					return classDeclarationSyntax;
				}
			}
		}

		return null;
	}

	private static bool IsTargeted(SyntaxNode syntaxNode)
	{
		return syntaxNode is ClassDeclarationSyntax classDeclarationSyntax &&
		       classDeclarationSyntax.AttributeLists.Count > 0;
	}
}