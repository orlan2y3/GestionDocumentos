namespace Sistema_Gestion_de_Documentos
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Registros
    {
        [Key]
        public int reg_cor_id { get; set; }

        [Required]
        [StringLength(1)]
        public string tipo_correspondencia { get; set; }

        [Required]
        [StringLength(4)]
        public string numero_correspondencia { get; set; }

        [Column(TypeName = "Date")]
        public DateTime fecha_correspondencia { get; set; }
        
        [StringLength(200)]
        public string institucion { get; set; }

        [StringLength(50)]
        public string persona { get; set; }

        [StringLength(50)]
        public string numero_remision { get; set; }

        [Required]
        [StringLength(200)]
        public string titulo_asunto { get; set; }

    }
}