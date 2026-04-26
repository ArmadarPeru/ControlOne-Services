using ControlOne.AdminService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControlOne.AdminService.Data
{
   public class AdminContext : DbContext
   {
      public AdminContext(DbContextOptions<AdminContext> options) : base(options)
      {
      }

      protected override void OnModelCreating(ModelBuilder modelBuilder)
      {
         modelBuilder.Entity<EventoORM>()
             .ToTable("Eventos", "dbo")
             .HasKey(p => p.id);

         modelBuilder.Entity<EventoORM>()
       .HasMany(p => p.promociones)     // Parent has many Children
       .WithOne(c => c.evento)       // Each Child has one Parent
       .HasForeignKey(c => c.eventoId) // Explicitly set the FK
       .OnDelete(DeleteBehavior.Cascade); // Automatically delete children if parent is deleted
      }

		public DbSet<Apoderado> Apoderados { get; set; }
      public DbSet<ApoderadoUsuario> ApoderadosUsuarios { get; set; }
      public DbSet<Usuario> Usuarios { get; set; }
      public DbSet<UsuarioZona> UsuariosZona { get; set; }
      public DbSet<UsuarioControlSalida> UsuariosControlSalida { get; set; }
      public DbSet<Cobro> Cobros { get; set; }
      public DbSet<UsuarioControlJuego> UsuariosControlJuego { get; set; }
      public DbSet<UsuarioAccion> UsuarioAccion { get; set; }
      public DbSet<User> Users { get; set; }
      public DbSet<OperacionResultado> OperacionResultados { get; set; }
      public DbSet<Role> Roles { get; set; }
      public DbSet<EventoHorario> EventoHorarios { get; set; }
      public DbSet<IziTrack> IziTracks { get; set; }
      public DbSet<PaymentInfo> PaymentInfos { get; set; }
      public DbSet<AforoInfo> AforoInfos { get; set; }
      public DbSet<UserRole> UserRoles { get; set; }
      public DbSet<Evento> Eventos { get; set; }
      public DbSet<Gasto> Gastos { get; set; }
      public DbSet<Caja> Cajas { get; set; }
      public DbSet<Concepto> Conceptos { get; set; }
      public DbSet<Configuracion> Configuraciones { get; set; }
      public DbSet<CajaView> CajaViews { get; set; }
      public DbSet<EventoResumen> EventoResumenes { get; set; }
      public DbSet<HoraActual> HorasActuales { get; set; }
      public DbSet<Descuento> Descuentos { get; set; }
      public DbSet<EventoOnline> EventosOnline { get; set; }
      public DbSet<EventoRow> EventoRows { get; set; }
		public DbSet<EventoORM> EventosORM { get; set; }
		public DbSet<EntradaRow> EntradaRows { get; set; }
      public DbSet<TicketPromocion> EntradaPromocionesRows { get; set; }
      public DbSet<EdadInfo> EdadInfos { get; set; }
      public DbSet<CanceladoInfo> Cancelados { get; set; }
      public DbSet<MiUsuario> MisUsuarios { get; set; }
      public DbSet<Promocion> Promociones { get; set; }
      public DbSet<PromocionFecha> PromocionesFecha { get; set; }
      public DbSet<PromocionRow> PromocionRows { get; set; }
      public DbSet<PromocionEmpresaRow> PromocionesEmpresaRows { get; set; }
      public DbSet<Historial> Historiales { get; set; }
      public DbSet<ApoderadoExcel> ApoderadosExcel { get; set; }
      public DbSet<TicketPromocion> TicketPromociones { get; set; }
      public DbSet<TicketDefinicion> TicketTipos { get; set; }
      public DbSet<TicketControl> TicketsControl { get; set; }
      public DbSet<TicketPrecio> TicketPrecio { get; set; }
		public DbSet<SimpleAforo> SimpleAforos { get; set; }
		public DbSet<EventoPromocion> EventosPromocion { get; set; }
	}
}