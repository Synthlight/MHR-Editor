﻿using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using PropertyChanged;

namespace RE_Editor.Common.Models.List_Wrappers;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public interface IListWrapper<T> : INotifyPropertyChanged {
    public T Value { get; set; }
}

[SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressPropertyChangedWarnings]
public abstract class ListWrapper<T> : OnPropertyChangedBase, IListWrapper<T> {
    public abstract T Value { get; set; }
}