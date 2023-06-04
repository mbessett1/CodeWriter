using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Bessett.CodeWriter.CodeSnippets
{
    public class CompileException : ApplicationException
    {
        public List<string> Failures = new List<string>();
        protected List<Diagnostic> DiagnosticFailures = new List<Diagnostic>();

        internal CompileException(IEnumerable<Diagnostic> failures ) : base("Compilation failed. See DiagnosticsFailures for details.")
        {
            DiagnosticFailures = failures.ToList();
            Failures = DiagnosticFailures.Select(f=> f.ToString()).ToList();
        }
    }
}