namespace CharacterService.Models.VM.Character
{
    public class CharacterAllVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Health { get; set; }
        public int Mana { get; set; }
        public int BaseStrength { get; set; }
        public int BaseAgility { get; set; }
        public int BaseIntelligence { get; set; }
        public int BaseFaith { get; set; }

        public string Class { get; set; }

        public string CreatedBy { get; set; }

        public int statsBonus { get; set; } = 0;
    }
}
