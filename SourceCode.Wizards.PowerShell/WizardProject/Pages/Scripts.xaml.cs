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
    public partial class Scripts : WizardPage
    {
        private ScriptItemCollection _scriptItems;


        public Scripts(PowerShellEvent theEvent)
            : base(theEvent)
        {
            InitializeComponent();

            _scriptItems = theEvent.EventItem.ScriptItems;
        }

        protected override bool OnValidate()
        {
            if (_scriptItems.Count == 0)
            {
                ShowK2Error(lvVariables, "You need to specify at least one script!");
                return false;
            }
 
            return true;
        }
    }
}
