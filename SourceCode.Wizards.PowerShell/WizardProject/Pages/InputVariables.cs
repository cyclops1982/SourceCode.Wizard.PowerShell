using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SourceCode.Wizard.PowerShell.Design;

namespace SourceCode.Wizard.PowerShell.Wizard.Pages
{
    public class InputVariables : Variables
    {

        public InputVariables(PowerShellEvent theEvent)
            : base(theEvent)
        {
            TitleBarContent = "Input variables";
            InfoBarContent = "Define input variables to use in your powershell script.";
            VariableIsInputVariable = true;
        }


                //This event is called when the Page is loaded.
        protected override bool OnActivate()
        {
            VariableCollection = (base.DataObject as PowerShellEvent).EventItem.InputVariables;
            lvVariables.ItemsSource = VariableCollection;

            return true;
        }

        //This event gets called when the page gets unloaded
        protected override bool OnDeactivate()
        {
            (base.DataObject as PowerShellEvent).EventItem.InputVariables = VariableCollection;
            return true;
        }
    }
}
