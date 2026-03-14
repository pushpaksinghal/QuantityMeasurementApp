using Microsoft.Extensions.DependencyInjection;
using QuantityMeasurementApp.ApplicationLayer.Menu;
using QuantityMeasurementApp.BusinessLayer.Factories;
using QuantityMeasurementApp.BusinessLayer.Services;
using QuantityMeasurementApp.ModelLayer.Enums;

using System;
using System.Collections.Generic;
using System.Text;

namespace QuantityMeasurementApp.ApplicationLayer.Entry
{
    public class QuantityManagenmentEntry
    {
        public static void Main(string[] args)
        {
            try
            {
                // Setup DI container
                var services = new ServiceCollection();

                // Register Core Services
                services.AddSingleton<UnitAdapterFactory>();
                services.AddSingleton<QuantityValidationService>();

                // Register Business Layer Services
                services.AddScoped<IQuantityConversionService, QuantityConversionService>();
                services.AddScoped<IQuantityArithmeticService, QuantityArithmeticService>();
                services.AddScoped<QuantityApplicationService>();
                // Register Generic Comparers
                services.AddSingleton(typeof(QuantityEqualityComparer<>));

                // Register Generic Menus for each unit type
                services.AddScoped<GenericQuantityMenu<LengthUnit>>();
                services.AddScoped<GenericQuantityMenu<WeightUnit>>();
                services.AddScoped<GenericQuantityMenu<VolumeUnit>>();
                services.AddScoped<GenericQuantityMenu<TemperatureUnit>>();

                // Register Main Menu
                services.AddScoped<AppMenu>();

                // Build service provider
                var serviceProvider = services.BuildServiceProvider();

                // Get and show main menu
                var appMenu = serviceProvider.GetRequiredService<AppMenu>();
                appMenu.Show();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Application Error: {ex.Message}");
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
            }
        }
    }
}
