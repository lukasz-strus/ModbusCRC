using System.Collections;
using System.Diagnostics;
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
        if (InputIsValid() || IterationIsValid()) return;
        if(Input is null || Iteration is null) return;

        var data = Input.ToByteArray();
        var iterations = int.Parse(Iteration);
        
        var result = Array.Empty<byte>();

        var sw = Stopwatch.StartNew();
        for (var i = 0; i < iterations; i++)
        {
            result = _crc.Calculate(data);
        }

        sw.Stop();

        if (ResultIsValid(result)) return;

        ResultCRC = result.ToHex(false);
        ResultTime = sw.Elapsed.ToString("mm':'ss':'fff");
        ResultIterationTime = (sw.Elapsed / iterations).ToString("mm':'ss':'fff");
    }

    private static bool ResultIsValid(IEnumerable? result)
    {
        if (result != null) return false;
        MessageBox.Show("Wystąpił błąd podczas obliczania CRC!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
        return true;

    }

    private bool IterationIsValid()
    {
        if (!string.IsNullOrEmpty(Iteration)) return false;
        MessageBox.Show("Liczba iteracji jest pusta!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
        return true;
    }

    private bool InputIsValid()
    {
        if (!string.IsNullOrEmpty(Input)) return false;
        MessageBox.Show("Wejście jest puste!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
        return true;
    }
}