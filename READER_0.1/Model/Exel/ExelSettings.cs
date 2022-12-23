using System;
using System.Collections.Generic;
using System.Text;

namespace READER_0._1.Model.Settings
{
    public class ExelSettingsRead
    {
        public bool MultiWorksheet { get; private set; } = false;
        public Dictionary<string, List<string>> SearchingColumnName { get; private set; } = new Dictionary<string, List<string>>
        {
            {"Номера отправки", ShipmentNumbersName},
            {"Номера вагонов", WagonNumberName},
            {"Номера контейнеров", ContainerNumberName},
            {"Станция отправления груза", StationOfDestinationName},
            {"Станция назначения груза", StationOfDispatchName},
            {"Груз", CargoName},
            {"Код груза ГНГ", CargoCodeGNGName},
            {"Код груза ЕТСНГ", CargoCodeETCHGName},
            {"Дата отправки", DateOfDispatchName},
            {"Дата входа", DateOfEntryName},
            {"Дата выхода", DateOfExitName}            
        };

        //0 индекс - дефолтное название столбика
        public static List<string> ShipmentNumbersName { get; private set; } = new List<string>
        {
            "Номер отправки","№ накладной","Номер накладной","номер накладной","номер отправок","Номер отправок","номер отправки",
           "№ отправки","№ отправок","Номера отправок","номера отправок","№ отправок"
        };
        public static List<string> WagonNumberName { get; private set; } = new List<string>
        {
            "Номер вагона","№ вагона","номер вагона","Номера вагонов","номера вагонов"
        };
        public static List<string> ContainerNumberName { get; private set; } = new List<string>
        {
            "Номер контейнера","номер контейнера"
        };
        public static List<string> StationOfDestinationName { get; private set; } = new List<string>
        {
            "Станция отправления груза","станция отправления груза"
        };
        public static List<string> StationOfDispatchName { get; private set; } = new List<string>
        {
            "Станция назначения груза","станция назначения груза"
        };
        public static List<string> CargoName { get; private set; } = new List<string>
        {
            "Груз","груз"
        };
        public static List<string> CargoCodeGNGName { get; private set; } = new List<string>
        {
            "Код груза ГНГ","код груза ГНГ"
        };
        public static List<string> CargoCodeETCHGName { get; private set; } = new List<string>
        {
            "Код груза ЕТСНГ","код груза ЕТСНГ"
        };
        public static List<string> DateOfDispatchName { get; private set; } = new List<string>
        {
            "Дата отправки","отп.","дата отправки","отправка","Отправка","Дата отправления","дата отправления"
        };
        public static List<string> DateOfEntryName { get; private set; } = new List<string>
        {
            "Дата входа","вх.","дата входа","Дата вхождения","дата вхождения"
        };
        public static List<string> DateOfExitName { get; private set; } = new List<string>
        {
           "Дата выхода","вых.","дата выхода","Дата выхода","дата выхода"
        };   
                        
    }
}
