using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Grinin.TaskPlanner.Domain.Models;
using Grinin.TaskPlanner.DataAccess.Abstractions;
using Newtonsoft.Json;


namespace Grinin.TaskPlanner.DataAccess
{
    public class FileWorkItemsRepository : IWorkItemsRepository
    {
        private const string FileName = "work-items.json";
        private readonly Dictionary<Guid, WorkItem> _workItems;

        public FileWorkItemsRepository()
        {
            _workItems = LoadDataFromFile();
        }

        public Guid Add(WorkItem workItem)
        {
            var clonedWorkItem = workItem.Clone();
            _workItems.Add(clonedWorkItem.Id, clonedWorkItem);
            SaveChanges();
            return clonedWorkItem.Id;
        }

        public WorkItem Get(Guid id)
        {
            return _workItems.TryGetValue(id, out var workItem) ? workItem : null;
        }

        public WorkItem[] GetAll()
        {
            return _workItems.Values.ToArray();
        }

        public bool Update(WorkItem workItem)
        {
            if (_workItems.ContainsKey(workItem.Id))
            {
                _workItems[workItem.Id] = workItem.Clone();
                SaveChanges();
                return true;
            }
            return false;
        }

        public bool Remove(Guid id)
        {
            if (_workItems.ContainsKey(id))
            {
                _workItems.Remove(id);
                SaveChanges();
                return true;
            }
            return false;
        }

        public void SaveChanges()
        {
            var jsonData = JsonConvert.SerializeObject(_workItems.Values);
            File.WriteAllText(FileName, jsonData);
        }

        private Dictionary<Guid, WorkItem> LoadDataFromFile()
        {
            if (File.Exists(FileName))
            {
                var jsonData = File.ReadAllText(FileName);
                var workItems = JsonConvert.DeserializeObject<List<WorkItem>>(jsonData);
                return workItems?.ToDictionary(item => item.Id) ?? new Dictionary<Guid, WorkItem>();
            }
            return new Dictionary<Guid, WorkItem>();
        }
    }
}
