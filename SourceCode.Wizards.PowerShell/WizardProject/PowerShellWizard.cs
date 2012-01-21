//////////////////////////////////////////////////////////////////////////
//																		//
//																		//
//																		//
//																		//
//																		//
//																		//
//////////////////////////////////////////////////////////////////////////
using System;
using System.Text;
using System.Xml;
using System.Drawing;

using SourceCode.Framework;
using SourceCode.Framework.Design;
using SourceCode.Workflow.Authoring;
using SourceCode.Workflow.Authoring.Design;
using SourceCode.Workflow.WizardFramework;
using SourceCode.Workflow.Design;
using SourceCode.Configuration;
using SourceCode.Workflow.Wizards;
using SourceCode.Wizard.PowerShell.Design;

namespace SourceCode.Wizard.PowerShell.Wizard
{
    [ConfigurationName("PowerShell Wizard")]
    [DisplayName("SourceCode.Wizard.PowerShell.Wizard.Properties.Resources", "PowerShellWizardName", typeof(PowerShellWizard))]
    [Description("SourceCode.Wizard.PowerShell.Wizard.Properties.Resources", "PowerShellWizardDescription", typeof(PowerShellWizard))]
    [ToolboxBitmap(typeof(PowerShellWizard), "Resources.powershell.small.ico")]
    public class PowerShellWizard : SourceCode.Workflow.WizardFramework.Wizard
    {

        public PowerShellWizard()
            : base()
        {
            base.Definition = new SourceCode.Wizard.PowerShell.Design.PowerShellWizardDefinition();
        }


        private void InitializeWizard(SourceCode.Framework.WizardInitializeArgs e)
        {
            PowerShellEvent eventObj = null;

            switch (base.Status)
            {
                case WizardStatus.New:
                case WizardStatus.NewDelayed:
                    eventObj = new PowerShellEvent();
                    eventObj.WizardDefinition = base.Definition;
                    SourceCode.Workflow.Wizards.WizardHelper.GetEventActivity(e.Parent).Events.Insert(e.InsertIndex, eventObj);
                    if (base.Status == WizardStatus.NewDelayed)
                    {
                        return;
                    }
                    break;
                case WizardStatus.Executed:
                case WizardStatus.Delayed:
                    if (e.Parent is PowerShellEvent)
                    {
                        eventObj = (PowerShellEvent)e.Parent;
                    }
                    break;
            }

            base.Pages.Add(new Pages.Start());
            base.Pages.Add(new Pages.InputVariables(eventObj));
            base.Pages.Add(new Pages.PowerShellScript(eventObj));
            base.Pages.Add(new Pages.OutputVariables(eventObj));
            base.Pages.Add(new Pages.Finish());
        }

 
        //This event is fired when the Wizard is dropped onto an activity/the canvas.
        public override void InitializeForNewExecution(WizardInitializeArgs e)
        {
            base.InitializeForNewExecution(e);
            InitializeWizard(e);
        }

        //This event is fired when the Ctrl button was held in during the drag operation
        public override void InitializeForNewDelayedExecution(WizardInitializeArgs e)
        {
            base.InitializeForNewDelayedExecution(e);
            InitializeWizard(e);
        }


        // no clue when this happens.
        public override void InitializeForDelayedExecution(WizardInitializeArgs e)
        {
            
            base.InitializeForDelayedExecution(e);
            InitializeWizard(e);
        }

        // Method is called when you re-execute the event. So when you click on the box next to the event name.
        public override void InitializeForReExecution(WizardInitializeArgs e)
        {
            base.InitializeForReExecution(e);
            InitializeWizard(e);
        }

        protected override bool OnFinish(IWizardHostService host)
        {
            bool retVal = base.OnFinish(host);

            WizardPage firstPageWithError = WizardHelper.GetErrorPage(this.Host, this.Pages);
            if (firstPageWithError != null)
            {
                if (host.CurrentWizard.CurrentPage != firstPageWithError)
                {
                    host.CurrentWizard.CurrentPage = firstPageWithError;
                }

                retVal = false;
            }
            return retVal;
        }


        protected override bool OnCanConfigureInstance(object parent)
        {
            //Can only be dropped on a Activity or Process
            if ((parent is Activity) || (parent is Process))
            {
                return true;
            }
            return false;
        }
    }
}
