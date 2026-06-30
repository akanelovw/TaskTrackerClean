using System.Globalization;
using TaskTracker.Application.Projects.GetProjectsList;
using TaskTracker.Application.Projects.GetProjectDetails;
using TaskTracker.Application.WorkItems.GetWorkItems;

namespace TaskTracker.Maui.Common.Converters;

public class PriorityColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string? priority = value switch
        {
            GetProjectsListResponse p => p.Priority,
            GetProjectDetailsResponse p => p.Priority,
            GetWorkItemsResponse w => w.Priority,
            string s => s,
            _ => null
        };

        return priority switch
        {
            "Low" => Colors.Green,
            "Medium" => Colors.Goldenrod,
            "High" => Colors.Orange,
            "Critical" => Colors.Red,
            _ => Colors.DarkGray
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}