using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace QLSanBong_API.Data;

public partial class QlsanBongContext : DbContext
{
    public QlsanBongContext()
    {
    }

    public QlsanBongContext(DbContextOptions<QlsanBongContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Action> Actions { get; set; }

    public virtual DbSet<ActionService> ActionServices { get; set; }

    public virtual DbSet<ChiTietPd> ChiTietPds { get; set; }

    public virtual DbSet<ChiTietYcd> ChiTietYcds { get; set; }

    public virtual DbSet<GiaGioThue> GiaGioThues { get; set; }

    public virtual DbSet<PhieuDatSan> PhieuDatSans { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RoleAuth> RoleAuths { get; set; }

    public virtual DbSet<SanBong> SanBongs { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<YeuCauDatSan> YeuCauDatSans { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=HIEW\\THANHHIEU;Database=QLSanBong;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Action>(entity =>
        {
            entity.ToTable("Action");

            entity.Property(e => e.ActionId)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("ActionID");
            entity.Property(e => e.ActionName).HasMaxLength(50);
        });

        modelBuilder.Entity<ActionService>(entity =>
        {
            entity.ToTable("ActionService");

            entity.Property(e => e.Id)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("ID");
            entity.Property(e => e.ActionId)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("ActionID");
            entity.Property(e => e.ServiceId)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("ServiceID");

            entity.HasOne(d => d.Action).WithMany(p => p.ActionServices)
                .HasForeignKey(d => d.ActionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ActionService_Action");

            entity.HasOne(d => d.Service).WithMany(p => p.ActionServices)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ActionService_Service");
        });

        modelBuilder.Entity<ChiTietPd>(entity =>
        {
            entity.HasKey(e => new { e.MaPds, e.MaSb, e.MaGio, e.Ngaysudung }).HasName("PK__ChiTietP__3AE048CAA0E04E9B");

            entity.ToTable("ChiTietPDS");

            entity.Property(e => e.MaPds)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("MaPDS");
            entity.Property(e => e.MaSb)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("MaSB");
            entity.Property(e => e.MaGio)
                .HasMaxLength(8)
                .IsUnicode(false);
            entity.Property(e => e.Ghichu).HasMaxLength(100);

            entity.HasOne(d => d.MaPdsNavigation).WithMany(p => p.ChiTietPds)
                .HasForeignKey(d => d.MaPds)
                .HasConstraintName("FK_ChiTietPDS_PhieuDatSan");
        });

        modelBuilder.Entity<ChiTietYcd>(entity =>
        {
            entity.HasKey(e => new { e.Stt, e.MaSb, e.Magio, e.Ngaysudung });

            entity.ToTable("ChiTietYCDS");

            entity.Property(e => e.Stt).HasColumnName("STT");
            entity.Property(e => e.MaSb)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("MaSB");
            entity.Property(e => e.Magio)
                .HasMaxLength(8)
                .IsUnicode(false);
            entity.Property(e => e.GhiChu).HasMaxLength(200);
            entity.Property(e => e.TrangThai).HasMaxLength(50);

            entity.HasOne(d => d.SttNavigation).WithMany(p => p.ChiTietYcds)
                .HasForeignKey(d => d.Stt)
                .HasConstraintName("FK_ChiTietYCDS_YeuCauDatSan");
        });

        modelBuilder.Entity<GiaGioThue>(entity =>
        {
            entity.HasKey(e => e.MaGio).HasName("PK__GiaGioTh__3CD3DE2CCC01FD0F");

            entity.ToTable("GiaGioThue");

            entity.Property(e => e.MaGio)
                .HasMaxLength(8)
                .IsUnicode(false);
            entity.Property(e => e.Dongia).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Ghichu).HasMaxLength(100);
        });

        modelBuilder.Entity<PhieuDatSan>(entity =>
        {
            entity.HasKey(e => e.MaPds).HasName("PK__PhieuDat__3AE048CAD5A388C3");

            entity.ToTable("PhieuDatSan");

            entity.Property(e => e.MaPds)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("MaPDS");
            entity.Property(e => e.GhiChu).HasMaxLength(50);
            entity.Property(e => e.MaKh)
                .HasMaxLength(250)
                .HasColumnName("MaKH");
            entity.Property(e => e.MaNv)
                .HasMaxLength(250)
                .HasColumnName("MaNV");
            entity.Property(e => e.Ngaylap).HasColumnType("datetime");
            entity.Property(e => e.Sttds).HasColumnName("sttds");
            entity.Property(e => e.TongTien).HasColumnType("decimal(18, 0)");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE3A7F5BCE7A");

            entity.ToTable("Role");

            entity.Property(e => e.RoleId)
                .HasMaxLength(250)
                .HasColumnName("RoleID");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ThongTin).HasMaxLength(100);
        });

        modelBuilder.Entity<RoleAuth>(entity =>
        {
            entity.HasKey(e => new { e.RoleId, e.AuthId });

            entity.ToTable("RoleAuth");

            entity.Property(e => e.RoleId)
                .HasMaxLength(250)
                .HasColumnName("RoleID");
            entity.Property(e => e.AuthId)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("AuthID");
            entity.Property(e => e.GhiChu).HasMaxLength(500);

            entity.HasOne(d => d.Auth).WithMany(p => p.RoleAuths)
                .HasForeignKey(d => d.AuthId)
                .HasConstraintName("FK_RoleAuth_ActionService");

            entity.HasOne(d => d.Role).WithMany(p => p.RoleAuths)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_RoleAuth_Role");
        });

        modelBuilder.Entity<SanBong>(entity =>
        {
            entity.HasKey(e => e.MaSb).HasName("PK__SanBong__2725080EEA944BF7");

            entity.ToTable("SanBong");

            entity.Property(e => e.MaSb)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("MaSB");
            entity.Property(e => e.DiaChi).HasColumnType("ntext");
            entity.Property(e => e.Dientich).HasMaxLength(20);
            entity.Property(e => e.Ghichu).HasColumnType("ntext");
            entity.Property(e => e.TenSb)
                .HasMaxLength(30)
                .HasColumnName("TenSB");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.ToTable("Service");

            entity.Property(e => e.ServiceId)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("ServiceID");
            entity.Property(e => e.ServiceName).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.Property(e => e.UserId)
                .HasMaxLength(250)
                .HasColumnName("UserID");
            entity.Property(e => e.Diachi).HasMaxLength(100);
            entity.Property(e => e.Password)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Sdt)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("SDT");
            entity.Property(e => e.Ten).HasMaxLength(30);
            entity.Property(e => e.Username)
                .HasMaxLength(30)
                .IsUnicode(false);
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.RoleId }).HasName("PK__UserRole__AF27604F44A92D61");

            entity.ToTable("UserRole");

            entity.Property(e => e.UserId)
                .HasMaxLength(250)
                .HasColumnName("UserID");
            entity.Property(e => e.RoleId)
                .HasMaxLength(250)
                .HasColumnName("RoleID");
            entity.Property(e => e.GhiChu).HasMaxLength(500);

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_UserRole_Role");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserRole_User");
        });

        modelBuilder.Entity<YeuCauDatSan>(entity =>
        {
            entity.HasKey(e => e.Stt);

            entity.ToTable("YeuCauDatSan");

            entity.Property(e => e.Stt).HasColumnName("STT");
            entity.Property(e => e.GhiChu).HasMaxLength(50);
            entity.Property(e => e.MaKh)
                .HasMaxLength(250)
                .HasColumnName("MaKH");
            entity.Property(e => e.Thoigiandat).HasColumnType("datetime");
            entity.Property(e => e.TongTien).HasColumnType("decimal(18, 0)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
