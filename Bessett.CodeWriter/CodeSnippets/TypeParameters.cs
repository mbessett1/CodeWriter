using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bessett.CodeWriter.CodeSnippets
{
    public class TypeParameterList 
    {
        public Dictionary<string, TypeParameter> TypeParams = new Dictionary<string, TypeParameter>();

        public TypeParameterList AddRange(params TypeParameter[] typeParameters)
        {
            foreach (var typeParameter in typeParameters)
            {
                TypeParams.Add(typeParameter.Name, typeParameter);
            }
            return this;
        }

        public bool HasValues
        {
            get { return TypeParams.Count > 0; }
        }

        public string ParamList
        {
            get
            {
                if (HasValues)
                    return $"<{string.Join(",", TypeParams.Keys)}>";
                else
                    return "";
            }
        }

        public CodeSnippet ConstraintSnippet
        {
            get
            {
                return CodeSnippet.FromLines(
                    TypeParams.Select(
                        t => $"where {t.Value.Name} : {(string.Join(",", t.Value.Constraints.ToArray()))}"));
            }
        }
    }

    public class TypeParameter
    {
        public string Name { get; set; }
        public List<string> Constraints = new List<string>();

        #region Construction

        public TypeParameter(string name, params string[] constraints)
        {
            WithName(name).WithConstraint(constraints);
        }

        #endregion
        #region Fluent
        public TypeParameter WithName(string name)
        {
            Name = name;
            return this;
        }
        public TypeParameter WithConstraint(params string[] constraints)
        {
            Constraints.AddRange(constraints);
            return this;
        }
        #endregion  

    }
}
