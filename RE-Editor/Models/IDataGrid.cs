using System.Windows.Controls;

namespace RE_Editor.Models;

public interface IDataGrid {
    public ScrollBarVisibility HorizontalScrollBarVisibility { get; set; }
    public ScrollBarVisibility VerticalScrollBarVisibility   { get; set; }
}