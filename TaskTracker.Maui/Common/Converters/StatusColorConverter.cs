using System.Globalization;
using TaskTracker.Application.Projects.GetProjectsList;
using TaskTracker.Application.Projects.GetProjectDetails;
using TaskTracker.Application.WorkItems.GetWorkItems;

namespace TaskTracker.Maui.Common.Converters;

public class StatusColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string? status = value switch
        {
            GetProjectsListResponse p => p.Status,
            GetProjectDetailsResponse p => p.Status,
            GetWorkItemsResponse w => w.Status,
            string s => s,
            _ => null
        };

        return status switch
        {
            "Active" => Colors.DodgerBlue,
            "Completed" => Colors.Green,
            "Archived" => Colors.Gray,

            "ToDo" => Colors.Gray,
            "InProgress" => Colors.Orange,
            "Done" => Colors.Green,

            _ => Colors.DarkGray
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}