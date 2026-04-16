using System;
using System.Collections.Generic;
using System.Text;

namespace lab2.Project
{
    public interface ITemperatureConverter
    {
        double CelsiusToFahrenheit(double celsius);
        double FahrenheitToCelsius(double fahrenheit);
        double CelsiusToKelvin(double celsius);
        double KelvinToCelsius(double kelvin);
        double FahrenheitToKelvin(double fahrenheit);
        double KelvinToFahrenheit(double kelvin);

        bool IsValidCelsius(double celsius);
        bool IsValidKelvin(double kelvin);
        string GetWaterStateByCelsius(double celsius);
        double NormalizeCelsius(double celsius);
    }
}
