using System.Windows.Controls;

namespace MHR_Editor.Controls.Models;

public class ColumnHolder {
    public readonly DataGridColumn column;
    public readonly int            sortOrder;

    public ColumnHolder(DataGridColumn column, int sortOrder) {
        this.column    = column;
        this.sortOrder = sortOrder;
    }
}