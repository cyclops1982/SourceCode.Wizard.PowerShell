using System;
using System.Collections.Generic;
using System.Text;

using SourceCode.Workflow.Design;
using SourceCode.Framework;
using SourceCode.Wizard.PowerShell.Design.Properties;

namespace SourceCode.Wizard.PowerShell.Design
{
    public class PowerShellPropertyWizardDefinition : DefaultPropertyWizardDefinition
    {
        public override K2Image Image
        {
            get
            {
                return new K2Image(Resources.powershell_large);
            }
        }
    }
}
