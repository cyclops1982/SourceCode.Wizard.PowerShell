using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SourceCode.Wizard.PowerShell.Design;

namespace SourceCode.Wizard.PowerShell.Wizard.Pages
{
    public class OutputVariables : Variables
    {

        public OutputVariables(PowerShellEvent theEvent) : base(theEvent)
        {
            TitleBarContent = "Output variables";
            InfoBarContent = "Define output variables to get values from your powershell script into K2 files.";
            VariableIsInputVariable = false;
        }


                //This event is called when the Page is loaded.
        protected override bool OnActivate()
        {
            VariableCollection = (base.DataObject as PowerShellEvent).EventItem.OutputVariables;
            lvVariables.ItemsSource = VariableCollection;

            return true;
        }

        //This event gets called when the page gets unloaded
        protected override bool OnDeactivate()
        {
            (base.DataObject as PowerShellEvent).EventItem.OutputVariables = VariableCollection;
            return true;
        }
    }
}
