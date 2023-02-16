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

        public MainWindowViewModel(IScheduleRepository scheduleRepository)
        {
            title = "";
            exerciseDescription = "";
            exerciseItems = new ObservableCollection<ExerciseItem>();

            this.scheduleRepository = scheduleRepository;

            ExerciseItems = new ObservableCollection<ExerciseItem>();

            EditMode = false;
            CurrentWindowView = WindowView.Weekview;
            MainTitle = "Week view";
            AmpmSelection = TimeSlot.AM;
            selectedSchedule = null!;

            ResetWeekdayCheckboxes();

            LoadSchedules();
        }

        [ObservableProperty]
        string title;

        [ObservableProperty]
        int numberOfRepetitions;

        [ObservableProperty]
        TimeSlot ampmSelection;

        [ObservableProperty]
        WeekDay weekDaySelection;

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
        string exerciseDescription;

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
        Schedule selectedSchedule;

        [ObservableProperty]
        private List<WeekItem> weekItems = new();

        [ObservableProperty]
        private bool itemCompleted;

        partial void OnWeekItemsChanged(List<WeekItem> value)
        {
            SetButtonVisibility();
        }

        [ObservableProperty]
        ObservableCollection<ExerciseItem> exerciseItems;

        [ObservableProperty]
        private string mainTitle = "";

        partial void OnMainTitleChanged(string value)
        {
            SetButtonVisibility();
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

        [RelayCommand]
        public void Create()
        {
            CurrentWindowView = WindowView.AddEditview;
            MainTitle = "Create item";
            EditMode = false;
        }

        [RelayCommand]
        public void Edit(object data) // TODO: Change this to get Database id for editing.
        {
            CurrentWindowView = WindowView.AddEditview;
            MainTitle = "Edit item";
            EditMode = true;

            var selectedSchedule = this.scheduleRepository.GetById((int)data);

            if (selectedSchedule == null) return;

            SelectedSchedule = selectedSchedule;

            Title = selectedSchedule?.Title ?? "";
            AmpmSelection = (TimeSlot)selectedSchedule!.Timeslot;

            WeekDaySelection = (WeekDay)selectedSchedule.Weekday;

            var exercises = selectedSchedule.Exercises.Select(s => new ExerciseItem(s.Description, RemoveExcerciseItemCommand));

            exercises.ToList().ForEach(f => ExerciseItems.Add(f));

            ItemCompleted = selectedSchedule.IsComplete;
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
                EditSchedule();
            }
            else
            {
                CreateSchedules();
            }

            ResetWeekView();
        }

        private void EditSchedule()
        {
            SelectedSchedule.Title = Title;
            SelectedSchedule.Timeslot = (short)AmpmSelection;
            SelectedSchedule.Weekday = (short)WeekDaySelection;

            SelectedSchedule.Exercises = ExerciseItems.Select(s => new Exercise
            {
                Description = s.Description,
                ExerciseId = s.ExerciseId
            }).ToList();

            SelectedSchedule.IsComplete = ItemCompleted;

            this.scheduleRepository.Edit(SelectedSchedule);

            SelectedSchedule = null!;
        }

        public void CreateSchedules()
        {
            List<Schedule> schedules = new List<Schedule>();

            for (int i = 0; i < NumberOfRepetitions; i++)
            {
                foreach (var (day, _) in GetCheckedDays())
                {
                    Schedule schedule = new()
                    {
                        Title = Title,
                        Weekday = (short)day,
                        Timeslot = (short)AmpmSelection,
                        IsComplete = false,
                        Exercises = ExerciseItems.Select(s => new Exercise
                        {
                            Description = s.Description
                        }).ToList()
                    };

                    schedules.Add(schedule);
                }
            }

            this.scheduleRepository.AddMultiple(schedules.ToArray());
        }

        private (WeekDay day, bool check)[] GetCheckedDays()
        {
            var weekdays = new List<(WeekDay day, bool check)>
                {
                    (WeekDay.Monday, MondayChecked),
                    (WeekDay.Tuesday, TuesdayChecked),
                    (WeekDay.Wednesday, WednesdayChecked),
                    (WeekDay.Thursday, ThursdayChecked),
                    (WeekDay.Friday, FridayChecked),
                    (WeekDay.Saturday, SaturdayChecked),
                    (WeekDay.Sunday, SundayChecked),
                };

            return weekdays.Where(w => w.check).ToArray();
        }

        [RelayCommand]
        public void AddExercise()
        {
            ExerciseItems.Add(new ExerciseItem(ExerciseDescription, RemoveExcerciseItemCommand));

            ExerciseDescription = "";
        }

        [RelayCommand]
        public void RemoveExcerciseItem(object data)
        {
            ExerciseItems.Remove((ExerciseItem)data);
        }

        [RelayCommand]
        public void MarkComplete()
        {
            ItemCompleted = !ItemCompleted;
        }

        public void ResetWeekdayCheckboxes()
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
            LoadSchedules();

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
            ResetWeekdayCheckboxes();
            ExerciseDescription = "";
            ExerciseItems = new ObservableCollection<ExerciseItem>();
        }

        public void LoadSchedules()
        {
            Func<Schedule, WeekItem> makeWeekItem = s
                => new WeekItem(s.Title, s.ScheduleId, EditCommand, s.Exercises.Select(s => s.Description).ToList());

            var weekItemArr = new WeekItem[14];

            for (int i = 0; i < weekItemArr.Length; i++)
            {
                weekItemArr[i] = new WeekItem();
            }

            this.scheduleRepository.GetAll()
                .Where(w => w.IsComplete == false)
                .DistinctBy(d => (d.Weekday, d.Timeslot))
                .ToList()
                .ForEach(f => AddItem(ref weekItemArr, makeWeekItem(f), (WeekDay)f.Weekday, (TimeSlot)f.Timeslot));

            WeekItems = weekItemArr.ToList();
        }

        public WeekItem[] AddItem(ref WeekItem[] weekItems, WeekItem newItem, WeekDay weekDay, TimeSlot timeSlot)
        {
            int index = (int)timeSlot == 1 ? (int)weekDay : (int)weekDay + 7;

            weekItems[index] = newItem;

            return weekItems;
        }
    }
}