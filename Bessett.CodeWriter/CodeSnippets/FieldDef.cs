using System;
using System.Collections.Generic;
using System.Linq;
using Bessett.CodeWriter.Common;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Bessett.CodeWriter.CodeSnippets
{
    public class FieldDef: ICodeAtom, ICodeSnippet
    {
        public string Name { get; protected set; }
        public string DataTypeName { get; protected set; }

        public object DefaultValue { get; protected set; }

        public string DefaultDeclaration
        {
            get
            {
                return 
                    HasDefault
                        ? (DefaultValue is ValueType) || IsLiteralDefault
                            ? $" = {DefaultValue}"
                        : DefaultValue is string
                            ? $" = {((string)DefaultValue).InQuotes()}"
                        : DefaultValue is CodeSnippet
                            ? $" = {((CodeSnippet)DefaultValue).ToCSharp()}"
                        : DefaultValue == null
                            ? $" = null"
                        : ""
                    : ""
                ;
                
                //if (HasDefault)
                //{
                //    if (IsLiteralDefault)
                //        return $" = {DefaultValue}";
                //    else if (DefaultValue is string)
                //        return $" = {((string)DefaultValue).InQuotes()}";
                //    else if (DefaultValue is CodeSnippet)
                //        return $" = {((CodeSnippet)DefaultValue).ToCSharp()}";
                //    else if(DefaultValue == null)
                //        return $" = null";
                //    else
                //        return $" = {DefaultValue}";
                //}

                //return "";
            }
        }

        public bool HasDefault { get; protected set; }
        public bool IsLiteralDefault { get; set; }

        public bool IsValid
        {
            get { return !string.IsNullOrEmpty(Name) && (DataTypeName != null); }
        }

        #region Fluent Model

        public FieldDef WithName(string name)
        {
            Name = name;
            return this;
        }
        public FieldDef WithDataType(string name)
        {
            DataTypeName = name;
            return this;
        }
        public FieldDef WithDataType(Type dataType)
        {
            DataTypeName = dataType.ToCSharp();
            return this;
        }
        public FieldDef HasDefaultValue(object value)
        {
            DefaultValue = value;
            HasDefault = true;
            return this;
        }
        public FieldDef HasLiteralDefaultValue(string value)
        {
            DefaultValue = value;
            IsLiteralDefault = true;
            HasDefault = true;
            return this;
        }
        #endregion

        #region Construction
        public FieldDef(string dataTypeName, string name ) : this()
        {
            Name = name;
            DataTypeName = dataTypeName;
        }
        public FieldDef(string dataTypeName, string name, object defaultValue) : this(dataTypeName, name)
        {
            DefaultValue = defaultValue;
            HasDefault = true;
        }
        public FieldDef(Type dataType, string name, object defaultValue) : this(dataType.ToCSharp(), name, defaultValue) {}
        public FieldDef(Type dataType, string name) : this(dataType.ToCSharp(), name) { }

        public FieldDef() { }

        #endregion

        #region Interface
        public IEnumerable<string> ToCSharp()
        {
            return new List<string>() { CSharp };
        }
        public string CSharp
        {
            get { return $"{DataTypeName} {Name} {DefaultDeclaration}"; }
        }
        #endregion

        #region Static

        public static FieldDef Simple(ValueString typename, string fieldName)
        {
            return new FieldDef(typename, fieldName);
        }
        public static FieldDef Simple(ValueString typename, string fieldName, object defaultValue)
        {
            return new FieldDef(typename, fieldName, defaultValue);
        }
        public static FieldDef Simple(Type typename, string fieldName)
        {
            return new FieldDef(typename, fieldName);
        }
        public static FieldDef Simple(Type typename, string fieldName, object defaultValue)
        {
            return new FieldDef(typename, fieldName, defaultValue);
        }
        #endregion
    }
}