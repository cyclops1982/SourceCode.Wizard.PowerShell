using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SourceCode.Framework;
using System.Collections.Specialized;

namespace SourceCode.Wizard.PowerShell.Design
{
    public class ScriptItemCollection : PersistableObjectCollectionBase<PowerShellEventItem, ScriptItem>, INotifyCollectionChanged
    {
        public ScriptItemCollection(PowerShellEventItem parent) : base(parent) { }
        protected override void Dispose(bool disposing)
        {
            //TODO!!!!
            base.Dispose(disposing);
        }


        public override void Insert(int index, ScriptItem item)
        {
            base.Insert(index, item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));

        }


        public override int Add(ScriptItem item)
        {
            int ret = base.Add(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            return ret;
        }

        public override void Remove(ScriptItem item)
        {
            base.Remove(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
        }



        protected override int GetItemIndexByName(string name)
        {
            // We don't have a name.
            return 0;
        }

        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, e);
            }
        }

        #endregion


    }
}
