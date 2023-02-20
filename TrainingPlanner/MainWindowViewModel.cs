using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.VisualBasic;
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
            this.scheduleBuilder.EditCommand = EditCommand;

            ExerciseItems = new ObservableCollection<ExerciseItem>();
            EditMode = false;
            CurrentWindowView = WindowView.Weekview;
            MainTitle = "Week view";
            AmpmSelection = TimeSlot.AM;

            SetWeekdayCheckboxesToFalse();
            WeekItems = this.scheduleBuilder.LoadSchedules();
        }

        [ObservableProperty]
        string title = "";

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

        [ObservableProperty]
        private bool itemCompleted;

        partial void OnWeekItemsChanged(List<WeekItem> value)
        {
            SetButtonVisibility();
        }

        [ObservableProperty]
        ObservableCollection<ExerciseItem> exerciseItems = new();

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
        public void RemoveExcerciseItem(object data)
        {
            ExerciseItems.Remove((ExerciseItem)data);
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
                EditSchedule();
            }
            else
            {
                CreateSchedules();
            }

            ResetWeekView();
        }

        [RelayCommand]
        public void Delete()
        {
            this.scheduleRepository.Delete(SelectedSchedule);

            SelectedSchedule = null!;

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

        public DateTime GetNextAvailableDate(WeekDay weekDay)
        {
            var currentDate = GetLastDate();

            // TODO: Use DayOfWeek instead of custom version.
            DayOfWeek day = currentDate.Date.DayOfWeek;

            int diff = (int)weekDay - (int)day;
            
            int daysToAdd = diff >= 0 
                ? diff 
                : 7 - Math.Abs(diff);

            return currentDate.AddDays(daysToAdd);
        }

        public DateTime GetLastDate()
        {
            var latestDate = this.scheduleRepository.GetLatestDate();

            if (latestDate < DateTime.Now)
            {
                latestDate = DateTime.Now;
            }

            return latestDate;
        }

        public void CreateSchedules()
        {
            List<Schedule> schedules = new List<Schedule>();

            for (int i = 0; i < NumberOfRepetitions; i++)
            {
                foreach (var day in GetCheckedDays())
                {
                    Schedule schedule = new()
                    {
                        Title = Title,
                        Weekday = (short)day,
                        Timeslot = (short)AmpmSelection,
                        IsComplete = false,
                        Date = GetNextAvailableDate(day),
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

        private WeekDay[] GetCheckedDays()
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

            return weekdays.Where(w => w.check).Select(s => s.day).ToArray();
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
            WeekItems = this.scheduleBuilder.LoadSchedules();

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