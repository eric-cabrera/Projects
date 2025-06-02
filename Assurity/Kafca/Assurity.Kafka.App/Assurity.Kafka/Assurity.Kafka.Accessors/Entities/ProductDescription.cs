namespace Assurity.Kafka.Accessors.Entities
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Runtime.Serialization;

    /// <summary>
    /// DB class for product description.
    /// </summary>
    [Table("ProductDescription")]
    [DataContract]
    public class ProductDescription
    {
        /// <summary>
        /// Primary Key.
        /// </summary>
        [DataMember]
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets ProductDescription.ProdNumber.
        /// </summary>
        [DataMember]
        public string? ProdNumber { get; set; }

        /// <summary>
        /// Gets or sets ProductDescription.CovID.
        /// </summary>
        [DataMember]
        public string? CovID { get; set; }

        /// <summary>
        /// Gets or sets ProductDescription.AltProdDesc.
        /// </summary>
        [DataMember]
        public string? AltProdDesc { get; set; }

        /// <summary>
        /// Gets or sets ProductDescription.ProdCategory.
        /// </summary>
        [DataMember]
        public string? ProdCategory { get; set; }

        /// <summary>
        /// Gets or sets ProductDescription.BaseRider.
        /// </summary>
        [DataMember]
        public string? BaseRider { get; set; }
    }
}