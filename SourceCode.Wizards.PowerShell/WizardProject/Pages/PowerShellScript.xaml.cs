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

using System.Management.Automation;

using SourceCode.Wizard.PowerShell.Design;

namespace SourceCode.Wizard.PowerShell.Wizard.Pages
{
    public partial class PowerShellScript : WizardPage
    {
        public PowerShellScript(PowerShellEvent theEvent)
            : base(theEvent)
        {
            InitializeComponent();
        }


        //This event is called when the Page is loaded.
        protected override bool OnActivate()
        {
            txtPowerShellScript.K2Field = (base.DataObject as PowerShellEvent).EventItem.PowerShellScript;


            return true;
        }

        //This event gets called when the page gets unloaded
        protected override bool OnDeactivate()
        {
            (base.DataObject as PowerShellEvent).EventItem.PowerShellScript = txtPowerShellScript.K2Field;

            return true;
        }

        //This event gets called when the Next/Finished button is clicked to 
        //validate that all required information has been entered into relevant areas
        protected override bool OnValidate()
        {
            HideK2Error(txtPowerShellScript);
            if (txtPowerShellScript.IsEmpty)
            {
                ShowK2Error(txtPowerShellScript, "Please enter a PowerShell Script");
                return false;
            }

            try
            {
                ScriptBlock sb = ScriptBlock.Create(txtPowerShellScript.K2Field.Text);
            }
            catch (ParseException ex)
            {
                ShowK2Error(txtPowerShellScript, string.Concat("Parse Exception:", Environment.NewLine, ex.Message));
                return false;
            }
            catch (Exception ex)
            {
                ShowK2Error(txtPowerShellScript, string.Concat("We tried to parse the script using a ScriptBlock, but this gave us an exception:", Environment.NewLine, ex.Message, Environment.NewLine, ex.StackTrace));
                return false;
            }


            //When all validations have been passed, return true so that OnDeactivate() override is initialized.
            return true;
        }
    }
}
