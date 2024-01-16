using Grinin.TaskPlanner.Domain.Models;
using Grinin.TaskPlanner.DataAccess.Abstractions;
using System.Linq;

namespace Grinin.TaskPlanner.Domain.Logic
{
    public class SimpleTaskPlanner
    {
        private readonly IWorkItemsRepository workItemsRepository;

        public SimpleTaskPlanner(IWorkItemsRepository workItemsRepository)
        {
            this.workItemsRepository = workItemsRepository;
        }

        public WorkItem[] CreatePlan()
        {
            var workItems = workItemsRepository.GetAll();

            var nonCompletedItems = workItems.Where(item => !item.IsCompleted)
                                             .OrderByDescending(item => item.Priority) // Change to OrderByDescending
                                             .ThenBy(item => item.DueDate)
                                             .ThenBy(item => item.Title)
                                             .ToArray();

            return nonCompletedItems;
        }

    }

}
