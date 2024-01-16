using Grinin.TaskPlanner.DataAccess;
using Grinin.TaskPlanner.DataAccess.Abstractions;
using Grinin.TaskPlanner.Domain.Logic;
using Grinin.TaskPlanner.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;

internal static class Program
{
    private static readonly IServiceProvider serviceProvider;

    static Program()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IWorkItemsRepository, FileWorkItemsRepository>();
        services.AddScoped<SimpleTaskPlanner>();

        serviceProvider = services.BuildServiceProvider();
    }

    public static void Main(string[] args)
    {
        Console.WriteLine("Task Planner Console App");

        while (true)
        {
            DisplayMenu();

            var choice = Console.ReadLine()?.ToUpper();

            switch (choice)
            {
                case "A":
                    AddWorkItem();
                    break;
                case "B":
                    BuildPlan();
                    break;
                case "M":
                    MarkAsCompleted();
                    break;
                case "R":
                    RemoveWorkItem();
                    break;
                case "Q":
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }

    private static void DisplayMenu()
    {
        Console.WriteLine("Menu:");
        Console.WriteLine("[A]dd work item");
        Console.WriteLine("[B]uild a plan");
        Console.WriteLine("[M]ark work item as completed");
        Console.WriteLine("[R]emove a work item");
        Console.WriteLine("[Q]uit the app");
        Console.Write("Enter your choice: ");
    }

    private static void AddWorkItem()
    {
        Console.WriteLine("Enter details for the new work item:");

        // Gather user input
        Console.Write("Title: ");
        string title = Console.ReadLine();

        Console.Write("Description: ");
        string description = Console.ReadLine();

        Console.Write("Priority (None/Low/Medium/High/Urgent): ");
        if (Enum.TryParse<Priority>(Console.ReadLine(), true, out var priority))
        {
            Console.Write("Complexity (None/Minutes/Hours/Days/Weeks): ");
            if (Enum.TryParse<Complexity>(Console.ReadLine(), true, out var complexity))
            {
                Console.Write("Due Date (dd.MM.yyyy): ");
                if (DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dueDate))
                {
                    // Create a new WorkItem with user input
                    var workItem = new WorkItem
                    {
                        Title = title,
                        Description = description,
                        Priority = priority,
                        Complexity = complexity,
                        DueDate = dueDate,
                        IsCompleted = false // Assuming a new work item is not completed by default
                    };

                    // Add the work item to the repository
                    using (var scope = serviceProvider.CreateScope())
                    {
                        var workItemsRepository = scope.ServiceProvider.GetRequiredService<IWorkItemsRepository>();
                        var id = workItemsRepository.Add(workItem);
                        Console.WriteLine($"Work item added with ID: {id}");
                    }


                    Console.WriteLine("Work item added successfully.");
                }
                else
                {
                    Console.WriteLine("Invalid date format. Work item not added.");
                }
            }
            else
            {
                Console.WriteLine("Invalid complexity. Work item not added.");
            }
        }
        else
        {
            Console.WriteLine("Invalid priority. Work item not added.");
        }
    }


    private static void BuildPlan()
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var planner = scope.ServiceProvider.GetRequiredService<SimpleTaskPlanner>();
            var sortedItems = planner.CreatePlan();

            Console.WriteLine("\nSorted Work Items:");
            foreach (var item in sortedItems)
            {
                Console.WriteLine(item);
            }
        }
    }

    private static void MarkAsCompleted()
    {
        Console.Write("Enter the ID of the work item to mark as completed: ");
        if (Guid.TryParse(Console.ReadLine(), out var itemId))
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var workItemsRepository = scope.ServiceProvider.GetRequiredService<IWorkItemsRepository>();
                var workItem = workItemsRepository.Get(itemId);

                if (workItem != null)
                {
                    workItem.IsCompleted = true;
                    workItemsRepository.Update(workItem);
                    Console.WriteLine("Work item marked as completed.");
                }
                else
                {
                    Console.WriteLine("Work item not found.");
                }
            }
        }
        else
        {
            Console.WriteLine("Invalid ID format.");
        }
    }

    private static void RemoveWorkItem()
    {
        Console.Write("Enter the ID of the work item to remove: ");
        if (Guid.TryParse(Console.ReadLine(), out var itemId))
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var workItemsRepository = scope.ServiceProvider.GetRequiredService<IWorkItemsRepository>();
                if (workItemsRepository.Remove(itemId))
                {
                    Console.WriteLine("Work item removed successfully.");
                }
                else
                {
                    Console.WriteLine("Work item not found.");
                }
            }
        }
        else
        {
            Console.WriteLine("Invalid ID format.");
        }
    }
}
