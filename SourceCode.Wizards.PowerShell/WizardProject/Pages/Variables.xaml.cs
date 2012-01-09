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
using System.Windows;
using System.Windows.Data;

namespace SourceCode.Wizard.PowerShell.Wizard.Pages
{
    public partial class Variables : WizardPage
    {

        private AssignVariable _dialogPage = null;
        private VariableItem _oldValue = null;

        private VariableItemCollection _variables;

        public Variables(PowerShellEvent theEvent)
            : base(theEvent)
        {
            InitializeComponent();

            _variables = theEvent.EventItem.OutputVariables;
            InitializeListView();
            SetRibbonButton();

        }


        protected VariableItemCollection VariableCollection
        {
            get
            {
                return _variables;
            }
            set
            {
                _variables = value;
            }
        }


        protected bool VariableIsInputVariable
        {
            get;
            set;
        }


        protected override bool OnValidate()
        {
            bool variableDouble = false;

            foreach (VariableItem item in _variables)
            {
                foreach (VariableItem item2 in _variables)
                {
                    if (item.Equals(item2) == false && string.Compare(item.Name, item2.Name, true) == 0)
                    {
                        variableDouble = true;
                        break;
                    }
                }
                if (variableDouble)
                {
                    break;
                }
            }

            if (variableDouble)
            {
                ShowK2Error(lvVariables, "A variable name can only be defined 1 time.");
                return false;
            }
            else
            {
                HideK2Error(lvVariables);
            }


            return true;
        }


        private void SetRibbonButton()
        {
            lvVariables.Items.Refresh();

            addRibbonButton.IsEnabled = assignRibbonButton.IsEnabled = removeAllRibbonButton.IsEnabled = removeRibbonButton.IsEnabled = true;

            if (lvVariables.Items.Count > 0)
            {
                if (lvVariables.SelectedItems.Count == 0)
                {
                    assignRibbonButton.IsEnabled = removeRibbonButton.IsEnabled = false;
                }
            }
            else
            {
                assignRibbonButton.IsEnabled = removeAllRibbonButton.IsEnabled = removeRibbonButton.IsEnabled = false;
            }

        }



        private void assignRibbonItem_Click(object sender, RoutedEventArgs e)
        {
            if (lvVariables.SelectedItem != null)
            {
                _oldValue = (lvVariables.SelectedItem as VariableItem);
                VariableItem item = _oldValue.Clone<VariableItem>();
                _variables.Remove(_oldValue);
                ModalDialog(item, false);
            }
            SetRibbonButton();
        }



        private void addRibbonItem_Click(object sender, RoutedEventArgs e)
        {
            this._oldValue = null;
            ModalDialog(new VariableItem(), true);
            SetRibbonButton();
        }

        private void removeRibbonItem_Click(object sender, RoutedEventArgs e)
        {
            if (lvVariables.SelectedItem != null)
            {
                _variables.Remove((lvVariables.SelectedItem as VariableItem));
            }
            SetRibbonButton();
        }

        private void removeAllRibbonItem_Click(object sender, RoutedEventArgs e)
        {
            _variables.Clear();
            SetRibbonButton();
        }

        private void lvVariables_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetRibbonButton();
        }

        private void lvVariables_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if ((e.Device.Target is TextBlock && (e.Device.Target as TextBlock).Parent is GridViewRowPresenter) ||
                (e.Device.Target is SourceCode.Workflow.WizardFramework.Controls.FieldPartVisual) ||
                e.Device.Target is Border)
            {
                this.assignRibbonItem_Click(null, null);
            }
            
        }

       
        private void ModalDialog(VariableItem item, bool add)
        {
            IModalWindow modalDialog = null;
            try
            {
                _dialogPage = new AssignVariable(item, base.Wizard.Host, VariableIsInputVariable);
                modalDialog = base.Wizard.Host.ShowModalWindow(new IWizardPage[] { _dialogPage }, new System.Drawing.Size((int)_dialogPage.Width, (int)_dialogPage.Height), false);
                modalDialog.ShowButton(ModalWindowButtonTypes.OK);
                modalDialog.ShowButton(ModalWindowButtonTypes.Cancel);
                modalDialog.OKClicked += new EventHandler<ModalWindowButtonClickEventArgs>(modalDialog_OKClicked);
                if (add)
                {
                    modalDialog.SetTitle("Add Variable");
                }
                else
                {
                    modalDialog.SetTitle("Assign Variable");
                    modalDialog.CancelClicked += new EventHandler<ModalWindowButtonClickEventArgs>(modalDialog_CancelClicked);
                }

                modalDialog.Show();
            }
            catch (Exception Ex)
            {
                ModalWindow.ShowMessage(base.Wizard.Host.WindowHandle, ModalWindowIconTypes.Error, "K2", Ex.Message.ToString(), new ModalWindowButtonTypes[] { ModalWindowButtonTypes.OK });
            }
            finally
            {
                if (_dialogPage != null)
                {
                    _dialogPage.Dispose();
                    _dialogPage = null;
                }

                if (modalDialog != null)
                {
                    modalDialog.OKClicked -= modalDialog_OKClicked;
                    if (!add)
                    {
                        modalDialog.CancelClicked -= modalDialog_CancelClicked;
                    }
                    modalDialog.Dispose();
                    modalDialog = null;
                }
            }

        }

        void modalDialog_CancelClicked(object sender, ModalWindowButtonClickEventArgs e)
        {
            _variables.Add(this._oldValue);
        }

        void modalDialog_OKClicked(object sender, ModalWindowButtonClickEventArgs e)
        {
            _variables.Add(_dialogPage.Variable);
        }



        private void InitializeListView()
        {
            try
            {
                lvVariables.ItemsSource = _variables;

                GridView view = (GridView)lvVariables.View;

                //TODO: Set Headers from resource file
                //view.Columns[0].Header = SourceCode.Extenders.WindowsDesigner.Resources.Resource.lblGridPage_lvSampleItemsName;
                //view.Columns[1].Header = SourceCode.Extenders.WindowsDesigner.Resources.Resource.lblGridPage_lvSampleItemsValue;


                FrameworkElementFactory fe = new FrameworkElementFactory(typeof(FieldPartRenderer));
                fe.SetValue(FieldPartRenderer.K2FieldProperty, new Binding("VariableValue"));

                DataTemplate dt = new DataTemplate(typeof(FieldPartRenderer));
                dt.VisualTree = fe;

                view.Columns[1].CellTemplate = dt;



                if (lvVariables.ItemContainerStyle == null)
                {
                    lvVariables.ItemContainerStyle = new Style(typeof(ListViewItem));
                    
                }

            }
            catch (Exception Ex)
            {
                ModalWindow.ShowMessage(base.Wizard.Host.WindowHandle, ModalWindowIconTypes.Error, "K2", Ex.Message.ToString(), new ModalWindowButtonTypes[] { ModalWindowButtonTypes.OK });
            }
        }

      


    }
}
