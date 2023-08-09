namespace CMSProductSystem.Models
{
    public class Kategorija
    {
        public int ID { get; set; }
        public string Naziv { get; set; }

        public ICollection<Proizvod> Product { get; set; } = new List<Proizvod>();
    }
}
