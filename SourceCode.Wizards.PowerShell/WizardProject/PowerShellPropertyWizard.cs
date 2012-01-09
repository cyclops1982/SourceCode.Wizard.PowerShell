using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SourceCode.Workflow.WizardFramework;
using SourceCode.Configuration;
using SourceCode.Framework;
using System.Drawing;
using SourceCode.Framework.Design;
using SourceCode.Wizard.PowerShell.Design;
using SourceCode.Workflow.Authoring;

namespace SourceCode.Wizard.PowerShell.Wizard
{

    /// <summary>
    /// The propertywizard is the wizard that gets launched when you right click on the event and select properties.
    /// </summary>
    [ConfigurationName("PowerShell Property Wizard")]
    [DisplayName("SourceCode.Wizard.PowerShell.Wizard.Properties.Resources", "PowerShellPropertyWizardName", typeof(PowerShellPropertyWizard))]
    [Description("SourceCode.Wizard.PowerShell.Wizard.Properties.Resources", "PowerShellPropertyWizardDescription", typeof(PowerShellPropertyWizard))]
    public class PowerShellPropertyWizard : PropertyWizard
    {
        private PowerShellEvent _PowerShellEvent = null;

        public PowerShellPropertyWizard()
        {
            base.Definition = new SourceCode.Wizard.PowerShell.Design.PowerShellWizardDefinition();
        }

        public override void Initialize(WizardInitializeArgs e)
        {
            base.Initialize(e);
            _PowerShellEvent = (PowerShellEvent)e.Parent;

            base.Pages.Add(new Pages.InputVariables(_PowerShellEvent));
            base.Pages.Add(new Pages.PowerShellScript(_PowerShellEvent));
            base.Pages.Add(new Pages.OutputVariables(_PowerShellEvent));

        }

        protected override bool OnCanConfigureInstance(object parent)
        {
            if (parent is Activity)
            {
                return true;
            }
            return false;
        }
    }
}
