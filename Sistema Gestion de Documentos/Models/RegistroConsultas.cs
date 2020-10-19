using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_Gestion_de_Documentos.Models
{
    public class RegistroConsultas
    {
        [Key]
        public int reg_cor_id { get; set; }

        [Required]
        [StringLength(1)]
        public string tipo_correspondencia { get; set; }

        [Required]
        [StringLength(4)]
        public string numero_correspondencia { get; set; }

        [StringLength(50)]
        public string numero_remision { get; set; }

        [Column(TypeName = "Date")]
        public DateTime fecha_correspondencia { get; set; }

        [StringLength(50)]
        public string persona { get; set; }

        [StringLength(200)]
        public string institucion { get; set; }               

        [Required]
        [StringLength(200)]
        public string titulo_asunto { get; set; }

        [StringLength(200)]
        public string ruta_archivo { get; set; }

        [StringLength(200)]
        public string acuse { get; set; }
       
    }
}