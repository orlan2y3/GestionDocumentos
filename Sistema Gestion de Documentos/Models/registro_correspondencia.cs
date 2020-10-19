namespace Sistema_Gestion_de_Documentos
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class registro_correspondencia
    {
        [Key]
        public int REG_COR_ID { get; set; }

        [Required]
        [StringLength(1)]
        public string tipo_correspondencia { get; set; }

        [Required]
        [StringLength(4)]
        public string numero_correspondencia { get; set; }

        [Column(TypeName = "date")]
        public DateTime fecha_correspondencia { get; set; }

        public int? institucion { get; set; }

        public int? persona { get; set; }

        [StringLength(50)]
        public string numero_remision { get; set; }

        [Required]
        [StringLength(200)]
        public string titulo_asunto { get; set; }

        [StringLength(50)]
        public string extension { get; set; }

        [StringLength(200)]
        public string notas_referencia { get; set; }

        [StringLength(200)]
        public string ruta_archivo { get; set; }

        public int? numero_expediente { get; set; }

        public int? id_departamento { get; set; }

        [StringLength(255)]
        public string nota { get; set; }

        public int? codigo_acp_serie { get; set; }

        public int? numero_acp_subserie { get; set; }

        [StringLength(255)]
        public string anexo1 { get; set; }

        [StringLength(255)]
        public string anexo2 { get; set; }

        public int? revisada { get; set; }

        [StringLength(200)]
        public string acuse { get; set; }

        public int id_usuario { get; set; }
    }
}
