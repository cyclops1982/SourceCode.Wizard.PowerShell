using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SourceCode.Framework;
using System.Collections.Specialized;

namespace SourceCode.Wizard.PowerShell.Design
{
    public class VariableItemCollection : PersistableObjectCollectionBase<PowerShellEventItem, VariableItem>, INotifyCollectionChanged
    {
        public VariableItemCollection(PowerShellEventItem parent)
            : base(parent)
        {
        }

        public override void Insert(int index, VariableItem item)
        {
            base.Insert(index, item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        public override int Add(VariableItem item)
        {
            int ret = base.Add(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            return ret;
        }

        public override void Remove(VariableItem item)
        {
            base.Remove(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
        }

        protected override int GetItemIndexByName(string name)
        {
            for (int i = 0; i < base.InnerList.Count; i++)
            {
                VariableItem item = (VariableItem)base.InnerList[i];
                if (String.Compare(item.Name, name, true) == 0)
                {
                    return i;
                }
            }
            return -1;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.InnerList != null)
                {
                    foreach (VariableItem item in this.InnerList)
                    {
                        if (item != null)
                        {
                            item.Dispose();
                        }
                    }
                    this.InnerList.Clear();
                }
            }
            base.Dispose(disposing);
        }


        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, e);
            }
        }

        #endregion

    }
}
