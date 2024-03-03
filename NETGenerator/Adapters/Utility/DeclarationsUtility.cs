using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using PascalABCCompiler.NETGenerator.Adapters;
using PascalABCCompiler.NETGenerator.Adapters.RoslynAdapters;
using Roslyn.Utilities;

namespace NETGenerator.Adapters.Utility
{
    internal static class DeclarationsUtility
    {
        public static ImmutableArray<SingleNamespaceOrTypeDeclaration> CreateDeclarations(IEnumerable<RoslynTypeBuilderAdapter> types)
        {
            var rootNamespace = new NamespaceNode("");
            foreach (var type in types)
            {
                var namespaces = type.Namespace.Split('.');
                var current = rootNamespace;
                foreach (var ns in namespaces)
                {
                    var next = Find(current, ns);
                    if (next is null)
                    {
                        var newNode = new NamespaceNode(ns);
                        current.Children.Add(newNode);
                        current = newNode;
                    }
                    else
                    {
                        Debug.Assert(next is NamespaceNode);
                        current = next as NamespaceNode;
                    }
                }
                
                Debug.Assert(current is object);
                current.Children.Add(new TypeNode(type));
            }

            return ToImmutable(rootNamespace);
        }

        public static SingleNamespaceDeclaration CreateNamespaceDeclaration(string name, ImmutableArray<SingleNamespaceOrTypeDeclaration> members)
        {
            return SingleNamespaceDeclaration.Create(
                name, 
                false, 
                false, 
                null, 
                null,
                members, 
                ImmutableArray<Diagnostic>.Empty
            );
        }

        private static Node Find(Node currentNode, string lookupName)
        {
            if (lookupName.IsEmpty())
            {
                return currentNode;
            }
            
            Debug.Assert(currentNode is NamespaceNode);

            return currentNode.Children.Find(node => node.Name == lookupName);
        }

        private static ImmutableArray<SingleNamespaceOrTypeDeclaration> ToImmutable(Node node)
        {
            return node.Children.Select(n => n.CreateDeclaration()).ToImmutableArray();
        }
    }

    internal abstract class Node
    {
        public List<Node> Children { get; }
        public string Name { get; }

        protected Node(string name)
        {
            Name = name;
            Children = new List<Node>();
        }

        public abstract SingleNamespaceOrTypeDeclaration CreateDeclaration();
    }

    internal class NamespaceNode : Node
    {
        public NamespaceNode(string name) : base(name)
        { }

        public override SingleNamespaceOrTypeDeclaration CreateDeclaration()
        {
            var members = Children
                .Select(node => node.CreateDeclaration())
                .ToImmutableArray();
            
            return DeclarationsUtility.CreateNamespaceDeclaration(Name, members);
        }
    }

    internal class TypeNode : Node
    {
        private RoslynTypeBuilderAdapter _type;

        public TypeNode(RoslynTypeBuilderAdapter type) : base(type.Name)
        {
            _type = type;
        }

        public override SingleNamespaceOrTypeDeclaration CreateDeclaration()
        {
            _type.CreateType();
            return _type.Declaration;
        }
    }
}