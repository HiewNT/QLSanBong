using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using QLSanBong.Models;

namespace QLSanBong.Data;

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

    public virtual DbSet<ChucNang> ChucNangs { get; set; }

    public virtual DbSet<DanhGia> DanhGia { get; set; }

    public virtual DbSet<GiaGioThue> GiaGioThues { get; set; }

    public virtual DbSet<KhachHang> KhachHangs { get; set; }

    public virtual DbSet<NhanVien> NhanViens { get; set; }

    public virtual DbSet<NhomNguoiDung> NhomNguoiDungs { get; set; }

    public virtual DbSet<NhomQuyenCn> NhomQuyenCns { get; set; }

    public virtual DbSet<PhieuDatSan> PhieuDatSans { get; set; }

    public virtual DbSet<Quyen> Quyens { get; set; }

    public virtual DbSet<SanBong> SanBongs { get; set; }

    public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }

    public virtual DbSet<YeuCauDatSan> YeuCauDatSans { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=hiew\\nthieu;Initial Catalog=QLSanBong;User ID=sa;Encrypt=false;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChiTietPd>(entity =>
        {
            entity.HasKey(e => new { e.MaPds, e.Magio, e.MaSb }).HasName("PK__ChiTietP__3AE048CAA0E04E9B");

            entity.ToTable("ChiTietPDS", tb => tb.HasTrigger("UpdateTongTienTrigger"));

            entity.Property(e => e.MaPds)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("MaPDS");
            entity.Property(e => e.Magio)
                .HasMaxLength(8)
                .IsUnicode(false);
            entity.Property(e => e.MaSb)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("MaSB");
            entity.Property(e => e.Ghichu).HasMaxLength(100);
            entity.Property(e => e.Giatien).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Ngaysudung).HasColumnType("date");

            entity.HasOne(d => d.MaPdsNavigation).WithMany(p => p.ChiTietPds)
                .HasForeignKey(d => d.MaPds)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChiTietPDS_PhieuDatSan");

            entity.HasOne(d => d.MaSbNavigation).WithMany(p => p.ChiTietPds)
                .HasForeignKey(d => d.MaSb)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChiTietPDS_SanBong");

            entity.HasOne(d => d.MagioNavigation).WithMany(p => p.ChiTietPds)
                .HasForeignKey(d => d.Magio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChiTietPDS_GiaGioThue");
        });

        modelBuilder.Entity<ChiTietYcd>(entity =>
        {
            entity.HasKey(e => new { e.Stt, e.MaSb, e.Magio });

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
            entity.Property(e => e.GiaTien).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Ngaysudung).HasColumnType("date");
            entity.Property(e => e.TrangThai).HasMaxLength(50);

            entity.HasOne(d => d.SttNavigation).WithMany(p => p.ChiTietYcds)
                .HasForeignKey(d => d.Stt)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChiTietYCDS_YeuCauDatSan");
        });

        modelBuilder.Entity<ChucNang>(entity =>
        {
            entity.HasKey(e => e.MaCn);

            entity.ToTable("ChucNang");

            entity.Property(e => e.MaCn)
                .ValueGeneratedNever()
                .HasColumnName("MaCN");
            entity.Property(e => e.TenCn)
                .HasMaxLength(5)
                .HasColumnName("TenCN");
        });

        modelBuilder.Entity<DanhGia>(entity =>
        {
            entity.HasKey(e => e.MaLuotDanhGia);

            entity.Property(e => e.MaSb)
                .HasMaxLength(8)
                .IsUnicode(false);
            entity.Property(e => e.NoiDung).HasColumnType("ntext");
            entity.Property(e => e.TenKh)
                .HasMaxLength(50)
                .HasColumnName("TenKH");
            entity.Property(e => e.ThoiGianGanhGia)
                .HasMaxLength(50)
                .IsUnicode(false)
                .IsFixedLength();
        });

        modelBuilder.Entity<GiaGioThue>(entity =>
        {
            entity.HasKey(e => e.Magio).HasName("PK__GiaGioTh__3455403EE3BA3574");

            entity.ToTable("GiaGioThue");

            entity.Property(e => e.Magio)
                .HasMaxLength(8)
                .IsUnicode(false);
            entity.Property(e => e.Dongia).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Ghichu).HasMaxLength(100);
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.MaKh).HasName("PK__KhachHan__2725CF1EE538993A");

            entity.ToTable("KhachHang");

            entity.Property(e => e.MaKh)
                .HasMaxLength(20)
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
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.TendangnhapNavigation).WithMany(p => p.KhachHangs)
                .HasForeignKey(d => d.Tendangnhap)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KhachHang__Tenda__4E88ABD4");
        });

        modelBuilder.Entity<NhanVien>(entity =>
        {
            entity.HasKey(e => e.MaNv).HasName("PK__NhanVien__2725D70A1D7EE5A5");

            entity.ToTable("NhanVien");

            entity.Property(e => e.MaNv)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("MaNV");
            entity.Property(e => e.Chucvu).HasMaxLength(20);
            entity.Property(e => e.Gioitinh).HasMaxLength(6);
            entity.Property(e => e.Ngaysinh).HasColumnType("date");
            entity.Property(e => e.Sdt)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("SDT");
            entity.Property(e => e.TenNv)
                .HasMaxLength(30)
                .HasColumnName("TenNV");
            entity.Property(e => e.Tendangnhap)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.TendangnhapNavigation).WithMany(p => p.NhanViens)
                .HasForeignKey(d => d.Tendangnhap)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__NhanVien__Tendan__48CFD27E");
        });

        modelBuilder.Entity<NhomNguoiDung>(entity =>
        {
            entity.HasKey(e => e.MaNhom);

            entity.ToTable("NhomNguoiDung");

            entity.Property(e => e.MaNhom).ValueGeneratedNever();
            entity.Property(e => e.TenNhom).HasMaxLength(50);
        });

        modelBuilder.Entity<NhomQuyenCn>(entity =>
        {
            entity.HasKey(e => new { e.MaQuyen, e.MaNhom, e.MaCn });

            entity.ToTable("Nhom_QuyenCN");

            entity.Property(e => e.MaQuyen)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MaCn).HasColumnName("MaCN");
            entity.Property(e => e.GhiChu)
                .HasMaxLength(10)
                .IsFixedLength();

            entity.HasOne(d => d.MaCnNavigation).WithMany(p => p.NhomQuyenCns)
                .HasForeignKey(d => d.MaCn)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Nhom_QuyenCN_ChucNang");

            entity.HasOne(d => d.MaNhomNavigation).WithMany(p => p.NhomQuyenCns)
                .HasForeignKey(d => d.MaNhom)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Nhom_QuyenCN_NhomNguoiDung");

            entity.HasOne(d => d.MaQuyenNavigation).WithMany(p => p.NhomQuyenCns)
                .HasForeignKey(d => d.MaQuyen)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Nhom_QuyenCN_Quyen");
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
            entity.Property(e => e.MaNv)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("MaNV");
            entity.Property(e => e.Ngaylap).HasColumnType("datetime");
            entity.Property(e => e.Phuongthuctt).HasMaxLength(50);
            entity.Property(e => e.Sdt)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("SDT");
            entity.Property(e => e.Sttds).HasColumnName("sttds");
            entity.Property(e => e.TenKh)
                .HasMaxLength(50)
                .HasColumnName("TenKH");
            entity.Property(e => e.TongTien).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.PhieuDatSans)
                .HasForeignKey(d => d.MaNv)
                .HasConstraintName("FK_PhieuDatSan_NhanVien");
        });

        modelBuilder.Entity<Quyen>(entity =>
        {
            entity.HasKey(e => e.MaQuyen);

            entity.ToTable("Quyen");

            entity.Property(e => e.MaQuyen)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.TenQuyen)
                .HasMaxLength(100)
                .HasColumnName("Ten Quyen");
        });

        modelBuilder.Entity<SanBong>(entity =>
        {
            entity.HasKey(e => e.MaSb).HasName("PK__SanBong__2725080EEA944BF7");

            entity.ToTable("SanBong");

            entity.Property(e => e.MaSb)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("MaSB");
            entity.Property(e => e.DiaChi).HasMaxLength(100);
            entity.Property(e => e.Dientich).HasMaxLength(20);
            entity.Property(e => e.Ghichu).HasColumnType("ntext");
            entity.Property(e => e.GiaSan)
                .HasMaxLength(50)
                .IsFixedLength();
            entity.Property(e => e.Hinhanh)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.LoaiSan).HasMaxLength(50);
            entity.Property(e => e.TenSb)
                .HasMaxLength(30)
                .HasColumnName("TenSB");
            entity.Property(e => e.Trangthai).HasMaxLength(20);
        });

        modelBuilder.Entity<TaiKhoan>(entity =>
        {
            entity.HasKey(e => e.Username);

            entity.ToTable("TaiKhoan");

            entity.Property(e => e.Username)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.Quyen)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<YeuCauDatSan>(entity =>
        {
            entity.HasKey(e => e.Stt);

            entity.ToTable("YeuCauDatSan");

            entity.Property(e => e.Stt).HasColumnName("STT");
            entity.Property(e => e.GhiChu).HasMaxLength(50);
            entity.Property(e => e.Phuongthuctt).HasMaxLength(50);
            entity.Property(e => e.Sdt)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("SDT");
            entity.Property(e => e.Tennguoidat).HasMaxLength(30);
            entity.Property(e => e.Thoigiandat).HasColumnType("datetime");
            entity.Property(e => e.TongTien).HasColumnType("decimal(18, 0)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
