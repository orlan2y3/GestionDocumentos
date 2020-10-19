namespace Sistema_Gestion_de_Documentos
{
    using System.ComponentModel.DataAnnotations;

    public partial class subserie2
    {
        [Key]
        public int sub_id { get; set; }

        public int? serie { get; set; }

        [StringLength(200)]
        public string titulo { get; set; }

        [StringLength(50)]
        public string sigla { get; set; }

        [StringLength(255)]
        public string nombre_serie { get; set; }
    }
}
