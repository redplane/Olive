namespace OlivesAdministration.ViewModels.Statistic
{
    public class StatusSummaryViewModel
    {
        /// <summary>
        ///     Role which is used for statistic.
        /// </summary>
        public byte Role { get; set; }

        /// <summary>
        ///     Status of role.
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        ///     Total role.
        /// </summary>
        public long Total { get; set; }
    }
}