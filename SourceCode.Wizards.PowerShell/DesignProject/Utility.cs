using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SourceCode.Workflow.Authoring;
using SourceCode.Workflow.Design;

namespace SourceCode.Wizard.PowerShell.Design
{
    public class Utility
    {
        public static K2Field CreateConfigSettingField<T>(K2Field k2field)
        {
            K2Field newField = new K2Field();
            if (k2field != null)
            {
                newField.Parts.AddRange(k2field.Parts.ToArray());
            }

            if (newField.Parts.Count == 0)
            {
                ValueTypePart val = new ValueTypePart();
                val.Value = default(T);
                newField.Parts.Add(val);
            }

            newField.ValueType = typeof(T);
            foreach (K2FieldPart part in newField.Parts)
            {
                part.ValueType = typeof(T);
            }

            return newField;
        }
    }
}
