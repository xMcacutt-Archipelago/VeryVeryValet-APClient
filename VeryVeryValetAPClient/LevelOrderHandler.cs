using System.Collections.Generic;

namespace VeryVeryValetAPClient
{
    public enum LevelType
    {
        Level,
        Bonus,
        Final
    }
    
    public struct LevelData
    {
        public string Name;
        public LevelType Type;

        public LevelData(string name, LevelType type)
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
        
        public static Dictionary<int, LevelData> LevelNumberToName = new Dictionary<int, LevelData>
        {
            { 0, new LevelData("Rooftop Parking", LevelType.Level) },
            { 1, new LevelData("Across the Street", LevelType.Level) },
            { 2, new LevelData("Bowled Over", LevelType.Bonus) },
            { 3, new LevelData("Alley Avenue", LevelType.Level) },
            { 4, new LevelData("Cliffside Overlook", LevelType.Level) },
            { 5, new LevelData("The Observatory", LevelType.Final) },
            { 6, new LevelData("Hotel No Vacancy", LevelType.Level) },
            { 7, new LevelData("Quartz Quarry", LevelType.Level) },
            { 8, new LevelData("Up and Down", LevelType.Level) },
            { 9, new LevelData("Cleanup Crew", LevelType.Bonus) },
            { 10, new LevelData("Overpass Galleria", LevelType.Level) },
            { 11, new LevelData("Now Departing", LevelType.Final) },
            { 12, new LevelData("Rinse and Return", LevelType.Level) },
            { 13, new LevelData("Downtown", LevelType.Level) },
            { 14, new LevelData("Home Sweep Home", LevelType.Bonus) },
            { 15, new LevelData("Macho Motors", LevelType.Level) },
            { 16, new LevelData("Double Parking", LevelType.Level) },
            { 17, new LevelData("Seismic Stories", LevelType.Final) },
            { 18, new LevelData("Dueling Venues", LevelType.Level) },
            { 19, new LevelData("The Lot", LevelType.Level) },
            { 20, new LevelData("Sharing Spaces", LevelType.Level) },
            { 21, new LevelData("Three In One", LevelType.Bonus) },
            { 22, new LevelData("Chaos Caboose", LevelType.Level) },
            { 23, new LevelData("Auto Recall", LevelType.Final) },
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