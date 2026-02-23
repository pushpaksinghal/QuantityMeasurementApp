using QuantityMeasurementApp.App.Menu;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuantityMeasurementApp.App
{
    public class QuantityManagenmentEntry
    {
        public static void Main(string[] args)
        {
            // calling the menu in main method 
            AppMenu quantityMenu = new AppMenu();
            quantityMenu.Menu();
        }
    }
}
