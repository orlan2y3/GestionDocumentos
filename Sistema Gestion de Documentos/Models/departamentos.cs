namespace Sistema_Gestion_de_Documentos
{
    using System.ComponentModel.DataAnnotations;

    public partial class departamentos
    {
        public int id { get; set; }

        [Required]
        [StringLength(125)]
        public string nombre { get; set; }
    }
}
