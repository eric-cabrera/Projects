namespace Assurity.Kafka.Accessors.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// This is the entity for the first of 3 queries to get all of the needed benefit information with the repsective extended keys
    /// </summary>
    [Table("ExtendedKeyQueryEntity")]
    public class ExtendedKeyQueryEntity
    {
        [Key]
        public string KEY_IDENTIFIER { get; set; }

        public string KEY_CATEGORY { get; set; }

        public short KEY_CATEGORY_VALUE { get; set; }

        public string KEY_OPTION { get; set; }

        public short KEY_OPTION_VALUE { get; set; }

        public short KEY0_NUM { get; set; }
    }
}