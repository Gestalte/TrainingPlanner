namespace TrainingPlanner
{
    public class ExerciseItem
    {
        public ExerciseItem(string description, RelayCommand<object> removeCommand)
        {
            Description = description;
            RemoveCommand = removeCommand;

            Myself = this;
        }

        public ExerciseItem Myself { get; set; }
        public int ExerciseId { get; set; }
        public string Description { get; set; }
        public RelayCommand<object> RemoveCommand { get; set; }
    }
}