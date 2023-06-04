using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace Bessett.CodeWriter.CodeSnippets
{
    public static class QuickSnippets
    {

        //public static IClassLevelSnippet ImplicitOperatorSnippet(Type returnType, Type valueType, CodeSnippet transforms, string valueDesignation = "value")
        //{
        //    return ImplicitOperatorSnippet(returnType.FullName, valueType.FullName, transforms, valueDesignation);
        //}

        //public static IClassLevelSnippet ImplicitOperatorSnippet(string returnTypeName, string valueTypeName, CodeSnippet transforms, string valueDesignation = "value")
        //{
        //    var snippet = new CodeSnippet()
        //        .AddLine($"public static implicit operator {returnTypeName} ( {valueTypeName} {valueDesignation})")
        //        .EncloseInScope(
        //            new CodeSnippet().AddLine($"return new {returnTypeName}()")
        //            .EncloseInScope(transforms).AddLine(";"))
        //        ;
        //    return (IClassLevelSnippet)snippet;
        //}

        //public static IClassLevelSnippet ConvertSnippet(string methodName, Type returnType, Type valueType, CodeSnippet transforms, string valueDesignation = "value")
        //{
        //    return ConvertSnippet(methodName, returnType.FullName, valueType.FullName, transforms, valueDesignation);
        //}

        //public static IClassLevelSnippet ConvertSnippet(string methodName, string returnTypeName, string valueTypeName, CodeSnippet transforms, string valueDesignation = "value")
        //{
        //    var snippet = new CodeSnippet()
        //        .AddLine($"public {returnTypeName} {methodName} ( {valueTypeName} {valueDesignation})")
        //        .EncloseInScope(
        //            new CodeSnippet().AddLine($"return new {returnTypeName}()")
        //            .EncloseInScope(transforms).AddLine(";"))
        //        ;
        //    return (IClassLevelSnippet)snippet;
        //}

    }



}