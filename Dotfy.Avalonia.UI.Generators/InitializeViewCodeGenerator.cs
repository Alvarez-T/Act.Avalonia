using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Dotfy.Avalonia.UI.Generators;

public class InitializeViewCodeGenerator :  ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new ViewSyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxReceiver is not ViewSyntaxReceiver receiver)
            return;

        foreach (var classDeclaration in receiver.cad
    }
}

public class ViewSyntaxReceiver : ISyntaxReceiver
{
    public List<ClassDeclarationSyntax> CandidateClasses { get; } = [];

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is not ClassDeclarationSyntax { BaseList: not null } classDeclaration) 
            return;

        var baseType = classDeclaration.BaseList.Types.FirstOrDefault()?.Type;
        if (baseType != null && baseType.ToString().EndsWith("View"))
        {
            CandidateClasses.Add(classDeclaration);
        }
    }
}