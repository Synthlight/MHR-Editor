using System.Windows.Controls;

namespace RE_Editor.Common.Controls.Models;

public class ColumnHolder {
    public readonly DataGridColumn column;
    public readonly int            sortOrder;
    public readonly HeaderInfo     headerInfo;
    public readonly ICustomSorter? customSorter;

    public ColumnHolder(DataGridColumn column, int sortOrder, HeaderInfo headerInfo, ICustomSorter? customSorter = null) {
        this.column       = column;
        this.sortOrder    = sortOrder;
        this.headerInfo   = headerInfo;
        this.customSorter = customSorter;
    }
}