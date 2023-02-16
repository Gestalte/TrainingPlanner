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
    public partial class MainWindowViewModel:ObservableObject,INotifyPropertyChanged
    {
        private readonly IScheduleRepository scheduleRepository;

        public MainWindowViewModel(IScheduleRepository scheduleRepository)
        {
            this.scheduleRepository = scheduleRepository;

            ExerciseItems = new ObservableCollection<ExerciseItem>();

            EditMode = false;
            CurrentWindowView = WindowView.Weekview;
            MainTitle = "Week view";
            AmpmSelection = TimeSlot.AM;

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

        private List<WeekItem> weekItems = new();
        public List<WeekItem> WeekItems
        {
            get => weekItems;
            set
            {
                weekItems = value;
                SetButtonVisibility();

                base.OnPropertyChanged(nameof(weekItems));
            }
        }

        [ObservableProperty]
        ObservableCollection<ExerciseItem> exerciseItems;

        private string mainTitle = "";
        public string MainTitle
        {
            get => mainTitle;
            set
            {
                mainTitle = value;
                SetButtonVisibility();
                base.OnPropertyChanged(nameof(MainTitle));
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

            Title = selectedSchedule?.Title ?? "";
            AmpmSelection = (TimeSlot)selectedSchedule.Timeslot;

            // TODO: Set radio group

            var exercises = selectedSchedule.Exercises.Select(s => new ExerciseItem(s.Description, RemoveExcerciseItemCommand));

            exercises.ToList().ForEach(f => ExerciseItems.Add(f));
        }

        [RelayCommand]
        public void Cancel()
        {
            ResetWeekView();
        }

        [RelayCommand]
        public void Save()
        {
            if (editMode)
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
            // TODO: swap out check boxes for radio group when editing.

            throw new NotImplementedException();
        }

        public void CreateSchedules()
        {
            List<Schedule> schedules = new List<Schedule>();

            for (int i = 0; i < numberOfRepetitions; i++)
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

                    //this.scheduleRepository.Add(schedule);

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