using Microsoft.UI.Xaml;

#nullable enable

namespace EHunter.Provider.Pixiv.Views
{
    public class EqualsStateTrigger : StateTriggerBase
    {
        public static readonly DependencyProperty ValueProperty
            = DependencyProperty.Register(nameof(Value), typeof(object), typeof(EqualsStateTrigger),
                new PropertyMetadata(null, ValueChanged));
#pragma warning disable CA1721 // 属性名不应与 get 方法匹配
        public object? Value
#pragma warning restore CA1721 // 属性名不应与 get 方法匹配
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly DependencyProperty BindValueProperty
            = DependencyProperty.Register(nameof(BindValue), typeof(object), typeof(EqualsStateTrigger),
                new PropertyMetadata(null, ValueChanged));
        public object? BindValue
        {
            get => GetValue(BindValueProperty);
            set => SetValue(BindValueProperty, value);
        }

        private static void ValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var trigger = (EqualsStateTrigger)o;
            object? value = trigger.Value;
            if (value == null)
                trigger.SetActive(trigger.BindValue == null);
            else
                trigger.SetActive(value.Equals(trigger.BindValue));
        }
    }
}
