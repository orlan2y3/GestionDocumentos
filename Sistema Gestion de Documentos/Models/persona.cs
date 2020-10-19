namespace Sistema_Gestion_de_Documentos
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("persona")]
    public partial class persona
    {
        [Key]
        public int per_id { get; set; }

        [Required]
        [StringLength(50)]
        public string nombre { get; set; }

        [StringLength(200)]
        public string descripcion { get; set; }

        [StringLength(100)]
        public string rol { get; set; }

        [StringLength(200)]
        public string direccion { get; set; }

        [StringLength(50)]
        public string telefono { get; set; }

        [StringLength(50)]
        public string fax { get; set; }

        [StringLength(50)]
        public string telefono_movil { get; set; }

        [StringLength(100)]
        public string correo_electronico { get; set; }

        [StringLength(100)]
        public string pagina_web { get; set; }
    }
}
