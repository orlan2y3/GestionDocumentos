namespace Sistema_Gestion_de_Documentos
{
    using System.ComponentModel.DataAnnotations;

    public partial class usuarios
    {
        [Key]
        public int idusuario { get; set; }

        [StringLength(25)]
        public string usuario { get; set; }

        [StringLength(25)]
        public string clave { get; set; }

        [StringLength(100)]
        public string nombre_completo { get; set; }
               
        public int? nivel { get; set; }

        public int? activo { get; set; }
    }
}
