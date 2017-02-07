using System;
using System.Collections.Generic;
// ReSharper disable MemberCanBePrivate.Global


namespace Domain
{
    // TODO: заблокировать некоторые ячейки для редактирования в форме
    public class PlayerEntity
    {
        public int Id { get; set; }
        public string PersonId { get; set; }
        public string Nickname { get; set; }
        public string Country { get; set; }
        public int Money { get; set; }
        public int ScoreMax { get; set; }
        public DateTime LastSessionDateTime { get; set; }
        public DateTime FirstSessionDateTime { get; set; }
        public short SelectedSkin { get; set; }
        public List<short> UnlockedSkins { get; }
        public string UnlockedSkinsString { get; set; }
        public short SelectedAbility { get; set; }
        public List<short> UnlockedAbilities { get; }
        public string UnlockedAbilitiesString { get; set; }

        public PlayerEntity(List<short> unlockedSkins, List<short> unlockedAbilities)
        {
            UnlockedSkins = unlockedSkins;
            UnlockedSkinsString = "";
            for (var i = 0; i < UnlockedSkins.Count; i++)
            {
                UnlockedSkinsString += UnlockedSkins[i] + (i < UnlockedSkins.Count - 1 ? ", " : "");
            }
            
            UnlockedAbilities = unlockedAbilities;
            UnlockedAbilitiesString = "";
            for (var i = 0; i < UnlockedAbilities.Count; i++)
            {
                UnlockedAbilitiesString += UnlockedAbilities[i] + (i < UnlockedAbilities.Count - 1 ? ", " : "");
            }
        }
    }
}
