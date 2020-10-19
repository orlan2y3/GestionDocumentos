namespace Sistema_Gestion_de_Documentos
{
    using System.Data.Entity;

    public partial class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
            : base("name=ApplicationDbContext")
        {
        }

        public virtual DbSet<departamentos> departamentos { get; set; }
        public virtual DbSet<institucional> institucional { get; set; }
        public virtual DbSet<persona> persona { get; set; }
        public virtual DbSet<registro_correspondencia> registro_correspondencia { get; set; }
        public virtual DbSet<serie> serie { get; set; }
        public virtual DbSet<subserie> subserie { get; set; }
        public virtual DbSet<usuarios> usuarios { get; set; }
 
    }
}
