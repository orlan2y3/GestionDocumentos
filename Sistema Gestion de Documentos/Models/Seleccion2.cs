using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_Gestion_de_Documentos.Models
{
    public class Seleccion2
    {
        [Key]
        public int reg_cor_id { get; set; }
        
        [StringLength(1)]
        public string tipo_correspondencia { get; set; }
        
        [StringLength(4)]
        public string numero_correspondencia { get; set; }

        [Column(TypeName = "date")]
        public DateTime fecha_correspondencia { get; set; }

        [StringLength(50)]
        public string remitente { get; set; }

        [StringLength(200)]
        public string institucion { get; set; }

        [StringLength(50)]
        public string numero_remision { get; set; }

        [StringLength(200)]
        public string titulo_asunto { get; set; }

        [StringLength(255)]
        public string nombre_serie { get; set; }

        [StringLength(200)]
        public string nombre_subserie { get; set; }        

        public int id_departamento { get; set; }        

        [StringLength(50)]
        public string nombre_dep { get; set; }

        [StringLength(200)]
        public string ruta_archivo { get; set; }

        public int id_usuario { get; set; }
    }
}