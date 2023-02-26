using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using TrainingPlanner.Data;

namespace TrainingPlanner
{
    public partial class MainWindowViewModel : ObservableObject, INotifyPropertyChanged
    {
        private readonly IScheduleRepository scheduleRepository;
        private readonly IScheduleBuilder scheduleBuilder;

        public MainWindowViewModel(IScheduleRepository scheduleRepository, IScheduleBuilder scheduleBuilder)
        {
            this.scheduleRepository = scheduleRepository;
            this.scheduleBuilder = scheduleBuilder;

            ExerciseItems = new ObservableCollection<ExerciseItem>();
            EditMode = false;
            CurrentWindowView = WindowView.Weekview;
            MainTitle = "Week view";
            AmpmSelection = TimeSlot.AM;

            SetWeekdayCheckboxesToFalse();

            FillWeekItems();
        }

        [ObservableProperty]
        string title = "";

        [ObservableProperty]
        int numberOfRepetitions;

        [ObservableProperty]
        TimeSlot ampmSelection;

        [ObservableProperty]
        DayOfWeek weekDaySelection;

        [ObservableProperty]
        bool mondayChecked;

        [ObservableProperty]
        bool tuesdayChecked;

        [ObservableProperty]
        bool wednesdayChecked;

        [ObservableProperty]
        bool thursdayChecked;

        [ObservableProperty]
        bool fridayChecked;

        [ObservableProperty]
        bool saturdayChecked;

        [ObservableProperty]
        bool sundayChecked;

        [ObservableProperty]
        string exerciseDescription = "";

        [ObservableProperty]
        WindowView currentWindowView;

        [ObservableProperty]
        bool showCancelButton;

        [ObservableProperty]
        bool showSaveButton;

        [ObservableProperty]
        bool showCreateButton;

        [ObservableProperty]
        bool editMode;

        [ObservableProperty]
        Schedule selectedSchedule = null!;

        [ObservableProperty]
        private List<WeekItem> weekItems = new();

        partial void OnWeekItemsChanged(List<WeekItem> value)
        {
            SetButtonVisibility();
        }

        [ObservableProperty]
        private bool itemCompleted;

        [ObservableProperty]
        ObservableCollection<ExerciseItem> exerciseItems = new();

        [ObservableProperty]
        private string mainTitle = "";

        partial void OnMainTitleChanged(string value)
        {
            SetButtonVisibility();
        }

        [RelayCommand]
        public void Create()
        {
            CurrentWindowView = WindowView.AddEditview;
            MainTitle = "Create item";
            EditMode = false;
        }

        [RelayCommand]
        public void RemoveExcerciseItem(object data)
        {
            ExerciseItems.Remove((ExerciseItem)data);
        }

        [RelayCommand]
        public void Edit(object data)
        {
            CurrentWindowView = WindowView.AddEditview;
            MainTitle = "Edit item";
            EditMode = true;

            var selectedSchedule = this.scheduleRepository.GetById((int)data);

            if (selectedSchedule == null) return;

            SelectedSchedule = selectedSchedule;

            Title = selectedSchedule?.Title ?? "";
            AmpmSelection = (TimeSlot)selectedSchedule!.Timeslot;
            WeekDaySelection = (DayOfWeek)selectedSchedule.Weekday;
            ItemCompleted = selectedSchedule.IsComplete;

            selectedSchedule.Exercises
                .Select(s => new ExerciseItem(s.Description, s.ExerciseId, RemoveExcerciseItemCommand))
                .ToList()
                .ForEach(f => ExerciseItems.Add(f));
        }

        [RelayCommand]
        public void Cancel()
        {
            ResetWeekView();
        }

        [RelayCommand]
        public void Save()
        {
            if (EditMode)
            {
                // TODO: Figure out how to do edits now that dates are added.
                // NOTE: Maybe don't allow changing the date?
                this.scheduleBuilder.EditSchedule(Title, (short)AmpmSelection, (short)WeekDaySelection, ExerciseItems.ToList(), ItemCompleted);

                SelectedSchedule = null!;
            }
            else
            {
                this.scheduleBuilder.CreateSchedules
                    (NumberOfRepetitions
                    , GetCheckedDays().ToList()
                    , Title
                    , (short)AmpmSelection
                    , ExerciseItems.ToList()
                    );
            }

            ResetWeekView();
        }

        [RelayCommand]
        public void Delete()
        {
            this.scheduleBuilder.DeleteSchedule(SelectedSchedule);

            SelectedSchedule = null!;

            ResetWeekView();
        }

        [RelayCommand]
        public void AddExercise()
        {
            ExerciseItems.Add(new ExerciseItem(ExerciseDescription, RemoveExcerciseItemCommand));

            ExerciseDescription = "";
        }

        [RelayCommand]
        public void MarkComplete()
        {
            ItemCompleted = !ItemCompleted;
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

        private void FillWeekItems()
        {
            Func<Schedule, WeekItem> makeWeekItem = (s)
                => new WeekItem(s.Title, s.ScheduleId, EditCommand, s.Exercises.Select(s => s.Description).ToList());

            if (WeekItems == null)
            {
                WeekItems = new WeekItem[14].ToList();
            }

            this.scheduleBuilder.LoadSchedules()
                .ForEach(f => AddItem(makeWeekItem(f), (DayOfWeek)f.Weekday, (TimeSlot)f.Timeslot));
        }

        private List<WeekItem> AddItem(WeekItem newItem, DayOfWeek weekDay, TimeSlot timeSlot)
        {
            int index = (int)timeSlot == 1 ? (int)weekDay : (int)weekDay + 7;

            WeekItems[index] = newItem;

            return WeekItems;
        }

        private DayOfWeek[] GetCheckedDays()
        {
            var weekdays = new List<(DayOfWeek day, bool check)>
            {
                (DayOfWeek.Sunday, SundayChecked),
                (DayOfWeek.Monday, MondayChecked),
                (DayOfWeek.Tuesday, TuesdayChecked),
                (DayOfWeek.Wednesday, WednesdayChecked),
                (DayOfWeek.Thursday, ThursdayChecked),
                (DayOfWeek.Friday, FridayChecked),
                (DayOfWeek.Saturday, SaturdayChecked),
            };

            return weekdays.Where(w => w.check).Select(s => s.day).ToArray();
        }

        public void SetWeekdayCheckboxesToFalse()
        {
            MondayChecked = false;
            TuesdayChecked = false;
            WednesdayChecked = false;
            ThursdayChecked = false;
            FridayChecked = false;
            SaturdayChecked = false;
            SundayChecked = false;
        }

        public void ResetWeekView()
        {
            FillWeekItems();

            ClearAddEditScreen();

            CurrentWindowView = WindowView.Weekview;
            MainTitle = "Week view";
            EditMode = false;
        }

        private void ClearAddEditScreen()
        {
            Title = "";
            NumberOfRepetitions = 0;
            AmpmSelection = TimeSlot.AM;
            SetWeekdayCheckboxesToFalse();
            ExerciseDescription = "";
            ExerciseItems = new ObservableCollection<ExerciseItem>();
        }
    }
}