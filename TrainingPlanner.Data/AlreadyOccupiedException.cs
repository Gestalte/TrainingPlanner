using System.Runtime.Serialization;

namespace TrainingPlanner.Data
{
    [Serializable]
    public class AlreadyOccupiedException : Exception
    {
        public AlreadyOccupiedException()
        {            
        }

        public AlreadyOccupiedException(string? message) : base(message)
        {
        }

        public AlreadyOccupiedException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected AlreadyOccupiedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}