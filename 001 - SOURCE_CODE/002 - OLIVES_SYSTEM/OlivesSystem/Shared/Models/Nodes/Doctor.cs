namespace Shared.Models.Nodes
{
    public class Doctor : Person
    {
        public string Specialization { get; set; }

        public double Rank { get; set; }

        public string IdentityCardNo { get; set; }
    }
}