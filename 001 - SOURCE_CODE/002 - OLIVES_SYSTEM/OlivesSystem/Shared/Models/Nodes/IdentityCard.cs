namespace Shared.Models.Nodes
{
    public class IdentityCard
    {
        /// <summary>
        ///     Identity card.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     Date when identity card was issued.
        /// </summary>
        public long IssuedDate { get; set; }

        /// <summary>
        ///     Place where identity card was issued.
        /// </summary>
        public Coordinate IssuedPlace { get; set; }

        /// <summary>
        ///     Whether identity card is treated as Passport.
        /// </summary>
        public bool IsPassport { get; set; }
    }
}