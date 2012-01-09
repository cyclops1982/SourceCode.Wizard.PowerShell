using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

using SourceCode.Framework;
using SourceCode.Framework.Design;
using SourceCode.Workflow.Design;
using SourceCode.Workflow.Authoring;
using SourceCode.Workflow.WizardFramework;
using SourceCode.Workflow.WizardFramework.Controls;
using SourceCode.Workflow.VisualDesigners.WizardHost;

using SourceCode.Wizard.PowerShell.Design;

namespace SourceCode.Wizard.PowerShell.Wizard.Pages
{
    public partial class AssignVariable : WizardPage
    {
        public AssignVariable(VariableItem variableItem, IWizardHostService host, bool variableIsInputVariable)
            : base(variableItem)
        {

            InitializeComponent();

            txtVariabelValue.ContextBrowserButton = btnContextBrowse2;
            txtVariabelValue.PluginContext = host.ServiceProvider;
            if (variableIsInputVariable)
            {
                txtVariabelValue.AddingFieldPart += new AddingPartHandler(addingFieldPart_CheckRead);
            }
            else
            {
                txtVariabelValue.AddingFieldPart += new AddingPartHandler(addingFieldPart_CheckWrite);

            }

        }


        bool addingFieldPart_CheckWrite(object sender, K2FieldPart part)
        {
            if (part.CanWrite)
            {
                return true;
            }
            return false;
        }

        bool addingFieldPart_CheckRead(object sender, K2FieldPart part)
        {
            if (part.CanRead)
            {
                return true;
            }
            return false;
        }


        public VariableItem Variable
        {
            get
            {
                return (base.DataObject as VariableItem);
            }
        }



        protected override bool OnActivate()
        {
            txtVariabelValue.K2Field = this.Variable.VariableValue;
            txtVariableName.Text = this.Variable.Name;

            return true;
        }


        protected override bool OnDeactivate()
        {
            this.Variable.Name = txtVariableName.Text;
            this.Variable.VariableValue = txtVariabelValue.K2Field;

            return true;
        }


        protected override bool OnValidate()
        {
            bool retval = true;
            if (string.IsNullOrEmpty(txtVariableName.Text))
            {
                ShowK2Error(txtVariableName, "Please enter a variable name");
                retval = false;
            }
            else
            {
                HideK2Error(txtVariableName);
            }

            if (txtVariabelValue.IsEmpty)
            {
                ShowK2Error(txtVariabelValue, "Please enter a variable value");
                retval = false;
            }
            else
            {
                HideK2Error(txtVariabelValue);
            }


            return retval;
        }
    }
}
