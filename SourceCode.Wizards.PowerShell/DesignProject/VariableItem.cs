using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SourceCode.Workflow.Authoring;
using SourceCode.Framework;

namespace SourceCode.Wizard.PowerShell.Design
{
    public class VariableItem : PersistableContainerObject
    {
        private K2Field _variableValue;
        private string _variableName;
        private const string VARIABLEVALUE = "VariableValue";
        private const string VARIABLENAME = "VariableName";


        public VariableItem()
            : base()
        {
        }

        public VariableItem(string variableName, K2Field variableValue)
            : this()
        {
            this.Name = variableName;
            this.VariableValue = variableValue;
        }

        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_variableName))
                {
                    return string.Empty;
                }
                return _variableName;
            }
            set
            {
                base.OnNotifyPropertyChanged(VARIABLENAME, ref _variableName, value);
            }
        }


        public K2Field VariableValue
        {
            get
            {
                if (_variableValue == null)
                {
                    _variableValue = new K2Field(this);
                }
                return _variableValue;
            }
            set
            {
                base.OnNotifyPropertyChanged<K2Field>(VARIABLEVALUE, ref _variableValue, value);
            }
        }


        protected override void OnSave(Framework.ISerializationInfo content)
        {
            base.OnSave(content);
            if (!K2Field.IsNullOrEmpty(_variableValue))
            {
                content.SetProperty(VARIABLEVALUE, _variableValue);
            }
            if (!string.IsNullOrEmpty(_variableName))
            {
                content.SetProperty(VARIABLENAME, _variableName);
            }


        }

        protected override void OnLoad(Framework.ISerializationInfo content)
        {
            base.OnLoad(content);

            if (content.HasProperty(VARIABLEVALUE))
            {
                this.VariableValue = (K2Field)content.GetPropertyAsPersistableObject(VARIABLEVALUE);
            }

            if (content.HasProperty(VARIABLENAME))
            {
                this.Name = content.GetPropertyAsString(VARIABLENAME);
            }

        }


        public override T Clone<T>()
        {
            VariableItem item = base.Clone<VariableItem>();

            //TODO: K2Field.isnullorempty?
            if (this.VariableValue != null)
            {
                item.VariableValue = this.VariableValue.Clone<K2Field>();
            }

            if (!string.IsNullOrEmpty(this.Name))
            {
                item.Name = this.Name;
            }

            return item as T;
        }


        protected override System.Collections.IEnumerable ContainedList
        {
            get
            {
                if (base.Container != null)
                {
                    if (base.Container is PowerShellEventItem)
                    {
                        return (base.Container as PowerShellEventItem).InputVariables;
                    }

                }
                return null;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_variableValue != null)
                {
                    _variableValue.Dispose();
                }
            }
            _variableName = null;
            _variableValue = null;


            base.Dispose(disposing);
        }




    }
}
