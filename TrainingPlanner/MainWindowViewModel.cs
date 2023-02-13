﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace TrainingPlanner
{
    public partial class MainWindowViewModel : INotifyPropertyChanged
    {
        public MainWindowViewModel()
        {
            CreateCommand = new RelayCommand(OnCreate);
            EditCommand = new RelayCommand<object>(OnEdit);
            CancelCommand = new RelayCommand(OnCancel);
            SaveCommand = new RelayCommand(OnSave);

            EditMode = false;
            CurrentWindowView = WindowView.Weekview;
            MainTitle = "Week view";

            var weekItemArr = new WeekItem[14];

            for (int i = 0; i < weekItemArr.Length; i++)
            {
                weekItemArr[i] = new WeekItem();
            }

            var n1 = new WeekItem("Test 1", EditCommand, new List<string>
                {
                    "Item 1",
                    "Item 2"
                });

            var n2 = new WeekItem("Test 2", EditCommand, new List<string>
                {
                    "Item 1",
                    "Item 2",
                    "Item 3"
                });

            AddItem(ref weekItemArr, n1, WeekDay.Monday, TimeSlot.AM);
            AddItem(ref weekItemArr, n2, WeekDay.Friday, TimeSlot.PM);

            WeekItems = weekItemArr.ToList();
        }

        public WeekItem[] AddItem(ref WeekItem[] weekItems, WeekItem newItem, WeekDay weekDay, TimeSlot timeSlot)
        {
            int index = (int)timeSlot == 1 ? (int)weekDay : (int)weekDay + 7;

            newItem.Param = (weekDay, timeSlot);

            weekItems[index] = newItem;

            return weekItems;
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

        private List<WeekItem> weekItems = new();
        public List<WeekItem> WeekItems
        {
            get => weekItems;
            set
            {
                weekItems = value;
                SetButtonVisibility();
                this.NotifyPropertyChanged(nameof(WeekItems));
            }
        }

        private string mainTitle = "";
        public string MainTitle
        {
            get => mainTitle;
            set
            {
                mainTitle = value;
                SetButtonVisibility();
                this.NotifyPropertyChanged(nameof(MainTitle));
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
            MainTitle = "Create item";
            CurrentWindowView = WindowView.AddEditview;
            EditMode = true;
        }

        public RelayCommand<object> EditCommand { get; set; }

        public void OnEdit(object data) // TODO: Change this to get Database id for editing.
        {
            MainTitle = "Edit item";
            CurrentWindowView = WindowView.AddEditview;
            EditMode = true;
        }

        public ICommand CancelCommand { get; set; }

        public void OnCancel()
        {
            MainTitle = "Week view";
            CurrentWindowView = WindowView.Weekview;
            EditMode = false;
        }

        public ICommand SaveCommand { get; set; }

        public void OnSave()
        {
            MainTitle = "Week view";
            CurrentWindowView = WindowView.Weekview;
            EditMode = false;
        }
    }
}