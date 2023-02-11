using System;
using System.ComponentModel;
using System.Transactions;
using System.Windows.Input;
using System.Windows.Markup;

namespace TrainingPlanner
{
    public partial class MainWindowViewModel : INotifyPropertyChanged
    {
        public MainWindowViewModel()
        {
            CreateCommand = new RelayCommand(OnCreate);
            EditCommand = new RelayCommand(OnEdit);
            CancelCommand = new RelayCommand(OnCancel);
            SaveCommand = new RelayCommand(OnSave);

            EditMode = false;
            CurrentWindowView = WindowView.Weekview;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        private WindowView currentWindowView;
        public WindowView CurrentWindowView
        {
            get => currentWindowView;
            set
            {
                currentWindowView = value;
                this.NotifyPropertyChanged(nameof(CurrentWindowView));
            }
        }

        private bool showCancelButton;
        public bool ShowCancelButton
        {
            get => showCancelButton;
            set
            {
                showCancelButton = value;
                this.NotifyPropertyChanged(nameof(ShowCancelButton));
            }
        }

        private bool showSaveButton;
        public bool ShowSaveButton
        {
            get => showSaveButton;
            set
            {
                showSaveButton = value;
                this.NotifyPropertyChanged(nameof(ShowSaveButton));
            }
        }

        private bool showCreateButton;
        public bool ShowCreateButton
        {
            get => showCreateButton;
            set
            {
                showCreateButton = value;
                this.NotifyPropertyChanged(nameof(ShowCreateButton));
            }
        }

        private bool editMode;
        public bool EditMode
        {
            get => editMode;
            set
            {
                editMode = value;
                SetButtonVisibility();
                this.NotifyPropertyChanged(nameof(EditMode));
            }
        }

        private void SetButtonVisibility()
        {
            if (CurrentWindowView == WindowView.Weekview)
            {
                ShowCancelButton = false;
                ShowSaveButton = false;

                ShowCreateButton = true;
            }
            else
            {
                ShowCancelButton = true;
                ShowSaveButton = true;

                ShowCreateButton = false;
            }
        }

        public ICommand CreateCommand { get; set; }

        public void OnCreate()
        {
            CurrentWindowView = WindowView.AddEditview;
            EditMode = true;
        }

        public ICommand EditCommand { get; set; }

        public void OnEdit()
        {
            CurrentWindowView = WindowView.AddEditview;
            EditMode = true;
        }

        public ICommand CancelCommand { get; set; }

        public void OnCancel()
        {
            CurrentWindowView = WindowView.Weekview;
            EditMode = false;
        }

        public ICommand SaveCommand { get; set; }

        public void OnSave()
        {
            CurrentWindowView = WindowView.Weekview;
            EditMode = false;
        }
    }
}