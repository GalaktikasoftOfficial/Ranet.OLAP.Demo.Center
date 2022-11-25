using System.ComponentModel;
using System.Windows.Input;
using Ranet.Olap.UI.Controls;

namespace PivotGrid
{
    public class WBCommandsPivotGrid : INotifyPropertyChanged
    {
        private UpdateablePivotGridControl PivotGridControl;

        public ICommand ToggleEnableCommand
        {
            get { return PivotGridControl != null ? PivotGridControl.WritebackSplitButton.ToggleEnableCommand : null; }
        }

        public ICommand ToggleCalculateAutomaticallyCommand
        {
            get
            {
                return PivotGridControl != null
                    ? PivotGridControl.WritebackSplitButton.ToggleCalculateAutomaticallyCommand
                    : null;
            }
        }

        public ICommand ChangesCalculateCommand
        {
            get
            {
                if (PivotGridControl != null)
                    return PivotGridControl.WritebackSplitButton.ChangesCalculateCommand;
                return null;
            }
        }

        public ICommand ChangesDiscardCommand
        {
            get
            {
                return PivotGridControl != null ? PivotGridControl.WritebackSplitButton.ChangesDiscardCommand : null;
            }
        }

        public ICommand ChangesPublishCommand
        {
            get
            {
                return PivotGridControl != null ? PivotGridControl.WritebackSplitButton.ChangesPublishCommand : null;
            }
        }

        public ICommand ChangesRollbackCommand
        {
            get
            {
                return PivotGridControl != null ? PivotGridControl.WritebackSplitButton.ChangesRollbackCommand : null;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void SetControl(UpdateablePivotGridControl control)
        {
            PivotGridControl = control;
            UpdateStatePane();
        }


        private void UpdateStatePane()
        {
            OnPropertyChanged("ToggleEnableCommand");
            OnPropertyChanged("ToggleCalculateAutomaticallyCommand");
            OnPropertyChanged("ChangesCalculateCommand");
            OnPropertyChanged("ChangesDiscardCommand");
            OnPropertyChanged("ChangesPublishCommand");
            OnPropertyChanged("ChangesRollbackCommand");
        }

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}