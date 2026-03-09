using System;
using System.Collections.Generic;
using System.Linq;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Newtonsoft.Json.Linq;

namespace VeryVeryValetAPClient
{
    public class SlotData
    {
        public readonly bool LimitStars;
        public readonly bool RequireRedStars;
        public readonly bool RequireLevelCompletions;
        public readonly Dictionary<string, string?> LevelMap;
        public readonly Dictionary<string, string?> BonusMap;
        public readonly Dictionary<string, string?> FinalMap;
        public readonly bool DeathLink;
        
        public SlotData(Dictionary<string, object> slotDict)
        {
            foreach (var x in slotDict) APConsole.Instance.DebugLog($"{x.Key} {x.Value}");
            
            if (slotDict.TryGetValue("LimitStars", out var rawLimitStars)
                && rawLimitStars is long limitStars)
                LimitStars = limitStars == 1;
            
            if (slotDict.TryGetValue("RequireRedStars", out var rawRequireRedStars) 
                && rawRequireRedStars is long requireRedStars)
                RequireRedStars = requireRedStars == 1;
            
            if (slotDict.TryGetValue("RequireLevelCompletions", out var rawRequireLevelCompletions)
                && rawRequireLevelCompletions is long requireLevelCompletions)
                RequireLevelCompletions = requireLevelCompletions == 1;
            
            if (slotDict.TryGetValue("LevelMapping", out var rawLevelMap) &&
                rawLevelMap is JObject levelMapObj)
                LevelMap = levelMapObj.ToObject<Dictionary<string, string>>() ?? new Dictionary<string, string?>();
            else
                LevelMap = new Dictionary<string, string?>();
            
            if (slotDict.TryGetValue("BonusMapping", out var rawBonusMap) &&
                rawBonusMap is JObject bonusMapObj)
                BonusMap = bonusMapObj.ToObject<Dictionary<string, string>>() ?? new Dictionary<string, string?>();
            else
                BonusMap = new Dictionary<string, string?>();

            if (slotDict.TryGetValue("FinalMapping", out var rawFinalMap) &&
                rawFinalMap is JObject finalMapObj)
                FinalMap = finalMapObj.ToObject<Dictionary<string, string>>() ?? new Dictionary<string, string?>();
            else
                FinalMap = new Dictionary<string, string?>();
            
            if (slotDict.TryGetValue("DeathLink", out var rawDeathLink)
                && rawDeathLink is long deathLink)
                DeathLink = deathLink == 1;
        }
    }
}