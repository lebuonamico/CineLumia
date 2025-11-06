using Cine_Lumia.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cine_Lumia.Models
{
    public class CineDbContext : DbContext
    {
        public CineDbContext(DbContextOptions<CineDbContext> options) : base(options) { }

        // =====================
        // DbSets
        // =====================
        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<Cine> Cines { get; set; }
        public DbSet<Sala> Salas { get; set; }
        public DbSet<Asiento> Asientos { get; set; }
        public DbSet<Genero> Generos { get; set; }
        public DbSet<Pelicula> Peliculas { get; set; }
        public DbSet<PeliculaGenero> PeliculaGeneros { get; set; }
        public DbSet<Proyeccion> Proyecciones { get; set; }
        public DbSet<Espectador> Espectadores { get; set; }
        public DbSet<Entrada> Entradas { get; set; }
        public DbSet<Consumible> Consumibles { get; set; }
        public DbSet<CineConsumible> CineConsumibles { get; set; }
        public DbSet<EspectadorConsumible> EspectadorConsumibles { get; set; }

        public DbSet<TipoEntrada> TipoEntrada { get; set; }
        public DbSet<Formato> Formato { get; set; }



        // =====================
        // Configuración de relaciones y claves compuestas
        // =====================
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ---------- CLAVES COMPUESTAS ----------
            modelBuilder.Entity<PeliculaGenero>()
                .HasKey(pg => new { pg.Id_Pelicula, pg.Id_Genero });

            modelBuilder.Entity<CineConsumible>()
                .HasKey(cc => new { cc.Id_Cine, cc.Id_Consumible });

            // ---------- RELACIONES 1:N ----------
            modelBuilder.Entity<Cine>()
                .HasOne(c => c.Empresa)
                .WithMany(e => e.Cines)
                .HasForeignKey(c => c.Id_Empresa)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Sala>()
                .HasOne(s => s.Cine)
                .WithMany(c => c.Salas)
                .HasForeignKey(s => s.Id_Cine)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Asiento>()
                .HasOne(a => a.Sala)
                .WithMany(s => s.Asientos)
                .HasForeignKey(a => a.Id_Sala)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PeliculaGenero>()
                .HasOne(pg => pg.Pelicula)
                .WithMany(p => p.PeliculaGeneros)
                .HasForeignKey(pg => pg.Id_Pelicula);

            modelBuilder.Entity<PeliculaGenero>()
                .HasOne(pg => pg.Genero)
                .WithMany(g => g.PeliculaGeneros)
                .HasForeignKey(pg => pg.Id_Genero);

            modelBuilder.Entity<Proyeccion>()
                .HasOne(p => p.Sala)
                .WithMany(s => s.Proyecciones)
                .HasForeignKey(p => p.Id_Sala)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Proyeccion>()
                .HasOne(p => p.Pelicula)
                .WithMany(pel => pel.Proyecciones)
                .HasForeignKey(p => p.Id_Pelicula)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Entrada>()
                .HasOne(e => e.Proyeccion)
                .WithMany(p => p.Entradas)
                .HasForeignKey(e => e.Id_Proyeccion)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Entrada>()
                .HasOne(e => e.Espectador)
                .WithMany(es => es.Entradas)
                .HasForeignKey(e => e.Id_Espectador)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Entrada>()
                .HasOne(e => e.Asiento)
                .WithMany(a => a.Entradas)
                .HasForeignKey(e => e.Id_Asiento)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CineConsumible>()
                .HasOne(cc => cc.Cine)
                .WithMany(c => c.CineConsumibles)
                .HasForeignKey(cc => cc.Id_Cine);

            modelBuilder.Entity<CineConsumible>()
                .HasOne(cc => cc.Consumible)
                .WithMany(c => c.CineConsumibles)
                .HasForeignKey(cc => cc.Id_Consumible);

            modelBuilder.Entity<EspectadorConsumible>()
                .HasOne(ec => ec.Espectador)
                .WithMany(e => e.EspectadorConsumibles)
                .HasForeignKey(ec => ec.Id_Espectador);

            modelBuilder.Entity<EspectadorConsumible>()
                .HasOne(ec => ec.Consumible)
                .WithMany(c => c.EspectadorConsumibles)
                .HasForeignKey(ec => ec.Id_Consumible);

            modelBuilder.Entity<EspectadorConsumible>()
                .HasOne(ec => ec.Cine)
                .WithMany(c => c.EspectadorConsumibles)
                .HasForeignKey(ec => ec.Id_Cine);
            // Formato - Sala
            modelBuilder.Entity<Sala>()
                .HasOne(s => s.Formato)
                .WithMany(f => f.Salas)
                .HasForeignKey(s => s.Id_Formato)
                .OnDelete(DeleteBehavior.Restrict);

            // Formato - TipoEntrada
            modelBuilder.Entity<TipoEntrada>()
                .HasOne(t => t.Formato)
                .WithMany(f => f.TipoEntradas)
                .HasForeignKey(t => t.Id_Formato)
                .OnDelete(DeleteBehavior.Cascade);

            // TipoEntrada - Entrada
            modelBuilder.Entity<Entrada>()
                .HasOne(e => e.TipoEntrada)
                .WithMany(t => t.Entradas)
                .HasForeignKey(e => e.Id_TipoEntrada)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
