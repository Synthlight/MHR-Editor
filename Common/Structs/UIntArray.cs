using System.Diagnostics.CodeAnalysis;
using System.IO;
using RE_Editor.Common.Models;

namespace RE_Editor.Common.Structs;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class UIntArray(uint dataCount) : RszObject, IViaType {
    public const uint DATA_WIDTH = 4;

    public List<UIntData> Data { get; init; } = new((int) dataCount);

    public void Read(BinaryReader reader) {
        for (var i = 0; i < dataCount; i++) {
            Data.Add(reader.ReadUInt32());
        }
    }

    public void Write(BinaryWriter writer) {
        for (var i = 0; i < dataCount; i++) {
            writer.Write(Data[i].DataAsUint);
        }
    }

    public UIntArray Copy() {
        var newList = new List<UIntData>();
        for (var i = 0; i < dataCount; i++) {
            newList.Add(Data[i].DataAsUint);
        }
        return new(dataCount) {
            Data = newList
        };
    }

    public class UIntData(uint data) : OnPropertyChangedBase {
        public uint DataAsUint {
            get => data;
            set {
                data = value;
                OnPropertyChanged(nameof(DataAsFloat));
            }
        }

        public float DataAsFloat {
            get => data;
            set {
                data = (uint) value;
                OnPropertyChanged(nameof(DataAsUint));
            }
        }

        public static implicit operator UIntData(uint input) {
            return new(input);
        }

        public static implicit operator UIntData(float input) {
            return new((uint) input);
        }

        public UIntData Copy() {
            return new(data);
        }
    }
}