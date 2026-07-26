using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Mono.Cecil;

namespace LuckierBlackCat.Contracts.Tests;

internal sealed class ReferenceAssembly : IDisposable
{
    private const string ExpectedElinSha256 =
        "118334af7a7ae6ce6798946865904594817734553c6f3728ea9fbea2eac9579a";

    private ReferenceAssembly(string repositoryRoot, AssemblyDefinition assembly)
    {
        RepositoryRoot = repositoryRoot;
        Assembly = assembly;
    }

    public string RepositoryRoot { get; }

    public AssemblyDefinition Assembly { get; }

    public static ReferenceAssembly LoadElin()
    {
        var root = FindRepositoryRoot();
        var path = Path.Combine(root, "ref", "Elin.dll");
        return new ReferenceAssembly(root, AssemblyDefinition.ReadAssembly(path));
    }

    public static string GetElinSha256()
    {
        var path = Path.Combine(FindRepositoryRoot(), "ref", "Elin.dll");
        using var stream = File.OpenRead(path);
        return Convert.ToHexString(SHA256.HashData(stream)).ToLowerInvariant();
    }

    public static string ExpectedSha256 => ExpectedElinSha256;

    public TypeDefinition RequireType(string fullName)
    {
        return Assembly.MainModule.GetType(fullName)
            ?? throw new InvalidOperationException($"Missing type: {fullName}");
    }

    public MethodDefinition RequireMethod(
        string declaringType,
        string name,
        string returnType,
        bool isStatic,
        params string[] parameterTypes)
    {
        var candidates = RequireType(declaringType)
            .Methods
            .Where(method =>
                method.Name == name
                && method.ReturnType.FullName == returnType
                && method.IsStatic == isStatic
                && method.Parameters.Select(parameter => parameter.ParameterType.FullName)
                    .SequenceEqual(parameterTypes))
            .ToArray();

        return candidates.Length switch
        {
            1 => candidates[0],
            0 => throw new InvalidOperationException(
                $"Missing method: {FormatMethod(declaringType, name, returnType, isStatic, parameterTypes)}"),
            _ => throw new InvalidOperationException(
                $"Ambiguous method: {FormatMethod(declaringType, name, returnType, isStatic, parameterTypes)}"),
        };
    }

    public void Dispose()
    {
        Assembly.Dispose();
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "LuckierBlackCat.sln"))
                && File.Exists(Path.Combine(directory.FullName, "ref", "Elin.dll")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            $"Could not find repository root from {AppContext.BaseDirectory}.");
    }

    private static string FormatMethod(
        string declaringType,
        string name,
        string returnType,
        bool isStatic,
        IEnumerable<string> parameterTypes)
    {
        var scope = isStatic ? "static " : string.Empty;
        return $"{scope}{returnType} {declaringType}.{name}({string.Join(", ", parameterTypes)})";
    }
}
