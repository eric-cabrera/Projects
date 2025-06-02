namespace Assurity.Kafka.Managers.Interfaces
{
    using Assurity.Kafka.Accessors.Entities;

    public interface IPGRUP_GROUP_MASTEREventManager
    {
        /// <summary>
        /// Process the event from the PGRUP_GROUP_MASTEREvent topic in which a new
        /// <see cref="PGRUP_GROUP_MASTER.NAME_ID"/> is applied to a group.
        /// </summary>
        /// <param name="groupMaster">the PGRUP_GROUP_MASTER record with updated data.</param>
        /// <returns></returns>
        Task ProcessEvent(PGRUP_GROUP_MASTER groupMaster);
    }
}
