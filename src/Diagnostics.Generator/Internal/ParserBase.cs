using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Diagnostics.Generator.Internal
{
    internal abstract class ParserBase
    {
        public const string GlobalNs = "<global namespace>";
        public const string GlobalNsKeyword = "<global";

        public static string GetSpecialName(string name)
        {
            var specialName = name.TrimStart('_');
            return char.ToUpper(specialName[0]) + specialName.Substring(1);
        }

        public static string GetVisiblity(ISymbol symbol)
        {
            return GetAccessibilityString(symbol.DeclaredAccessibility);
        }

        public static bool HasKeyword(ISymbol symbol, SyntaxKind kind)
        {
            var syntax = symbol.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax();
            if (symbol == null)
            {
                return false;
            }
            if (syntax is MemberDeclarationSyntax m)
            {
                return m.Modifiers.Any(x => x.IsKind(kind));
            }
            if (syntax is ClassDeclarationSyntax c)
            {
                return c.Modifiers.Any(x => x.IsKind(kind));
            }
            if (syntax is StructDeclarationSyntax s)
            {
                return s.Modifiers.Any(x => x.IsKind(kind));
            }
            return false;
        }

        public static bool IsAutoGen(ISymbol symbol)
        {
            return symbol.GetAttributes()
                    .Any(x => x.AttributeClass?.ToString() == typeof(GeneratorAttribute).FullName);
        }

        public static ISymbol? GetFieldOrPropertySymbolType(ISymbol symbol, int index)
        {
            if (symbol is IFieldSymbol fieldSymbol)
            {
                return ((INamedTypeSymbol)fieldSymbol.Type).TypeArguments[index];
            }
            else if (symbol is IPropertySymbol propertySymbol)
            {
                return ((INamedTypeSymbol)propertySymbol.Type).TypeArguments[index];
            }
            return null;
        }


        public static string GetTypeFullName(ISymbol symbol)
        {
            var rawNameSpace = GetNameSpace(symbol);
            if (rawNameSpace.Contains(GlobalNsKeyword))
            {
                return string.Empty;
            }
            return $"global::{symbol}";
        }
        private static bool HasParentClass(ISymbol symbol)
        {
            if (symbol.DeclaringSyntaxReferences.Length == 0)
            {
                return false;
            }
            TypeDeclarationSyntax? syntax = (TypeDeclarationSyntax?)symbol.DeclaringSyntaxReferences[0].GetSyntax();
            int hasParentClass = 0;
            for (TypeDeclarationSyntax? currentType = syntax; currentType != null; currentType = currentType.Parent as TypeDeclarationSyntax)
            {
                if (hasParentClass == 0)
                {
                    hasParentClass = 1;
                }
                else if (hasParentClass == 1)
                {
                    hasParentClass = 2;
                    break;
                }
            }
            return hasParentClass == 2;
        }
        private static bool GetNestClassDeclare(ISymbol symbol, SemanticModel model, out string? start, out string? end)
        {
            start = null;
            end = null;
            if (!HasParentClass(symbol))
            {
                return false;
            }

            var endStr = string.Empty;
            var currentType = symbol.ContainingType;
            var classes = new List<string>();
            while (currentType != null)
            {
                StringBuilder stringBuilder = new();

                var syntax = (TypeDeclarationSyntax)currentType.DeclaringSyntaxReferences[0].GetSyntax();

                foreach (SyntaxToken modifier in syntax.Modifiers)
                {
                    stringBuilder.Append(modifier.Text);
                    stringBuilder.Append(' ');
                }

                stringBuilder.Append(GetTypeKindKeyword(syntax));
                stringBuilder.Append(' ');

                string typeName = currentType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
                stringBuilder.Append(typeName);
                classes.Add(stringBuilder.ToString());
                endStr += "}\n";
                currentType = currentType.ContainingType;
            }
            classes.Reverse();
            start = string.Join("\n", classes.Select(x => x + " {"));
            end = endStr;
            return true;
        }
        public static string GetTypeKindKeyword(TypeDeclarationSyntax typeDeclaration)
        {
            switch (typeDeclaration.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    return "class";
                case SyntaxKind.InterfaceDeclaration:
                    return "interface";
                case SyntaxKind.StructDeclaration:
                    return "struct";
                case SyntaxKind.RecordDeclaration:
                    return "record";
                case SyntaxKind.RecordStructDeclaration:
                    return "record struct";
                case SyntaxKind.EnumDeclaration:
                    return "enum";
                case SyntaxKind.DelegateDeclaration:
                    return "delegate";
                default:
                    Debug.Fail("unexpected syntax kind");
                    return null;
            }
        }
        public static void GetWriteNameSpace(ISymbol symbol, SemanticModel model, out string nameSpaceStart, out string nameSpaceEnd, bool includeClassInClass = false)
        {
            var rawNameSpace = GetNameSpace(symbol);

            var additionStart = string.Empty;
            var additionEnd = string.Empty;

            if (GetNestClassDeclare(symbol, model, out var start, out var end))
            {
                additionStart = "\n" + start;
                additionEnd = "\n" + end;
            }
            if (string.IsNullOrEmpty(rawNameSpace))
            {
                nameSpaceStart = additionStart;
                nameSpaceEnd = additionEnd;
            }
            else
            {
                nameSpaceStart = $"namespace {rawNameSpace}\n{{" + additionStart;
                nameSpaceEnd = "}" + additionEnd;
            }
        }
        public static string GetNameSpace(ISymbol symbol)
        {
            var ns = symbol.ContainingNamespace.ToString();
            if (ns.Contains(GlobalNsKeyword))
            {
                return string.Empty;
            }
            return ns;
        }
        public static string GetAccessibilityString(Accessibility accessibility)
        {
            if (accessibility == Accessibility.Private)
            {
                return "private";
            }
            if (accessibility == Accessibility.ProtectedAndInternal)
            {
                return "protected internal";
            }
            if (accessibility == Accessibility.Protected)
            {
                return "protected";
            }
            if (accessibility == Accessibility.Internal)
            {
                return "internal";
            }
            if (accessibility == Accessibility.Public)
            {
                return "public";
            }
            return string.Empty;
        }
    }
}
