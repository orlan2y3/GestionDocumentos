using System.ComponentModel.DataAnnotations;

namespace Sistema_Gestion_de_Documentos.Models
{
    public class Remitentes
    {
        [Key]
        public int per_id { get; set; }

        [Required]
        [StringLength(50)]
        public string nombre { get; set; }
    }
}