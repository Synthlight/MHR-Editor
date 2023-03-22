using System.Windows;
using System.Windows.Controls;
using RE_Editor.Common.Controls.Models;

namespace RE_Editor.Controls;

public partial class HeaderFilter {
    public static readonly DependencyProperty HeaderInfoProperty = DependencyProperty.Register(nameof(HeaderInfo), typeof(HeaderInfo), typeof(HeaderFilter), new());

    public HeaderInfo HeaderInfo {
        get => (HeaderInfo) GetValue(HeaderInfoProperty);
        set => SetValue(HeaderInfoProperty, value);
    }

    public static readonly RoutedEvent OnFilterChangedEvent = EventManager.RegisterRoutedEvent(nameof(OnFilterChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(HeaderFilter));

    public event RoutedEventHandler OnFilterChanged {
        add => AddHandler(OnFilterChangedEvent, value);
        remove => RemoveHandler(OnFilterChangedEvent, value);
    }

    public string FilterText { get; set; }

    public HeaderFilter() {
        InitializeComponent();
    }

    private void OnFilterTextChanged(object sender, TextChangedEventArgs e) {
        var newEventArgs = new RoutedEventArgs(OnFilterChangedEvent);
        RaiseEvent(newEventArgs);
    }
}