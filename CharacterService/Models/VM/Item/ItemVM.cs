namespace CharacterService.Models.VM.Item
{
    public class ItemVM
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public int BonusStrength { get; set; }
        public int BonusAgility { get; set; }
        public int BonusIntelligence { get; set; }
        public int BonusFaith { get; set; }
    }
}
