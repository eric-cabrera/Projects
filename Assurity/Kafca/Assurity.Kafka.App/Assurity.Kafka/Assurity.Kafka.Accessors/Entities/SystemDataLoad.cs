namespace Assurity.Kafka.Accessors.Entities
{
    /// <summary>
    /// A control table used by the Assurity.Commissions.Debt.Identifier service
    /// to manage Debt data loads, their initiation, and status.
    /// </summary>
    public class SystemDataLoad
    {
        public int Id { get; set; }

        /// <summary>
        /// Indicates the type of data load to perform.
        /// Change - Loads a specific agent for which its hierarchy has been changed.
        /// Full - Pulls all data.
        /// Partial - Pulls only the data since the last successful Full/Partial data load.
        /// </summary>
        public LoadType LoadType { get; set; }

        /// <summary>
        /// A status indicator with the following possible values.
        ///   null - Indicates that the data load has been initiated (by the existence of
        ///     the SystemDataLoad record) but not yet started by the Identifier service.
        ///   Failed - An unexpected error occured and the logs should be investigated.
        ///   Finished - The data load has been fully completed by the Identifier service.
        ///   Started - Indicates that the data load has been started by the Identifier
        ///     service and is currently running.
        ///   Waiting - Indicates that the Identifier service is waiting until "cycle" is
        ///     no longer running in LifePro.
        /// </summary>
        public SystemDataLoadStatus Status { get; set; }

        /// <summary>
        /// Used as a trigger. Intended to be populated during this record's creation
        /// in the database with the current or future DateTime.
        /// </summary>
        public DateTime InitiatedDate { get; set; }

        /// <summary>
        /// When the data load was actually kicked off by the Identifier service.
        /// </summary>
        public DateTime? StartedDate { get; set; }

        /// <summary>
        /// When the data load was completed by the Identifier service,
        /// such as when all records were sent to the "Commissions" RabbitMQ queue,
        /// or if the data load fails.
        /// </summary>
        public DateTime? FinishedDate { get; set; }

        public virtual AgentHierarchyChange AgentHierarchyChange { get; set; }
    }
}