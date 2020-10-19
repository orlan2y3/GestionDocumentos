using System.ComponentModel.DataAnnotations;

namespace Sistema_Gestion_de_Documentos.Models
{
    public class Instituciones
    {
        [Key]
        public int cre_id { get; set; }

        [Required]
        [StringLength(200)]
        public string nombre_institucional { get; set; }
    }
}