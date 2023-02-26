namespace TrainingPlanner.Data
{
    public interface IScheduleRepository
    {
        IEnumerable<Schedule> GetAll();
        Schedule? GetById(int Id);
        void Add(Schedule schedule);
        void AddMultiple(Schedule[] schedules);
        void Edit(Schedule selectedSchedule);
        void Delete(Schedule selectedSchedule);
        DateTime GetLatestDate(short timeslot, short weekday);
    }
}