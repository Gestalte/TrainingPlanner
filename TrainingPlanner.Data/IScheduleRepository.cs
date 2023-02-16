namespace TrainingPlanner.Data
{
    public interface IScheduleRepository
    {
        IEnumerable<Schedule> GetAll();
        void Add(Schedule schedule);
        void AddMultiple(Schedule[] schedules);
    }
}