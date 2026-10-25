using System.Text;
using System.Text.RegularExpressions;
//using Microsoft.CodeAnalysis;
//using Microsoft.CodeAnalysis.CSharp;
//using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LocalRenamer;

// 用法: LocalRenamer <dir> [--apply]
// 只重命名「反编译临时变量」的局部声明，参数 / 字段 / 成员一律不动。
internal static class Program
{
    private static readonly Regex TempName = new(@"^(num|text|array|flag|obj|val|data|result)(\d+)?$", RegexOptions.Compiled);
    private static readonly Regex LenLike = new(@"\.(Length|Count)\b", RegexOptions.Compiled);
    private static readonly Regex ReadLike = new(@"\.(Receive|Read|ReceiveAsync|ReadAsync)\s*\(", RegexOptions.Compiled);

    private static int Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.Error.WriteLine("usage: LocalRenamer <dir> [--apply]");
            return 2;
        }

        string dir = args[0];
        bool apply = args.Contains("--apply");

        if (args.Contains("--check"))
        {
            return Check(dir);
        }

        int files = 0, considered = 0, renamed = 0, skipped = 0, editsTotal = 0;
        var samples = new List<string>();
        var byKind = new Dictionary<string, int>();
        // 每个方法体已占用的名字（含本次已分配的），避免同一方法内重名
        var ownerNames = new Dictionary<SyntaxNode, HashSet<string>>(ReferenceEqualityComparer.Instance);

        foreach (var file in Directory.EnumerateFiles(dir, "*.cs", SearchOption.AllDirectories))
        {
            files++;
            string text = File.ReadAllText(file);
            var root = CSharpSyntaxTree.ParseText(text).GetRoot();
            var edits = new List<(int Start, int Len, string New)>();

            foreach (var decl in root.DescendantNodes().OfType<LocalDeclarationStatementSyntax>())
            {
                if (decl.Declaration.Variables.Count != 1) continue;
                var v = decl.Declaration.Variables[0];
                string name = v.Identifier.ValueText;
                if (!TempName.IsMatch(name)) continue;

                considered++;
                string type = decl.Declaration.Type.ToString();
                string init = v.Initializer?.Value.ToString() ?? "";
                string suggestion = Suggest(type, init, decl);
                if (suggestion == null) { skipped++; continue; }

                var owner = decl.Ancestors().FirstOrDefault(a => a is MethodDeclarationSyntax
                    or AccessorDeclarationSyntax or LocalFunctionStatementSyntax
                    or ConstructorDeclarationSyntax or OperatorDeclarationSyntax
                    or DestructorDeclarationSyntax);
                if (owner == null) { skipped++; continue; }

                if (!ownerNames.TryGetValue(owner, out var used))
                {
                    used = new HashSet<string>(owner.DescendantTokens()
                        .Where(t => t.IsKind(SyntaxKind.IdentifierToken))
                        .Select(t => t.ValueText));
                    ownerNames[owner] = used;
                }

                string final = null;
                foreach (var cand in Candidates(suggestion))
                {
                    if (!used.Contains(cand)) { final = cand; break; }
                }
                if (final == null) { skipped++; continue; }

                // 原名字让位，新名字占位，保证同一方法内不产生重名
                used.Remove(name);
                used.Add(final);

                var localEdits = new List<(int, int, string)>
                {
                    (v.Identifier.SpanStart, v.Identifier.Span.Length, final)
                };
                foreach (var tok in owner.DescendantTokens())
                {
                    if (!tok.IsKind(SyntaxKind.IdentifierToken)) continue;
                    if (tok.ValueText != name) continue;
                    if (tok.SpanStart < decl.Span.End) continue;
                    localEdits.Add((tok.SpanStart, tok.Span.Length, final));
                }

                edits.AddRange(localEdits);
                renamed++;
                editsTotal += localEdits.Count;
                byKind[suggestion] = byKind.GetValueOrDefault(suggestion) + 1;
                if (samples.Count < 10)
                    samples.Add($"{Path.GetFileName(file),-38} {Trunc($"{type} {name} = {init}"),-70} -> {final}");
            }

            if (apply && edits.Count > 0)
            {
                var uniq = edits.GroupBy(e => e.Start).Select(g => g.First())
                                .OrderByDescending(e => e.Start).ToList();
                var sb = new StringBuilder(text);
                foreach (var e in uniq) sb.Remove(e.Start, e.Len).Insert(e.Start, e.New);
                File.WriteAllText(file, sb.ToString(), new UTF8Encoding(false));
            }
        }

        Console.WriteLine($"files        {files}");
        Console.WriteLine($"candidates   {considered}");
        Console.WriteLine($"renamed      {renamed}");
        Console.WriteLine($"skipped      {skipped}");
        Console.WriteLine($"ident edits  {editsTotal}");
        Console.WriteLine($"mode         {(apply ? "APPLIED" : "dry-run")}");
        foreach (var kv in byKind.OrderByDescending(k => k.Value)) Console.WriteLine($"  -> {kv.Key,-10} {kv.Value}");
        Console.WriteLine("samples:");
        foreach (var s in samples) Console.WriteLine("  " + s);
        return 0;
    }

    private static int Check(string dir)
    {
        int files = 0, syntaxErrFiles = 0, dupScopes = 0;
        var offenders = new List<string>();
        foreach (var file in Directory.EnumerateFiles(dir, "*.cs", SearchOption.AllDirectories))
        {
            files++;
            var tree = CSharpSyntaxTree.ParseText(File.ReadAllText(file));
            var errs = tree.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
            if (errs.Count > 0)
            {
                syntaxErrFiles++;
                if (offenders.Count < 10) offenders.Add($"SYNTAX {Path.GetFileName(file)}: {errs[0].GetMessage()}");
            }

            var root = tree.GetRoot();
            foreach (var block in root.DescendantNodes().OfType<BlockSyntax>())
            {
                var dups = block.Statements.OfType<LocalDeclarationStatementSyntax>()
                    .SelectMany(s => s.Declaration.Variables.Select(v => v.Identifier.ValueText))
                    .GroupBy(n => n)
                    .Where(g => g.Count() > 1)
                    .ToList();
                foreach (var d in dups)
                {
                    dupScopes++;
                    if (offenders.Count < 20) offenders.Add($"DUP {Path.GetFileName(file)}: '{d.Key}' x{d.Count()}");
                }
            }
        }
        Console.WriteLine($"files              {files}");
        Console.WriteLine($"syntax-error files {syntaxErrFiles}");
        Console.WriteLine($"duplicate scopes   {dupScopes}");
        foreach (var o in offenders) Console.WriteLine("  " + o);
        return 0;
    }

    private static IEnumerable<string> Candidates(string baseName)
    {
        if (baseName == "i") { yield return "i"; yield return "j"; yield return "k"; yield break; }
        yield return baseName;
        for (int i = 2; i <= 6; i++) yield return baseName + i;
    }

    private static string Suggest(string type, string init, LocalDeclarationStatementSyntax decl)
    {
        if (type is "byte[]" or "Byte[]" or "System.Byte[]")
        {
            if (init.StartsWith("new byte[") || init.StartsWith("new byte [")) return "buffer";
            if (init.Contains("GetBytes(")) return "bytes";
            return null;
        }

        if (type is "int" or "long" or "uint" or "ulong" or "short" or "ushort" or "double" or "float")
        {
            if (decl.Parent is ForStatementSyntax) return "i";
            if (ReadLike.IsMatch(init)) return "bytesRead";
            if (LenLike.IsMatch(init) && !init.Contains('+') && !init.Contains('-') && !init.Contains('*')) return "count";
            return null;
        }

        if (type == "bool")
        {
            if (init == "false") return "success";
            return null;
        }

        if (type == "string")
        {
            if (init.Contains("GetString(")) return "message";
            return null;
        }

        return null;
    }

    private static string Trunc(string s)
    {
        s = s.Replace('\n', ' ').Replace('\r', ' ').Trim();
        return s.Length <= 64 ? s : s[..64] + "…";
    }
}
