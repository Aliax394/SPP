using System;
using System.Collections.Generic;
using System.Text;

namespace lab2.Project
{
    internal class TempConver
    {
        private const double AbsoluteZeroCelsius = -273.15;
        private const double AbsoluteZeroKelvin = 0.0;
        private const double WaterFreezingPointCelsius = 0.0;
        private const double WaterBoilingPointCelsius = 100.0;

        public double CelsiusToFahrenheit(double celsius)
        {
            ValidateCelsius(celsius);
            return celsius * 9.0 / 5.0 + 32.0;
        }

        public double FahrenheitToCelsius(double fahrenheit)
        {
            double celsius = (fahrenheit - 32.0) * 5.0 / 9.0;
            ValidateCelsius(celsius);
            return celsius;
        }

        public double CelsiusToKelvin(double celsius)
        {
            ValidateCelsius(celsius);
            return celsius + 273.15;
        }

        public double KelvinToCelsius(double kelvin)
        {
            ValidateKelvin(kelvin);
            return kelvin - 273.15;
        }

        public double FahrenheitToKelvin(double fahrenheit)
        {
            double celsius = FahrenheitToCelsius(fahrenheit);
            return CelsiusToKelvin(celsius);
        }

        public double KelvinToFahrenheit(double kelvin)
        {
            double celsius = KelvinToCelsius(kelvin);
            return CelsiusToFahrenheit(celsius);
        }

        public bool IsValidCelsius(double celsius)
        {
            return celsius >= AbsoluteZeroCelsius;
        }

        public bool IsValidKelvin(double kelvin)
        {
            return kelvin >= AbsoluteZeroKelvin;
        }

        public string GetWaterStateByCelsius(double celsius)
        {
            ValidateCelsius(celsius);

            if (celsius < WaterFreezingPointCelsius)
                return "Solid";

            if (Math.Abs(celsius - WaterFreezingPointCelsius) < 0.0001)
                return "Freezing point";

            if (celsius > WaterFreezingPointCelsius && celsius < WaterBoilingPointCelsius)
                return "Liquid";

            if (Math.Abs(celsius - WaterBoilingPointCelsius) < 0.0001)
                return "Boiling point";

            return "Gas";
        }

        public double NormalizeCelsius(double celsius)
        {
            ValidateCelsius(celsius);
            return Math.Round(celsius, 2, MidpointRounding.AwayFromZero);
        }

        private void ValidateCelsius(double celsius)
        {
            if (!IsValidCelsius(celsius))
                throw new ArgumentException("Temperature below absolute zero in Celsius.");
        }

        private void ValidateKelvin(double kelvin)
        {
            if (!IsValidKelvin(kelvin))
                throw new ArgumentException("Temperature below absolute zero in Kelvin.");
        }
    
    }
}
