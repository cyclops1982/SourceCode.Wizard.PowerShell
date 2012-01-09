using System;
using System.Collections.Generic;
using System.Text;

using SourceCode.Workflow.Authoring;
using SourceCode.Workflow.Design;
using SourceCode.Framework;
using SourceCode.Wizard.PowerShell.Design.Properties;
using SourceCode.Framework.Design;

namespace SourceCode.Wizard.PowerShell.Design
{
    [Serializable]
    public class PowerShellEvent : Event
    {
        public PowerShellEvent()
            : base()
        {
            base.Type = EventTypes.Server;
            base.EventItem = new PowerShellEventItem();
        }

        public new PowerShellEventItem EventItem
        {
            get { return base.EventItem as PowerShellEventItem; }
            set { base.EventItem = value; }
        }

        public override K2Image Image
        {
            get
            {
                return new K2Image(Resources.powershell);
            }
        }
    }
}
