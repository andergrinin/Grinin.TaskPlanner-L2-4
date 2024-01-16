using Grinin.TaskPlanner.Domain.Models;
using System;

namespace Grinin.TaskPlanner.DataAccess.Abstractions
{
    public interface IWorkItemsRepository
{
    Guid Add(WorkItem workItem);
    WorkItem Get(Guid id);
    WorkItem[] GetAll();
    bool Update(WorkItem workItem);
    bool Remove(Guid id);
    void SaveChanges();
}
}
