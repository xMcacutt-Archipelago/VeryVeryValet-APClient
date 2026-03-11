using System.Collections.Generic;

namespace VeryVeryValetAPClient
{
    public enum APLevelType
    {
        Level,
        Bonus,
        Final
    }
    
    public struct APLevelData
    {
        public string Name;
        public APLevelType Type;

        public APLevelData(string name, APLevelType type)
        {
            Name = name;
            Type = type;
        }
    }
    
    public class LevelOrderHandler
    {
        public static Dictionary<string, string> SaveKeyToLevelName = new Dictionary<string, string>()
        {
            { "Rooftop1", "Rooftop Parking" },
            { "AcrossSt", "Across the Street" },
            { "Bowling3", "Bowled Over" },
            { "tunnelpa", "Alley Avenue" },
            { "CliffRnd", "Cliffside Overlook" },
            { "Obvservy", "The Observatory" },
            { "nphotel", "Hotel No Vacancy" },
            { "Quarry", "Quartz Quarry" },
            { "Tunnel", "Up and Down" },
            { "Garbage1", "Cleanup Crew" },
            { "OverPass", "Overpass Galleria" },
            { "Airport", "Now Departing" },
            { "CarWash", "Rinse and Return" },
            { "Downtown", "Downtown" },
            { "Sweeper1", "Home Sweep Home" },
            { "CarDealr", "Macho Motors" },
            { "DblPark", "Double Parking" },
            { "Earthqk", "Seismic Stories" },
            { "twozones", "Dueling Venues" },
            { "MallLot", "The Lot" },
            { "sharedlt", "Sharing Spaces" },
            { "Combo1", "Three In One" },
            { "railroad", "Chaos Caboose" },
            { "Boss0", "Auto Recall" },
        };
        
        public static Dictionary<int, APLevelData> LevelNumberToName = new Dictionary<int, APLevelData>
        {
            { 0, new APLevelData("Rooftop Parking", APLevelType.Level) },
            { 1, new APLevelData("Across the Street", APLevelType.Level) },
            { 2, new APLevelData("Bowled Over", APLevelType.Bonus) },
            { 3, new APLevelData("Alley Avenue", APLevelType.Level) },
            { 4, new APLevelData("Cliffside Overlook", APLevelType.Level) },
            { 5, new APLevelData("The Observatory", APLevelType.Final) },
            { 6, new APLevelData("Hotel No Vacancy", APLevelType.Level) },
            { 7, new APLevelData("Quartz Quarry", APLevelType.Level) },
            { 8, new APLevelData("Up and Down", APLevelType.Level) },
            { 9, new APLevelData("Cleanup Crew", APLevelType.Bonus) },
            { 10, new APLevelData("Overpass Galleria", APLevelType.Level) },
            { 11, new APLevelData("Now Departing", APLevelType.Final) },
            { 12, new APLevelData("Rinse and Return", APLevelType.Level) },
            { 13, new APLevelData("Downtown", APLevelType.Level) },
            { 14, new APLevelData("Home Sweep Home", APLevelType.Bonus) },
            { 15, new APLevelData("Macho Motors", APLevelType.Level) },
            { 16, new APLevelData("Double Parking", APLevelType.Level) },
            { 17, new APLevelData("Seismic Stories", APLevelType.Final) },
            { 18, new APLevelData("Dueling Venues", APLevelType.Level) },
            { 19, new APLevelData("The Lot", APLevelType.Level) },
            { 20, new APLevelData("Sharing Spaces", APLevelType.Level) },
            { 21, new APLevelData("Three In One", APLevelType.Bonus) },
            { 22, new APLevelData("Chaos Caboose", APLevelType.Level) },
            { 23, new APLevelData("Auto Recall", APLevelType.Final) },
        };
        
        public static Dictionary<string, int> LevelNameToNumber = new Dictionary<string, int>
        {
            { "Rooftop Parking", 0 },
            { "Across the Street", 1 },
            { "Bowled Over", 2 },
            { "Alley Avenue", 3 },
            { "Cliffside Overlook", 4 },
            { "The Observatory", 5 },
            { "Hotel No Vacancy", 6 },
            { "Quartz Quarry", 7 },
            { "Up and Down", 8 },
            { "Cleanup Crew", 9 },
            { "Overpass Galleria", 10 },
            { "Now Departing", 11 },
            { "Rinse and Return", 12 },
            { "Downtown", 13 },
            { "Home Sweep Home", 14 },
            { "Macho Motors", 15 },
            { "Double Parking", 16 },
            { "Seismic Stories", 17 },
            { "Dueling Venues", 18 },
            { "The Lot", 19 },
            { "Sharing Spaces", 20 },
            { "Three In One", 21 },
            { "Chaos Caboose", 22 },
            { "Auto Recall", 23 },
        };
        
        public static Dictionary<string, int> LevelNameToNormalizedNumber = new Dictionary<string, int>
        {
            { "Rooftop Parking", 0 },
            { "Across the Street", 1 },
            { "Alley Avenue", 2 },
            { "Cliffside Overlook", 3 },
            { "Bowled Over", 4 },
            { "The Observatory", 5 },
            { "Hotel No Vacancy", 6 },
            { "Quartz Quarry", 7 },
            { "Up and Down", 8 },
            { "Overpass Galleria", 9 },
            { "Cleanup Crew", 10 },
            { "Now Departing", 11 },
            { "Rinse and Return", 12 },
            { "Downtown", 13 },
            { "Macho Motors", 14 },
            { "Double Parking", 15 },
            { "Home Sweep Home", 16 },
            { "Seismic Stories", 17 },
            { "Dueling Venues", 18 },
            { "The Lot", 19 },
            { "Sharing Spaces", 20 },
            { "Chaos Caboose", 21 },
            { "Three In One", 22 },
            { "Auto Recall", 23 },
        };
    }
}