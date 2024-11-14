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

    public virtual DbSet<ChiTietPd> ChiTietPds { get; set; }

    public virtual DbSet<ChiTietYcd> ChiTietYcds { get; set; }

    public virtual DbSet<GiaGioThue> GiaGioThues { get; set; }

    public virtual DbSet<KhachHang> KhachHangs { get; set; }

    public virtual DbSet<NhanVien> NhanViens { get; set; }

    public virtual DbSet<PhieuDatSan> PhieuDatSans { get; set; }

    public virtual DbSet<SanBong> SanBongs { get; set; }

    public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }

    public virtual DbSet<YeuCauDatSan> YeuCauDatSans { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=HIEW\\THANHHIEU;Database=QLSanBong;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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

            entity.HasOne(d => d.MaGioNavigation).WithMany(p => p.ChiTietPds)
                .HasForeignKey(d => d.MaGio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChiTietPDS_GiaGioThue");

            entity.HasOne(d => d.MaPdsNavigation).WithMany(p => p.ChiTietPds)
                .HasForeignKey(d => d.MaPds)
                .HasConstraintName("FK_ChiTietPDS_PhieuDatSan");

            entity.HasOne(d => d.MaSbNavigation).WithMany(p => p.ChiTietPds)
                .HasForeignKey(d => d.MaSb)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChiTietPDS_SanBong");
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

            entity.HasOne(d => d.MaSbNavigation).WithMany(p => p.ChiTietYcds)
                .HasForeignKey(d => d.MaSb)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChiTietYCDS_SanBong");

            entity.HasOne(d => d.MagioNavigation).WithMany(p => p.ChiTietYcds)
                .HasForeignKey(d => d.Magio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChiTietYCDS_GiaGioThue");

            entity.HasOne(d => d.SttNavigation).WithMany(p => p.ChiTietYcds)
                .HasForeignKey(d => d.Stt)
                .HasConstraintName("FK_ChiTietYCDS_YeuCauDatSan");
        });

        modelBuilder.Entity<GiaGioThue>(entity =>
        {
            entity.HasKey(e => e.MaGio).HasName("PK__GiaGioTh__3CD3DE2C0EFE0A63");

            entity.ToTable("GiaGioThue");

            entity.Property(e => e.MaGio)
                .HasMaxLength(8)
                .IsUnicode(false);
            entity.Property(e => e.Dongia).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Ghichu).HasMaxLength(100);
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.MaKh).HasName("PK__KhachHan__2725CF1EC8ABA411");

            entity.ToTable("KhachHang");

            entity.Property(e => e.MaKh)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("MaKH");
            entity.Property(e => e.Diachi).HasMaxLength(100);
            entity.Property(e => e.Gioitinh).HasMaxLength(6);
            entity.Property(e => e.Sdt)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("SDT");
            entity.Property(e => e.TenKh)
                .HasMaxLength(30)
                .HasColumnName("TenKH");
            entity.Property(e => e.Tendangnhap)
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.HasOne(d => d.TendangnhapNavigation).WithMany(p => p.KhachHangs)
                .HasForeignKey(d => d.Tendangnhap)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KhachHang_TaiKhoan");
        });

        modelBuilder.Entity<NhanVien>(entity =>
        {
            entity.HasKey(e => e.MaNv).HasName("PK__NhanVien__2725D70AE35EEBAE");

            entity.ToTable("NhanVien");

            entity.Property(e => e.MaNv)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("MaNV");
            entity.Property(e => e.Chucvu).HasMaxLength(20);
            entity.Property(e => e.Sdt)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("SDT");
            entity.Property(e => e.TenNv)
                .HasMaxLength(30)
                .HasColumnName("TenNV");
            entity.Property(e => e.Tendangnhap)
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.HasOne(d => d.TendangnhapNavigation).WithMany(p => p.NhanViens)
                .HasForeignKey(d => d.Tendangnhap)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NhanVien_TaiKhoan");
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
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("MaKH");
            entity.Property(e => e.MaNv)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("MaNV");
            entity.Property(e => e.Ngaylap).HasColumnType("datetime");
            entity.Property(e => e.Sttds).HasColumnName("sttds");
            entity.Property(e => e.TongTien).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.PhieuDatSans)
                .HasForeignKey(d => d.MaKh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PhieuDatSan_KhachHang");

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.PhieuDatSans)
                .HasForeignKey(d => d.MaNv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PhieuDatSan_NhanVien");
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

        modelBuilder.Entity<TaiKhoan>(entity =>
        {
            entity.HasKey(e => e.Username);

            entity.ToTable("TaiKhoan");

            entity.Property(e => e.Username)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<YeuCauDatSan>(entity =>
        {
            entity.HasKey(e => e.Stt);

            entity.ToTable("YeuCauDatSan");

            entity.Property(e => e.Stt).HasColumnName("STT");
            entity.Property(e => e.GhiChu).HasMaxLength(50);
            entity.Property(e => e.MaKh)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("MaKH");
            entity.Property(e => e.Thoigiandat).HasColumnType("datetime");
            entity.Property(e => e.TongTien).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.YeuCauDatSans)
                .HasForeignKey(d => d.MaKh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_YeuCauDatSan_KhachHang");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
