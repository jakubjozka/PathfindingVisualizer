using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using PathfindingVisualizer.Models;

namespace PathfindingVisualizer.Helpers
{
    // Converts NodeType to corresponding SolidColorBrush for UI representation
    public class NodeTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is NodeType nodeType)
            {
                return nodeType switch
                {
                    NodeType.Empty => new SolidColorBrush(Colors.White),
                    NodeType.Wall => new SolidColorBrush(Color.FromRgb(52, 73, 94)),
                    NodeType.Start => new SolidColorBrush(Color.FromRgb(46, 204, 113)),
                    NodeType.End => new SolidColorBrush(Color.FromRgb(231, 76, 60)),
                    NodeType.Visited => new SolidColorBrush(Color.FromRgb(52, 152, 219)),
                    NodeType.Path => new SolidColorBrush(Color.FromRgb(243, 156, 18)),
                    NodeType.Grass => new SolidColorBrush(Color.FromRgb(144, 238, 144)),
                    NodeType.Mud => new SolidColorBrush(Color.FromRgb(139, 69, 19)),
                    _ => new SolidColorBrush(Colors.White)
                };
            }
            return new SolidColorBrush(Colors.White);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
