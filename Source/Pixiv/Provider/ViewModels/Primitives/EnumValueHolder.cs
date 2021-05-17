using System;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;

namespace EHunter.Pixiv.ViewModels.Primitives
{
    public class EnumValueHolder<T> : ObservableObject
        where T : struct, Enum
    {
        public EnumValueHolder()
        {
            if (Unsafe.SizeOf<T>() != sizeof(int))
                throw new NotSupportedException("This type must be used with int-sized enum.");
        }

        private T _value;
        public T Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }
    }
}
