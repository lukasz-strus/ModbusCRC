using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ModbusCRC.Extensions;
using ModbusCRC.Models;

namespace ModbusCRC.ViewModels;

// ReSharper disable InconsistentNaming
// ReSharper disable StringLiteralTypo
internal partial class MainViewModel : ObservableObject
{
    [ObservableProperty] private string? _input;
    [ObservableProperty] private string? _iteration;

    [ObservableProperty] private string? _resultCRC;
    [ObservableProperty] private string? _resultTime;
    [ObservableProperty] private string? _resultIterationTime;

    private readonly CRC _crc = new();

    [RelayCommand]
    private void Calculate()
    {
        if (InputIsNotValid() || IterationIsNotValid()) return;
        if(Input is null || Iteration is null) return;

        try
        {
            var data = Input.ToByteArray();
            var iterations = int.Parse(Iteration);
            var result = Array.Empty<byte>();
            var stopWatch = new Stopwatch();

            stopWatch.Start();
            for (var i = 0; i < iterations; i++)
            {
                result = _crc.Calculate(data);
            }
            stopWatch.Stop();

            if (ResultIsNotValid(result)) return;

            ResultCRC = result.ToHex(false);

            decimal elapsedMilliseconds = stopWatch.ElapsedMilliseconds;
            ResultTime = elapsedMilliseconds.ToString(CultureInfo.CurrentCulture);
            var elapsedIterationMilliseconds = elapsedMilliseconds / iterations;
            ResultIterationTime = elapsedIterationMilliseconds.ToString("0.00000000");
        }
        catch
        {
            ShowCRCErrorMsg();
        }
    }

    private static bool ResultIsNotValid(IEnumerable? result)
    {
        if (result != null) return false;

        ShowCRCErrorMsg();
        return true;
    }

    private bool IterationIsNotValid()
    {
        if (string.IsNullOrEmpty(Iteration))
        {
            MessageBox.Show("Liczba iteracji jest pusta!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            return true;
        }
        var iteration = long.Parse(Iteration);
        if (iteration is >= 1 and <= 1000000000) return false;

        MessageBox.Show("Liczba iteracji musi być z zakresu 1-1000000000!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
        return true;
    }

    private bool InputIsNotValid()
    {
        if (string.IsNullOrEmpty(Input))
        {
            MessageBox.Show("Wejście jest puste!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            return true;
        }

        if (Input.Length <= 512) return false;

        MessageBox.Show("Wejście nie może być dłuższe niż 256 bajtów!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
        return true;
    }

    private static void ShowCRCErrorMsg() => MessageBox.Show("Wystąpił błąd podczas obliczania CRC!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);

}