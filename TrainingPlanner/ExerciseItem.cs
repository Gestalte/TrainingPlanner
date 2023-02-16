using CommunityToolkit.Mvvm.Input;

namespace TrainingPlanner
{
    public class ExerciseItem
    {
        public ExerciseItem(string description, IRelayCommand<object> removeCommand)
        {
            Description = description;
            RemoveCommand = removeCommand;

            Myself = this;
        }

        public ExerciseItem Myself { get; set; }
        public int ExerciseId { get; set; }
        public string Description { get; set; }
        public IRelayCommand<object> RemoveCommand { get; set; }
    }
}