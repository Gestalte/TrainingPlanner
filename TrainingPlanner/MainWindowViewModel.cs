using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using TrainingPlanner.Data;

namespace TrainingPlanner
{
    public partial class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly IScheduleRepository scheduleRepository;

        public MainWindowViewModel(IScheduleRepository scheduleRepository)
        {
            this.scheduleRepository = scheduleRepository;

            CreateCommand = new RelayCommand(OnCreate);
            EditCommand = new RelayCommand<object>(OnEdit);
            CancelCommand = new RelayCommand(OnCancel);
            SaveCommand = new RelayCommand(OnSave);
            AddExerciseCommand = new RelayCommand(OnAddExercise);
            RemoveExcerciseItemCommand = new RelayCommand<object>(OnRemoveExcerciseItem);

            ExerciseItems = new ObservableCollection<ExerciseItem>();

            EditMode = false;
            CurrentWindowView = WindowView.Weekview;
            MainTitle = "Week view";
            AMPMSelection = TimeSlot.AM;

            ResetWeekdayCheckboxes();

            LoadSchedules();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        private string title = "";
        public string Title
        {
            get => title;
            set
            {
                title = value;
                this.NotifyPropertyChanged(nameof(Title));
            }
        }

        private int numberOfRepetitions;
        public int NumberOfRepetitions
        {
            get => numberOfRepetitions;
            set
            {
                numberOfRepetitions = value;
                this.NotifyPropertyChanged(nameof(NumberOfRepetitions));
            }
        }

        private TimeSlot ampmSelection;
        public TimeSlot AMPMSelection
        {
            get => ampmSelection;
            set
            {
                ampmSelection = value;
                this.NotifyPropertyChanged(nameof(AMPMSelection));
            }
        }

        private bool mondayChecked;
        public bool MondayChecked
        {
            get => mondayChecked;
            set
            {
                mondayChecked = value;
                this.NotifyPropertyChanged(nameof(MondayChecked));
            }
        }

        private bool tuesdayChecked;
        public bool TuesdayChecked
        {
            get => tuesdayChecked;
            set
            {
                tuesdayChecked = value;
                this.NotifyPropertyChanged(nameof(TuesdayChecked));
            }
        }

        private bool wednesdayChecked;
        public bool WednesdayChecked
        {
            get => wednesdayChecked;
            set
            {
                wednesdayChecked = value;
                this.NotifyPropertyChanged(nameof(WednesdayChecked));
            }
        }

        private bool thursdayChecked;
        public bool ThursdayChecked
        {
            get => thursdayChecked;
            set
            {
                thursdayChecked = value;
                this.NotifyPropertyChanged(nameof(ThursdayChecked));
            }
        }

        private bool fridayChecked;
        public bool FridayChecked
        {
            get => fridayChecked;
            set
            {
                fridayChecked = value;
                this.NotifyPropertyChanged(nameof(FridayChecked));
            }
        }

        private bool saturdayChecked;
        public bool SaturdayChecked
        {
            get => saturdayChecked;
            set
            {
                saturdayChecked = value;
                this.NotifyPropertyChanged(nameof(SaturdayChecked));
            }
        }

        private bool sundayChecked;
        public bool SundayChecked
        {
            get => sundayChecked;
            set
            {
                sundayChecked = value;
                this.NotifyPropertyChanged(nameof(SundayChecked));
            }
        }

        private string exerciseDescription = "";
        public string ExerciseDescription
        {
            get => exerciseDescription;
            set
            {
                exerciseDescription = value;
                this.NotifyPropertyChanged(nameof(ExerciseDescription));
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

        private ObservableCollection<ExerciseItem> exerciseItems = new();
        public ObservableCollection<ExerciseItem> ExerciseItems
        {
            get => exerciseItems;
            set
            {
                exerciseItems = value;
                this.NotifyPropertyChanged(nameof(ExerciseItems));
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
            EditMode = false;
        }

        public RelayCommand<object> EditCommand { get; set; }

        public void OnEdit(object data) // TODO: Change this to get Database id for editing.
        {
            MainTitle = "Edit item";
            CurrentWindowView = WindowView.AddEditview;
            EditMode = true;

            var selectedSchedule = this.scheduleRepository.GetById((int)data);

            if (selectedSchedule == null) return;

            Title = selectedSchedule?.Title ?? "";
            AMPMSelection = (TimeSlot)selectedSchedule.Timeslot;

            // TODO: Set radio group

            var exercises = selectedSchedule.Exercises.Select(s => new ExerciseItem(s.Description, RemoveExcerciseItemCommand));

            exercises.ToList().ForEach(f => ExerciseItems.Add(f));
        }

        public ICommand CancelCommand { get; set; }

        public void OnCancel()
        {
            ResetWeekView();
        }

        public ICommand SaveCommand { get; set; }

        public void OnSave()
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
                        Timeslot = (short)AMPMSelection,
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

        public ICommand AddExerciseCommand { get; set; }

        public void OnAddExercise()
        {
            ExerciseItems.Add(new ExerciseItem(ExerciseDescription, RemoveExcerciseItemCommand));

            ExerciseDescription = "";
        }

        public RelayCommand<object> RemoveExcerciseItemCommand { get; set; }

        public void OnRemoveExcerciseItem(object data)
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

            MainTitle = "Week view";
            CurrentWindowView = WindowView.Weekview;
            EditMode = false;
        }

        private void ClearAddEditScreen()
        {
            Title = "";
            NumberOfRepetitions = 0;
            AMPMSelection = TimeSlot.AM;
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