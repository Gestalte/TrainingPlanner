﻿namespace TrainingPlanner.Data
{
    public interface IScheduleRepository
    {
        IEnumerable<Schedule> GetAll();
    }
}