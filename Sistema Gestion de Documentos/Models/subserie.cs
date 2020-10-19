namespace Sistema_Gestion_de_Documentos
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("subserie")]
    public partial class subserie
    {
        [Key]
        public int sub_id { get; set; }

        public int? serie { get; set; }

        [StringLength(200)]
        public string titulo { get; set; }

        [StringLength(50)]
        public string sigla { get; set; }
    }
}
