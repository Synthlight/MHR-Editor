using System.Windows.Controls;

namespace MHR_Editor.Common.Controls.Models;

public class ColumnHolder {
    public readonly DataGridColumn column;
    public readonly int            sortOrder;
    public readonly ICustomSorter? customSorter;

    public ColumnHolder(DataGridColumn column, int sortOrder, ICustomSorter? customSorter = null) {
        this.column       = column;
        this.sortOrder    = sortOrder;
        this.customSorter = customSorter;
    }
}