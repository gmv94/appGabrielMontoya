using Microsoft.EntityFrameworkCore;
using appGabrielMontoya.Models;

namespace appGabrielMontoya.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<TipoGasto> TiposGasto { get; set; }
        public DbSet<FondoMonetario> FondosMonetarios { get; set; }
        public DbSet<Presupuesto> Presupuestos { get; set; }
        public DbSet<GastoEncabezado> GastosEncabezados { get; set; }
        public DbSet<GastoDetalle> GastoDetalles { get; set; }
        public DbSet<Deposito> Depositos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FondoMonetario>(entity =>
            {
                entity.HasOne(f => f.Usuario)
                      .WithMany(u => u.FondosMonetarios)
                      .HasForeignKey(f => f.UsuarioId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Presupuesto>()
                .HasIndex(p => new { p.UsuarioId, p.TipoGastoId, p.Mes, p.Anio })
                .IsUnique();

            modelBuilder.Entity<Presupuesto>()
                .HasOne(p => p.Usuario)
                .WithMany(u => u.Presupuestos)
                .HasForeignKey(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GastoEncabezado>()
                .HasOne(g => g.Usuario)
                .WithMany(u => u.GastosEncabezados)
                .HasForeignKey(g => g.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Deposito>()
                .HasOne(d => d.Usuario)
                .WithMany(u => u.Depositos)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GastoEncabezado>()
                .HasOne(g => g.FondoMonetario)
                .WithMany(f => f.GastosEncabezados)
                .HasForeignKey(g => g.FondoMonetarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GastoDetalle>()
                .HasOne(d => d.GastoEncabezado)
                .WithMany(e => e.GastoDetalles)
                .HasForeignKey(d => d.GastoEncabezadoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GastoDetalle>()
                .HasOne(d => d.TipoGasto)
                .WithMany(t => t.GastoDetalles)
                .HasForeignKey(d => d.TipoGastoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Deposito>()
                .HasOne(d => d.FondoMonetario)
                .WithMany(f => f.Depositos)
                .HasForeignKey(d => d.FondoMonetarioId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}