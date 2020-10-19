namespace Sistema_Gestion_de_Documentos
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("institucional")]
    public partial class institucional
    {
        [Key]
        public int cre_id { get; set; }

        [Required]
        [StringLength(200)]
        public string nombre_institucional { get; set; }

        [StringLength(15)]
        public string sigla { get; set; }

        [StringLength(200)]
        public string direccion { get; set; }

        [StringLength(50)]
        public string correo { get; set; }

        [StringLength(50)]
        public string telefono { get; set; }

        [StringLength(50)]
        public string fax { get; set; }

        [StringLength(50)]
        public string movil { get; set; }
    }
}
