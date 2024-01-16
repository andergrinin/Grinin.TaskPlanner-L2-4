using System;

namespace Grinin.TaskPlanner.Domain.Models
{
    public enum Priority
    {
        None,
        Low,
        Medium,
        High,
        Urgent
    }

    public enum Complexity
    {
        None,
        Minutes,
        Hours,
        Days,
        Weeks
    }

    public class WorkItem
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime CreationDate { get; set; }
        public DateTime DueDate { get; set; }
        public Priority Priority { get; set; }
        public Complexity Complexity { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }

        public WorkItem Clone()
        {
            return new WorkItem
            {
                CreationDate = this.CreationDate,
                DueDate = this.DueDate,
                Priority = this.Priority,
                Complexity = this.Complexity,
                Title = this.Title,
                Description = this.Description,
                IsCompleted = this.IsCompleted
            };
        }

        public override string ToString()
        {
            return $"{Title}: due {DueDate.ToString("dd.MM.yyyy")}, {Priority.ToString().ToLower()} priority";
        }
    }
}
