//////////////////////////////////////////////////////////////////////////
//																		//
//																		//
//																		//
//																		//
//																		//
//																		//
//////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;

using SourceCode.Workflow.Design;
using SourceCode.Workflow.Authoring;
using SourceCode.Framework;
using SourceCode.Wizard.PowerShell.Design.Properties;

namespace SourceCode.Wizard.PowerShell.Design
{
    public class PowerShellWizardDefinition : DefaultWizardDefinition
    {
        public override K2Image Image
        {
            get
            {
                return new K2Image(Resources.powershell_small);
            }
        }
    }
}
