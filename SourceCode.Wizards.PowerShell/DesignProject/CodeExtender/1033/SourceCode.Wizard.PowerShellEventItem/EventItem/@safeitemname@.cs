using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Drawing;
using SourceCode.KO;
using SourceCode.Workflow.Common.Extenders;
using hostContext = $contexttype$;

using System.Management.Automation;
using System.Collections.ObjectModel;
using System.Text;

namespace $rootnamespace$
{
    public partial class $safeitemname$ : ICodeExtender<hostContext>
	{
        #region ICodeExtender<hostContext> Members

        // NOTICE: This code can be edited if needed. The code inside the InputMap and OutputMap methods WILL be overwriten when you do anything to the process!!
        public void Main($contexttype$ K2)
        {
            using (PowerShell ps = PowerShell.Create())
            {
                InputMap(K2, ps.Runspace);
                ps.Runspace.SessionStateProxy.SetVariable("K2", K2);

                ps.AddScript(K2.Configuration.PowerShellScript);
                Collection<PSObject> results = ps.Invoke();
                if (ps.Streams.Error.Count != 0)
                {
                    
                    StringBuilder errorBuilder = new StringBuilder();
                    int i = 0;
                    foreach (ErrorRecord error in ps.Streams.Error)
                    {
                        errorBuilder.AppendFormat("Error {0}:", i++);
                        if (error.ErrorDetails != null)
                        {
                            errorBuilder.AppendFormat("Error.ErrorDetails.Message: {0}", error.ErrorDetails.Message);
                            errorBuilder.AppendFormat("Error.ErrorDetails.RecommandedAction: {0}", error.ErrorDetails.RecommendedAction);
                        }
                        errorBuilder.AppendFormat("Error.Exception.Message: {0}", error.Exception.Message);
                    }
                    throw new Exception(errorBuilder.ToString());
                }
                OutputMap(K2, ps.Runspace);
            }
        }
        #endregion
    }
} 
