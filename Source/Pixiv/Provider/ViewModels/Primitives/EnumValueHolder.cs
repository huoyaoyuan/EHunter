using System;
using System.Runtime.CompilerServices;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace EHunter.Pixiv.ViewModels.Primitives
{
    public class EnumValueHolder<T> : ObservableObject
        where T : struct, Enum
    {
        // TODO: part of workaround of https://github.com/microsoft/microsoft-ui-xaml/issues/3339

        public EnumValueHolder()
        {
            if (Unsafe.SizeOf<T>() != sizeof(int))
                throw new NotSupportedException("This type must be used with int-sized enum.");
        }

        private T _value;
        public T Value
        {
            get => _value;
            set
            {
                if (SetProperty(ref _value, value))
                    OnPropertyChanged(nameof(IntValue));
            }
        }

        public int IntValue
        {
            get => Unsafe.As<T, int>(ref _value);
            set => Value = Unsafe.As<int, T>(ref value);
        }
    }
}
