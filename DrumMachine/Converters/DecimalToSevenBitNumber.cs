using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Melanchall.DryWetMidi.Common;

namespace DrumMachine.Converters
{

    public class DecimalToSevenBitNumber : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is SevenBitNumber)
            {
                return (byte)(SevenBitNumber)value;
            }
            
            return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is Decimal)
            {
                return (SevenBitNumber)(int)(Decimal)(value);
            }

            throw new NotSupportedException();
        }
    }
}
