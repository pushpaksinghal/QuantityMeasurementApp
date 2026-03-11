using QuantityMeasurementApp.Core.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuantityMeasurementApp.App.Menu
{
    public sealed class AppMenu
    {
        public void Menu()
        {
            bool flag = true;
            while (flag)
            {
                Console.WriteLine("WELCOME TO THE QUANTITY MENAGEMENT APP");
                Console.WriteLine("1.LENGTH");
                Console.WriteLine("2.WEIGHT");
                Console.WriteLine("3.VOLUME");//Added volume option in menu
                Console.WriteLine("4.TEMPERATURE");
                Console.WriteLine("5.EXIT");
                int choice = int.Parse(Console.ReadLine());
                switch (choice)
                {
                    case 1: GenericQuantityMenu.Menu<LengthUnit>("Length"); break;
                    case 2: GenericQuantityMenu.Menu<WeightUnit>("Weigth"); break;
                    case 3: GenericQuantityMenu.Menu<VolumeUnit>("Volume"); break;
                    case 4: GenericQuantityMenu.Menu<TemperatureUnit>("Temperature"); break;
                    case 5: flag = false; Console.WriteLine("THANKS FOR WISITING"); break;
                    default: flag = false; break;
                }
            }
        }
    }
}
