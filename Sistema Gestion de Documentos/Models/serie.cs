namespace Sistema_Gestion_de_Documentos
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("serie")]
    public partial class serie
    {
        [Key]       
        public int ser_id { get; set; }

        [StringLength(255)]
        public string titulo { get; set; }

        [StringLength(25)]
        public string siglas { get; set; }
    }
}
