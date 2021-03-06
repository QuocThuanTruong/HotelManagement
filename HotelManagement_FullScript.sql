USE [master]
GO
/****** Object:  Database [HotelManagement]    Script Date: 7/7/2021 6:00:19 PM ******/
CREATE DATABASE [HotelManagement]
GO
USE [HotelManagement]
GO
ALTER DATABASE [HotelManagement] SET COMPATIBILITY_LEVEL = 150
GO
GO
ALTER DATABASE [HotelManagement] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [HotelManagement] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [HotelManagement] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [HotelManagement] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [HotelManagement] SET ARITHABORT OFF 
GO
ALTER DATABASE [HotelManagement] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [HotelManagement] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [HotelManagement] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [HotelManagement] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [HotelManagement] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [HotelManagement] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [HotelManagement] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [HotelManagement] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [HotelManagement] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [HotelManagement] SET  DISABLE_BROKER 
GO
ALTER DATABASE [HotelManagement] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [HotelManagement] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [HotelManagement] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [HotelManagement] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [HotelManagement] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [HotelManagement] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [HotelManagement] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [HotelManagement] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [HotelManagement] SET  MULTI_USER 
GO
ALTER DATABASE [HotelManagement] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [HotelManagement] SET DB_CHAINING OFF 
GO
ALTER DATABASE [HotelManagement] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [HotelManagement] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [HotelManagement] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [HotelManagement] SET QUERY_STORE = OFF
GO
USE [HotelManagement]
GO
/****** Object:  UserDefinedFunction [dbo].[func_AuthenticateUser]    Script Date: 7/7/2021 6:00:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[func_AuthenticateUser](@username varchar(20), @password varchar(20))
RETURNS int
AS
BEGIN
	IF (EXISTS (SELECT * FROM DBO.NhanVien WHERE Username = @username AND Password = @password AND Active = 'true'))
	BEGIN
		RETURN 1
	END

	RETURN 0
END
GO
/****** Object:  UserDefinedFunction [dbo].[func_getAllRoom]    Script Date: 7/7/2021 6:00:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[func_getAllRoom]()
RETURNS @result table(
		[SoPhong] [int] NOT NULL,
		[ID_LoaiPhong] [int] NULL,
		[TinhTrang] [bit] NULL,
		[GhiChu] [nvarchar](100) NULL,
		[TenLoaiPhong] [nvarchar](50) NULL,
		[DonGia] [float] NULL,
		[SLKhachToiDa] [int] NULL,
		[Active] [bit] NULL)
AS
BEGIN
	INSERT INTO @result ([SoPhong], [ID_LoaiPhong], [TinhTrang],[GhiChu],[TenLoaiPhong],[DonGia],[SLKhachToiDa], [Active])
	SELECT P.SoPhong, P.ID_LoaiPhong, P.TinhTrang, P.GhiChu, LP.TenLoaiPhong, LP.DonGia, LP.SLKhachToiDa, P.Active FROM dbo.Phong P, dbo.LoaiPhong LP WHERE P.ID_LoaiPhong = LP.ID_LoaiPhong AND P.Active = 1

	RETURN
END
GO
/****** Object:  UserDefinedFunction [dbo].[func_GetCheckin]    Script Date: 7/7/2021 6:00:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[func_GetCheckin]()
RETURNS int
AS
BEGIN
	DECLARE @num int
	SET @num = (SELECT COUNT(*) FROM DBO.PhieuThue WHERE CONVERT(date, NgayBatDau) = CONVERT(date, getdate()))

	RETURN @num;
END
GO
/****** Object:  UserDefinedFunction [dbo].[func_GetCheckout]    Script Date: 7/7/2021 6:00:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[func_GetCheckout]()
RETURNS int
AS
BEGIN
	DECLARE @num int
	SET @num = (SELECT COUNT(*) FROM DBO.HoaDon WHERE CONVERT(date, NgayTraPhong) = CONVERT(date, getdate()))

	RETURN @num;
END
GO
/****** Object:  UserDefinedFunction [dbo].[func_GetNumEmpty]    Script Date: 7/7/2021 6:00:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[func_GetNumEmpty]()
RETURNS int
AS
BEGIN
	DECLARE @num int
	SET @num = (SELECT COUNT(*) FROM DBO.Phong WHERE TinhTrang = 0)

	RETURN @num;
END
GO
/****** Object:  UserDefinedFunction [dbo].[func_GetNumRenting]    Script Date: 7/7/2021 6:00:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[func_GetNumRenting]()
RETURNS int
AS
BEGIN
	DECLARE @num int
	SET @num = (SELECT COUNT(*) FROM DBO.Phong WHERE TinhTrang = 1)

	RETURN @num;
END
GO
/****** Object:  UserDefinedFunction [dbo].[func_GetRentBillFactor]    Script Date: 7/7/2021 6:00:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[func_GetRentBillFactor](@ID_PhieuThue int)
RETURNS float
AS
BEGIN
	DECLARE @num float;
	SET @num = (select MAX(HeSo) from dbo.KhachHang KH, dbo.ChiTietPhieuThue CT, DBO.LoaiKhach LK WHERE 
				CT.ID_PhieuThue = @ID_PhieuThue and KH.ID_KhachHang = CT.ID_KhachHang AND LK.ID_LoaiKhach = KH.ID_LoaiKhach)
	RETURN @num
END
GO
/****** Object:  UserDefinedFunction [dbo].[func_GetRevenueByRoomCat]    Script Date: 7/7/2021 6:00:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[func_GetRevenueByRoomCat](@roomCat int, @month int)
RETURNS int
AS
BEGIN
	DECLARE @num int
	SET @num = (SELECT CAST(SUM(H.TongTien) AS int) FROM dbo.Phong P, dbo.HoaDon H, dbo.ChiTietPhieuThue CT
				WHERE MONTH(H.NgayTraPhong) = @month AND H.ID_PhieuThue = CT.ID_PhieuThue AND CT.SoPhong = P.SoPhong AND P.ID_LoaiPhong = @roomCat )

	RETURN @num;
END
GO
/****** Object:  UserDefinedFunction [dbo].[func_GetRoomDensity]    Script Date: 7/7/2021 6:00:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[func_GetRoomDensity](@room int, @month int)
RETURNS int
AS
BEGIN
	DECLARE @num int
	DECLARE @num2 int

	SET @num = (SELECT COUNT(*) FROM DBO.PhieuThue PT, dbo.ChiTietPhieuThue C  
				WHERE PT.ID_PhieuThue = C.ID_PhieuThue 
				AND MONTH(PT.NgayBatDau) = @month AND C.SoPhong = @room)

	SET @num2 = (SELECT COUNT(*) FROM dbo.ChiTietPhieuThue C, dbo.HoaDon HD
				WHERE C.ID_PhieuThue = hd.ID_PhieuThue and hd.ID_KhachHang IN (SELECT ID_KhachHang FROM dbo.ChiTietPhieuThue)
				AND MONTH(hd.NgayTraPhong) = @month AND C.SoPhong = @room)

	IF (@num > @num2) 
		RETURN @num;

		
		
	RETURN @num2;

END
GO
/****** Object:  Table [dbo].[KhachHang]    Script Date: 7/7/2021 6:00:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[KhachHang](
	[ID_KhachHang] [int] NOT NULL,
	[HoTen] [nvarchar](50) NULL,
	[CMND] [varchar](15) NULL,
	[DiaChi] [nvarchar](100) NULL,
	[ID_LoaiKhach] [int] NOT NULL,
 CONSTRAINT [PK_KhachHang] PRIMARY KEY CLUSTERED 
(
	[ID_KhachHang] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ChiTietPhieuThue]    Script Date: 7/7/2021 6:00:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChiTietPhieuThue](
	[ID_KhachHang] [int] NOT NULL,
	[ID_PhieuThue] [int] NOT NULL,
	[SoPhong] [int] NOT NULL,
	[ID_NhanVien] [int] NOT NULL,
	[Active] [bit] NULL,
 CONSTRAINT [PK_ChiTietPhieuThue] PRIMARY KEY CLUSTERED 
(
	[ID_KhachHang] ASC,
	[ID_PhieuThue] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  UserDefinedFunction [dbo].[func_GetCustomerInRoom]    Script Date: 7/7/2021 6:00:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[func_GetCustomerInRoom](@ID_PhieuThue int)
RETURNS table
AS
	return (select kh.HoTen from dbo.KhachHang KH, dbo.ChiTietPhieuThue CT WHERE CT.ID_PhieuThue = @ID_PhieuThue and KH.ID_KhachHang = CT.ID_KhachHang)
GO
/****** Object:  Table [dbo].[CauHinh]    Script Date: 7/7/2021 6:00:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CauHinh](
	[ID] [int] NOT NULL,
	[TenThuocTinh] [varchar](20) NULL,
	[KieuDuLieu] [varchar](20) NULL,
	[GiaTri] [varchar](20) NULL,
	[DieuKien] [varchar](20) NULL,
 CONSTRAINT [PK_CauHinh] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[HoaDon]    Script Date: 7/7/2021 6:00:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HoaDon](
	[ID_HoaDon] [int] NOT NULL,
	[ID_PhieuThue] [int] NOT NULL,
	[ID_NhanVien] [int] NOT NULL,
	[ID_KhachHang] [int] NOT NULL,
	[NgayTraPhong] [datetime] NULL,
	[TongTien] [float] NULL,
	[Active] [bit] NULL,
 CONSTRAINT [PK_HoaDon] PRIMARY KEY CLUSTERED 
(
	[ID_HoaDon] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LoaiKhach]    Script Date: 7/7/2021 6:00:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LoaiKhach](
	[ID_LoaiKhach] [int] NOT NULL,
	[TenLoaiKhach] [nvarchar](50) NULL,
	[HeSo] [float] NULL,
	[Active] [bit] NULL,
 CONSTRAINT [PK_LoaiKhach] PRIMARY KEY CLUSTERED 
(
	[ID_LoaiKhach] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LoaiPhong]    Script Date: 7/7/2021 6:00:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LoaiPhong](
	[ID_LoaiPhong] [int] NOT NULL,
	[TenLoaiPhong] [nvarchar](50) NULL,
	[DonGia] [float] NULL,
	[SLKhachToiDa] [int] NULL,
	[Active] [bit] NULL,
 CONSTRAINT [PK_LoaiPhong] PRIMARY KEY CLUSTERED 
(
	[ID_LoaiPhong] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NhanVien]    Script Date: 7/7/2021 6:00:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NhanVien](
	[ID_NhanVien] [int] NOT NULL,
	[HoTen] [nvarchar](50) NULL,
	[CMND] [varchar](15) NULL,
	[LoaiNhanVien] [bit] NULL,
	[Username] [varchar](20) NULL,
	[Password] [varchar](256) NULL,
	[Active] [bit] NOT NULL,
 CONSTRAINT [PK_NhanVien] PRIMARY KEY CLUSTERED 
(
	[ID_NhanVien] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PhieuThue]    Script Date: 7/7/2021 6:00:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PhieuThue](
	[ID_PhieuThue] [int] NOT NULL,
	[NgayBatDau] [datetime] NULL,
	[Active] [int] NOT NULL,
 CONSTRAINT [PK_PhieuThue] PRIMARY KEY CLUSTERED 
(
	[ID_PhieuThue] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Phong]    Script Date: 7/7/2021 6:00:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Phong](
	[SoPhong] [int] NOT NULL,
	[ID_LoaiPhong] [int] NULL,
	[TinhTrang] [bit] NULL,
	[GhiChu] [nvarchar](100) NULL,
	[Active] [bit] NULL,
 CONSTRAINT [PK_Phong] PRIMARY KEY CLUSTERED 
(
	[SoPhong] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[CauHinh] ([ID], [TenThuocTinh], [KieuDuLieu], [GiaTri], [DieuKien]) VALUES (1, N'TiLePhuThu', N'float', N'0.25', N'>=3')
GO
INSERT [dbo].[ChiTietPhieuThue] ([ID_KhachHang], [ID_PhieuThue], [SoPhong], [ID_NhanVien], [Active]) VALUES (1, 1, 100, 3, 1)
INSERT [dbo].[ChiTietPhieuThue] ([ID_KhachHang], [ID_PhieuThue], [SoPhong], [ID_NhanVien], [Active]) VALUES (3, 2, 201, 2, 1)
INSERT [dbo].[ChiTietPhieuThue] ([ID_KhachHang], [ID_PhieuThue], [SoPhong], [ID_NhanVien], [Active]) VALUES (4, 3, 203, 2, 1)
INSERT [dbo].[ChiTietPhieuThue] ([ID_KhachHang], [ID_PhieuThue], [SoPhong], [ID_NhanVien], [Active]) VALUES (5, 4, 302, 2, 1)
INSERT [dbo].[ChiTietPhieuThue] ([ID_KhachHang], [ID_PhieuThue], [SoPhong], [ID_NhanVien], [Active]) VALUES (8, 5, 303, 3, 1)
INSERT [dbo].[ChiTietPhieuThue] ([ID_KhachHang], [ID_PhieuThue], [SoPhong], [ID_NhanVien], [Active]) VALUES (10, 6, 400, 2, 1)
INSERT [dbo].[ChiTietPhieuThue] ([ID_KhachHang], [ID_PhieuThue], [SoPhong], [ID_NhanVien], [Active]) VALUES (12, 7, 402, 4, 1)
INSERT [dbo].[ChiTietPhieuThue] ([ID_KhachHang], [ID_PhieuThue], [SoPhong], [ID_NhanVien], [Active]) VALUES (14, 8, 203, 4, 1)
INSERT [dbo].[ChiTietPhieuThue] ([ID_KhachHang], [ID_PhieuThue], [SoPhong], [ID_NhanVien], [Active]) VALUES (15, 10, 103, 3, 1)
INSERT [dbo].[ChiTietPhieuThue] ([ID_KhachHang], [ID_PhieuThue], [SoPhong], [ID_NhanVien], [Active]) VALUES (16, 9, 102, 2, 1)
INSERT [dbo].[ChiTietPhieuThue] ([ID_KhachHang], [ID_PhieuThue], [SoPhong], [ID_NhanVien], [Active]) VALUES (17, 10, 103, 3, 1)
INSERT [dbo].[ChiTietPhieuThue] ([ID_KhachHang], [ID_PhieuThue], [SoPhong], [ID_NhanVien], [Active]) VALUES (18, 11, 401, 1, 1)
INSERT [dbo].[ChiTietPhieuThue] ([ID_KhachHang], [ID_PhieuThue], [SoPhong], [ID_NhanVien], [Active]) VALUES (19, 12, 103, 1, 1)
INSERT [dbo].[ChiTietPhieuThue] ([ID_KhachHang], [ID_PhieuThue], [SoPhong], [ID_NhanVien], [Active]) VALUES (20, 13, 101, 1, 1)
INSERT [dbo].[ChiTietPhieuThue] ([ID_KhachHang], [ID_PhieuThue], [SoPhong], [ID_NhanVien], [Active]) VALUES (21, 13, 101, 1, 1)
INSERT [dbo].[ChiTietPhieuThue] ([ID_KhachHang], [ID_PhieuThue], [SoPhong], [ID_NhanVien], [Active]) VALUES (22, 14, 100, 1, 1)
INSERT [dbo].[ChiTietPhieuThue] ([ID_KhachHang], [ID_PhieuThue], [SoPhong], [ID_NhanVien], [Active]) VALUES (23, 15, 100, 1, 1)
INSERT [dbo].[ChiTietPhieuThue] ([ID_KhachHang], [ID_PhieuThue], [SoPhong], [ID_NhanVien], [Active]) VALUES (24, 16, 200, 1, 1)
GO
INSERT [dbo].[HoaDon] ([ID_HoaDon], [ID_PhieuThue], [ID_NhanVien], [ID_KhachHang], [NgayTraPhong], [TongTien], [Active]) VALUES (1, 1, 2, 1, CAST(N'2021-06-12T00:00:00.000' AS DateTime), 500000, 1)
INSERT [dbo].[HoaDon] ([ID_HoaDon], [ID_PhieuThue], [ID_NhanVien], [ID_KhachHang], [NgayTraPhong], [TongTien], [Active]) VALUES (2, 2, 3, 3, CAST(N'2021-06-17T00:00:00.000' AS DateTime), 600000, 1)
INSERT [dbo].[HoaDon] ([ID_HoaDon], [ID_PhieuThue], [ID_NhanVien], [ID_KhachHang], [NgayTraPhong], [TongTien], [Active]) VALUES (3, 3, 2, 4, CAST(N'2021-06-17T00:00:00.000' AS DateTime), 1000000, 1)
INSERT [dbo].[HoaDon] ([ID_HoaDon], [ID_PhieuThue], [ID_NhanVien], [ID_KhachHang], [NgayTraPhong], [TongTien], [Active]) VALUES (4, 4, 1, 5, CAST(N'2021-06-17T00:00:00.000' AS DateTime), 400000, 1)
INSERT [dbo].[HoaDon] ([ID_HoaDon], [ID_PhieuThue], [ID_NhanVien], [ID_KhachHang], [NgayTraPhong], [TongTien], [Active]) VALUES (5, 5, 4, 8, CAST(N'2021-06-17T00:00:00.000' AS DateTime), 600000, 1)
INSERT [dbo].[HoaDon] ([ID_HoaDon], [ID_PhieuThue], [ID_NhanVien], [ID_KhachHang], [NgayTraPhong], [TongTien], [Active]) VALUES (6, 6, 4, 10, CAST(N'2021-06-17T00:00:00.000' AS DateTime), 2000000, 1)
INSERT [dbo].[HoaDon] ([ID_HoaDon], [ID_PhieuThue], [ID_NhanVien], [ID_KhachHang], [NgayTraPhong], [TongTien], [Active]) VALUES (7, 7, 2, 12, CAST(N'2021-06-17T00:00:00.000' AS DateTime), 1200000, 1)
INSERT [dbo].[HoaDon] ([ID_HoaDon], [ID_PhieuThue], [ID_NhanVien], [ID_KhachHang], [NgayTraPhong], [TongTien], [Active]) VALUES (8, 8, 2, 14, CAST(N'2021-06-17T00:00:00.000' AS DateTime), 2300000, 1)
INSERT [dbo].[HoaDon] ([ID_HoaDon], [ID_PhieuThue], [ID_NhanVien], [ID_KhachHang], [NgayTraPhong], [TongTien], [Active]) VALUES (9, 9, 1, 16, CAST(N'2021-06-17T00:00:00.000' AS DateTime), 700000, 1)
INSERT [dbo].[HoaDon] ([ID_HoaDon], [ID_PhieuThue], [ID_NhanVien], [ID_KhachHang], [NgayTraPhong], [TongTien], [Active]) VALUES (10, 10, 2, 15, CAST(N'2021-06-17T00:00:00.000' AS DateTime), 1200000, 1)
INSERT [dbo].[HoaDon] ([ID_HoaDon], [ID_PhieuThue], [ID_NhanVien], [ID_KhachHang], [NgayTraPhong], [TongTien], [Active]) VALUES (11, 10, 1, 15, CAST(N'2021-06-20T14:24:24.000' AS DateTime), 750000, 1)
INSERT [dbo].[HoaDon] ([ID_HoaDon], [ID_PhieuThue], [ID_NhanVien], [ID_KhachHang], [NgayTraPhong], [TongTien], [Active]) VALUES (12, 13, 1, 21, CAST(N'2021-06-24T23:46:17.000' AS DateTime), 450000, 1)
INSERT [dbo].[HoaDon] ([ID_HoaDon], [ID_PhieuThue], [ID_NhanVien], [ID_KhachHang], [NgayTraPhong], [TongTien], [Active]) VALUES (13, 14, 1, 22, CAST(N'2021-06-24T23:59:05.000' AS DateTime), 300000, 1)
INSERT [dbo].[HoaDon] ([ID_HoaDon], [ID_PhieuThue], [ID_NhanVien], [ID_KhachHang], [NgayTraPhong], [TongTien], [Active]) VALUES (14, 15, 1, 23, CAST(N'2021-07-04T19:40:37.000' AS DateTime), 3000000, 1)
INSERT [dbo].[HoaDon] ([ID_HoaDon], [ID_PhieuThue], [ID_NhanVien], [ID_KhachHang], [NgayTraPhong], [TongTien], [Active]) VALUES (15, 16, 1, 24, CAST(N'2021-07-04T19:44:31.000' AS DateTime), 750000, 1)
INSERT [dbo].[HoaDon] ([ID_HoaDon], [ID_PhieuThue], [ID_NhanVien], [ID_KhachHang], [NgayTraPhong], [TongTien], [Active]) VALUES (16, 11, 1, 18, CAST(N'2021-07-04T00:00:00.000' AS DateTime), 750000, 1)
GO
INSERT [dbo].[KhachHang] ([ID_KhachHang], [HoTen], [CMND], [DiaChi], [ID_LoaiKhach]) VALUES (1, N'Thái Thanh Toàn', N'2781876184', N'986, Ấp Yến, Xã 1, Huyện Khoa Hậu Dương Hậu Giang', 1)
INSERT [dbo].[KhachHang] ([ID_KhachHang], [HoTen], [CMND], [DiaChi], [ID_LoaiKhach]) VALUES (2, N'Phan Duy Thạch', N'5073308781', N'03, Ấp Sa Lý, Thôn Phong Thương, Quận Khuyên Mẫn Đồng Nai', 1)
INSERT [dbo].[KhachHang] ([ID_KhachHang], [HoTen], [CMND], [DiaChi], [ID_LoaiKhach]) VALUES (3, N'Nguyễn Thái Hòa', N'4241085238', N'62 Phố Tiếp Huệ Chiêu, Ấp Toàn Đức, Quận Hảo Bình Định', 1)
INSERT [dbo].[KhachHang] ([ID_KhachHang], [HoTen], [CMND], [DiaChi], [ID_LoaiKhach]) VALUES (4, N'Đặng Gia Ðức', N'9061310554', N'87, Thôn Tăng Tiếp, Thôn Khoa Mi, Huyện Dinh Tuyết An Giang', 1)
INSERT [dbo].[KhachHang] ([ID_KhachHang], [HoTen], [CMND], [DiaChi], [ID_LoaiKhach]) VALUES (5, N'Dương Mộng Hương', N'2981575355', N'7846, Thôn Lộc Yên, Phường 5, Quận Hào Thời Thừa Thiên Huế', 1)
INSERT [dbo].[KhachHang] ([ID_KhachHang], [HoTen], [CMND], [DiaChi], [ID_LoaiKhach]) VALUES (6, N'Uất Hoài Việt', N'5267855661', N'66, Thôn Nhã, Xã Đồng Thuận, Quận Thể Thực Thanh Hóa', 1)
INSERT [dbo].[KhachHang] ([ID_KhachHang], [HoTen], [CMND], [DiaChi], [ID_LoaiKhach]) VALUES (7, N'Lê Hồng Thắm', N'0700407884', N'685 Phố Lạc Hiệp Đạt, Thôn Lỡ Vu, Quận Trung Bảo Hòa Bình', 1)
INSERT [dbo].[KhachHang] ([ID_KhachHang], [HoTen], [CMND], [DiaChi], [ID_LoaiKhach]) VALUES (8, N'Thạch Thu Hằng', N'3602102567', N'251 Kuvalis Plain Apt. 971 Cecilefort, CA', 2)
INSERT [dbo].[KhachHang] ([ID_KhachHang], [HoTen], [CMND], [DiaChi], [ID_LoaiKhach]) VALUES (9, N'Gennaro Larson', N'4728620552', N'251 Kuvalis Plain Apt. 971 Cecilefort, CA', 2)
INSERT [dbo].[KhachHang] ([ID_KhachHang], [HoTen], [CMND], [DiaChi], [ID_LoaiKhach]) VALUES (10, N'Brady Konopelski Jr', N'9245895566', N'56786 Derick Terrace Port Marty', 2)
INSERT [dbo].[KhachHang] ([ID_KhachHang], [HoTen], [CMND], [DiaChi], [ID_LoaiKhach]) VALUES (11, N'Rebeca Sauer', N'8187011388', N'20370 Reinger Highway Romaguerabury, AL', 2)
INSERT [dbo].[KhachHang] ([ID_KhachHang], [HoTen], [CMND], [DiaChi], [ID_LoaiKhach]) VALUES (12, N'Pete Schuppe', N'3116588358', N'87487 Strosin Harbors North Yvette, GA', 2)
INSERT [dbo].[KhachHang] ([ID_KhachHang], [HoTen], [CMND], [DiaChi], [ID_LoaiKhach]) VALUES (13, N'Hoàng Bích Hà', N'3098396443', N'98 Phố Bành Khai Vinh, Thôn Lộc Thường, Huyện Chiêm Quảng Bình', 1)
INSERT [dbo].[KhachHang] ([ID_KhachHang], [HoTen], [CMND], [DiaChi], [ID_LoaiKhach]) VALUES (14, N'Lý Khánh Vân', N'7798709219', N'2 Phố Bình Lý Quyên, Phường Nhượng Thúc, Quận Tuyến Bùi Vĩnh Long', 1)
INSERT [dbo].[KhachHang] ([ID_KhachHang], [HoTen], [CMND], [DiaChi], [ID_LoaiKhach]) VALUES (15, N'Eloy Hettinger', N'1899367599', N'5077 Melba Lake Lucienneside', 2)
INSERT [dbo].[KhachHang] ([ID_KhachHang], [HoTen], [CMND], [DiaChi], [ID_LoaiKhach]) VALUES (16, N'Phan Ngọc Ánh', N'4092076415', N'72, Thôn Sơn, Ấp Mang Phượng, Huyện 04 Bến Tre', 1)
INSERT [dbo].[KhachHang] ([ID_KhachHang], [HoTen], [CMND], [DiaChi], [ID_LoaiKhach]) VALUES (17, N'Dominic Smitham IV', N'6434336748', N'5477 Weimann Mission East Franceston', 2)
INSERT [dbo].[KhachHang] ([ID_KhachHang], [HoTen], [CMND], [DiaChi], [ID_LoaiKhach]) VALUES (18, N'Tran Kien Quoc', N'1234567890', N'Quận Tân Bình, TP. Hồ Chí Minh', 1)
INSERT [dbo].[KhachHang] ([ID_KhachHang], [HoTen], [CMND], [DiaChi], [ID_LoaiKhach]) VALUES (19, N'Nguyễn Tân Vinh', N'33453535633', N'KTX Khu B, ĐHQG TP.Hồ Chí Minh', 1)
INSERT [dbo].[KhachHang] ([ID_KhachHang], [HoTen], [CMND], [DiaChi], [ID_LoaiKhach]) VALUES (20, N'Nguyễn Sư Phước', N'123455433', N'Nguyễn Trọng Tuyển, Tân Bình', 1)
INSERT [dbo].[KhachHang] ([ID_KhachHang], [HoTen], [CMND], [DiaChi], [ID_LoaiKhach]) VALUES (21, N'Hồ Hoàng Việt Tiến', N'965322232', N'KTX Khu B, ĐHQG TP.Hồ Chí Minh', 2)
INSERT [dbo].[KhachHang] ([ID_KhachHang], [HoTen], [CMND], [DiaChi], [ID_LoaiKhach]) VALUES (22, N'Trương Quốc Thuận', N'121301512', N'Mỹ Nhơn, Bến Tre', 1)
INSERT [dbo].[KhachHang] ([ID_KhachHang], [HoTen], [CMND], [DiaChi], [ID_LoaiKhach]) VALUES (23, N'Lê Nhật Tuấn', N'3454353453', N'Di Linh, Lâm Đồng', 1)
INSERT [dbo].[KhachHang] ([ID_KhachHang], [HoTen], [CMND], [DiaChi], [ID_LoaiKhach]) VALUES (24, N'Như Huỳnh', N'1234785963', N'Quận Bình Thạnh, TP Hồ Chí Minh', 2)
GO
INSERT [dbo].[LoaiKhach] ([ID_LoaiKhach], [TenLoaiKhach], [HeSo], [Active]) VALUES (1, N'Nội địa', 1, 1)
INSERT [dbo].[LoaiKhach] ([ID_LoaiKhach], [TenLoaiKhach], [HeSo], [Active]) VALUES (2, N'Nước ngoài', 1.5, 1)
GO
INSERT [dbo].[LoaiPhong] ([ID_LoaiPhong], [TenLoaiPhong], [DonGia], [SLKhachToiDa], [Active]) VALUES (1, N'Đơn', 300000, 2, 1)
INSERT [dbo].[LoaiPhong] ([ID_LoaiPhong], [TenLoaiPhong], [DonGia], [SLKhachToiDa], [Active]) VALUES (2, N'Đôi', 500000, 4, 1)
INSERT [dbo].[LoaiPhong] ([ID_LoaiPhong], [TenLoaiPhong], [DonGia], [SLKhachToiDa], [Active]) VALUES (3, N'VIP', 1000000, 8, 1)
GO
INSERT [dbo].[NhanVien] ([ID_NhanVien], [HoTen], [CMND], [LoaiNhanVien], [Username], [Password], [Active]) VALUES (1, N'Châu Bảo Quốc', N'5718585434', 1, N'QL_Quoc', N'$2y$10$GHwdFVU5wGads8lI/KwcRuqoi05PXY1XliYKagVBDR7SxZ4Y6EzAK', 1)
INSERT [dbo].[NhanVien] ([ID_NhanVien], [HoTen], [CMND], [LoaiNhanVien], [Username], [Password], [Active]) VALUES (2, N'Huỳnh Hà Nhi', N'9621868905', 1, N'QL_Nhi', N'$2y$10$GHwdFVU5wGads8lI/KwcRuqoi05PXY1XliYKagVBDR7SxZ4Y6EzAK', 1)
INSERT [dbo].[NhanVien] ([ID_NhanVien], [HoTen], [CMND], [LoaiNhanVien], [Username], [Password], [Active]) VALUES (3, N'Võ Hải Thụy', N'8691484828', 0, N'NV_Thuy', N'$2y$10$GHwdFVU5wGads8lI/KwcRuqoi05PXY1XliYKagVBDR7SxZ4Y6EzAK', 1)
INSERT [dbo].[NhanVien] ([ID_NhanVien], [HoTen], [CMND], [LoaiNhanVien], [Username], [Password], [Active]) VALUES (4, N'Nguyễn Huy Tường', N'6301729892', 0, N'NV_Tuong', N'$2y$10$GHwdFVU5wGads8lI/KwcRuqoi05PXY1XliYKagVBDR7SxZ4Y6EzAK', 1)
GO
INSERT [dbo].[PhieuThue] ([ID_PhieuThue], [NgayBatDau], [Active]) VALUES (1, CAST(N'2021-06-01T00:00:00.000' AS DateTime), 2)
INSERT [dbo].[PhieuThue] ([ID_PhieuThue], [NgayBatDau], [Active]) VALUES (2, CAST(N'2021-06-03T00:00:00.000' AS DateTime), 2)
INSERT [dbo].[PhieuThue] ([ID_PhieuThue], [NgayBatDau], [Active]) VALUES (3, CAST(N'2021-06-05T00:00:00.000' AS DateTime), 2)
INSERT [dbo].[PhieuThue] ([ID_PhieuThue], [NgayBatDau], [Active]) VALUES (4, CAST(N'2021-06-08T00:00:00.000' AS DateTime), 2)
INSERT [dbo].[PhieuThue] ([ID_PhieuThue], [NgayBatDau], [Active]) VALUES (5, CAST(N'2021-06-10T00:00:00.000' AS DateTime), 2)
INSERT [dbo].[PhieuThue] ([ID_PhieuThue], [NgayBatDau], [Active]) VALUES (6, CAST(N'2021-06-11T00:00:00.000' AS DateTime), 2)
INSERT [dbo].[PhieuThue] ([ID_PhieuThue], [NgayBatDau], [Active]) VALUES (7, CAST(N'2021-06-12T00:00:00.000' AS DateTime), 2)
INSERT [dbo].[PhieuThue] ([ID_PhieuThue], [NgayBatDau], [Active]) VALUES (8, CAST(N'2021-06-12T00:00:00.000' AS DateTime), 2)
INSERT [dbo].[PhieuThue] ([ID_PhieuThue], [NgayBatDau], [Active]) VALUES (9, CAST(N'2021-06-12T00:00:00.000' AS DateTime), 2)
INSERT [dbo].[PhieuThue] ([ID_PhieuThue], [NgayBatDau], [Active]) VALUES (10, CAST(N'2021-06-13T00:00:00.000' AS DateTime), 2)
INSERT [dbo].[PhieuThue] ([ID_PhieuThue], [NgayBatDau], [Active]) VALUES (11, CAST(N'2021-06-17T00:00:00.000' AS DateTime), 2)
INSERT [dbo].[PhieuThue] ([ID_PhieuThue], [NgayBatDau], [Active]) VALUES (12, CAST(N'2021-06-17T00:00:00.000' AS DateTime), 2)
INSERT [dbo].[PhieuThue] ([ID_PhieuThue], [NgayBatDau], [Active]) VALUES (13, CAST(N'2021-06-24T23:44:59.000' AS DateTime), 2)
INSERT [dbo].[PhieuThue] ([ID_PhieuThue], [NgayBatDau], [Active]) VALUES (14, CAST(N'2021-06-24T23:58:38.000' AS DateTime), 2)
INSERT [dbo].[PhieuThue] ([ID_PhieuThue], [NgayBatDau], [Active]) VALUES (15, CAST(N'2021-06-25T00:48:19.000' AS DateTime), 2)
INSERT [dbo].[PhieuThue] ([ID_PhieuThue], [NgayBatDau], [Active]) VALUES (16, CAST(N'2021-07-04T19:43:22.000' AS DateTime), 2)
GO
INSERT [dbo].[Phong] ([SoPhong], [ID_LoaiPhong], [TinhTrang], [GhiChu], [Active]) VALUES (100, 1, 0, N'Phòng không hư hỏng', 1)
INSERT [dbo].[Phong] ([SoPhong], [ID_LoaiPhong], [TinhTrang], [GhiChu], [Active]) VALUES (101, 1, 0, N'Phòng không hư hỏng', 1)
INSERT [dbo].[Phong] ([SoPhong], [ID_LoaiPhong], [TinhTrang], [GhiChu], [Active]) VALUES (102, 2, 0, N'Phòng không hư hỏng', 1)
INSERT [dbo].[Phong] ([SoPhong], [ID_LoaiPhong], [TinhTrang], [GhiChu], [Active]) VALUES (103, 2, 0, N'Phòng thiếu 1 bàn', 1)
INSERT [dbo].[Phong] ([SoPhong], [ID_LoaiPhong], [TinhTrang], [GhiChu], [Active]) VALUES (200, 2, 0, N'Phòng không hư hỏng', 1)
INSERT [dbo].[Phong] ([SoPhong], [ID_LoaiPhong], [TinhTrang], [GhiChu], [Active]) VALUES (201, 2, 0, N'Phòng không hư hỏng', 1)
INSERT [dbo].[Phong] ([SoPhong], [ID_LoaiPhong], [TinhTrang], [GhiChu], [Active]) VALUES (202, 1, 0, N'Phòng không hư hỏng', 1)
INSERT [dbo].[Phong] ([SoPhong], [ID_LoaiPhong], [TinhTrang], [GhiChu], [Active]) VALUES (203, 1, 0, N'Phòng không hư hỏng', 1)
INSERT [dbo].[Phong] ([SoPhong], [ID_LoaiPhong], [TinhTrang], [GhiChu], [Active]) VALUES (300, 3, 0, N'Phòng thiếu 1 ghế', 1)
INSERT [dbo].[Phong] ([SoPhong], [ID_LoaiPhong], [TinhTrang], [GhiChu], [Active]) VALUES (301, 3, 0, N'Phòng không hư hỏng', 1)
INSERT [dbo].[Phong] ([SoPhong], [ID_LoaiPhong], [TinhTrang], [GhiChu], [Active]) VALUES (302, 2, 0, N'Phòng không hư hỏng', 1)
INSERT [dbo].[Phong] ([SoPhong], [ID_LoaiPhong], [TinhTrang], [GhiChu], [Active]) VALUES (303, 2, 0, N'Phòng không hư hỏng', 1)
INSERT [dbo].[Phong] ([SoPhong], [ID_LoaiPhong], [TinhTrang], [GhiChu], [Active]) VALUES (400, 3, 0, N'Phòng không hư hỏng', 1)
INSERT [dbo].[Phong] ([SoPhong], [ID_LoaiPhong], [TinhTrang], [GhiChu], [Active]) VALUES (401, 3, 0, N'Phòng không hư hỏng', 1)
INSERT [dbo].[Phong] ([SoPhong], [ID_LoaiPhong], [TinhTrang], [GhiChu], [Active]) VALUES (402, 3, 0, N'Phòng không hư hỏng', 1)
INSERT [dbo].[Phong] ([SoPhong], [ID_LoaiPhong], [TinhTrang], [GhiChu], [Active]) VALUES (403, 3, 0, N'Phòng không hư hỏng', 1)
GO
ALTER TABLE [dbo].[ChiTietPhieuThue] ADD  CONSTRAINT [DF_ChiTietPhieuThue_Active]  DEFAULT ((1)) FOR [Active]
GO
ALTER TABLE [dbo].[ChiTietPhieuThue]  WITH CHECK ADD  CONSTRAINT [FK_ChiTietPhieuThue_KhachHang] FOREIGN KEY([ID_KhachHang])
REFERENCES [dbo].[KhachHang] ([ID_KhachHang])
GO
ALTER TABLE [dbo].[ChiTietPhieuThue] CHECK CONSTRAINT [FK_ChiTietPhieuThue_KhachHang]
GO
ALTER TABLE [dbo].[ChiTietPhieuThue]  WITH CHECK ADD  CONSTRAINT [FK_ChiTietPhieuThue_NhanVien] FOREIGN KEY([ID_NhanVien])
REFERENCES [dbo].[NhanVien] ([ID_NhanVien])
GO
ALTER TABLE [dbo].[ChiTietPhieuThue] CHECK CONSTRAINT [FK_ChiTietPhieuThue_NhanVien]
GO
ALTER TABLE [dbo].[ChiTietPhieuThue]  WITH CHECK ADD  CONSTRAINT [FK_ChiTietPhieuThue_PhieuThue] FOREIGN KEY([ID_PhieuThue])
REFERENCES [dbo].[PhieuThue] ([ID_PhieuThue])
GO
ALTER TABLE [dbo].[ChiTietPhieuThue] CHECK CONSTRAINT [FK_ChiTietPhieuThue_PhieuThue]
GO
ALTER TABLE [dbo].[ChiTietPhieuThue]  WITH CHECK ADD  CONSTRAINT [FK_ChiTietPhieuThue_Phong] FOREIGN KEY([SoPhong])
REFERENCES [dbo].[Phong] ([SoPhong])
GO
ALTER TABLE [dbo].[ChiTietPhieuThue] CHECK CONSTRAINT [FK_ChiTietPhieuThue_Phong]
GO
ALTER TABLE [dbo].[HoaDon]  WITH CHECK ADD  CONSTRAINT [FK_HoaDon_NhanVien] FOREIGN KEY([ID_NhanVien])
REFERENCES [dbo].[NhanVien] ([ID_NhanVien])
GO
ALTER TABLE [dbo].[HoaDon] CHECK CONSTRAINT [FK_HoaDon_NhanVien]
GO
ALTER TABLE [dbo].[HoaDon]  WITH CHECK ADD  CONSTRAINT [FK_HoaDon_PhieuThue] FOREIGN KEY([ID_PhieuThue])
REFERENCES [dbo].[PhieuThue] ([ID_PhieuThue])
GO
ALTER TABLE [dbo].[HoaDon] CHECK CONSTRAINT [FK_HoaDon_PhieuThue]
GO
ALTER TABLE [dbo].[KhachHang]  WITH CHECK ADD  CONSTRAINT [FK_KhachHang_LoaiKhach] FOREIGN KEY([ID_LoaiKhach])
REFERENCES [dbo].[LoaiKhach] ([ID_LoaiKhach])
GO
ALTER TABLE [dbo].[KhachHang] CHECK CONSTRAINT [FK_KhachHang_LoaiKhach]
GO
ALTER TABLE [dbo].[Phong]  WITH CHECK ADD  CONSTRAINT [FK_Phong_LoaiPhong] FOREIGN KEY([ID_LoaiPhong])
REFERENCES [dbo].[LoaiPhong] ([ID_LoaiPhong])
GO
ALTER TABLE [dbo].[Phong] CHECK CONSTRAINT [FK_Phong_LoaiPhong]
GO
USE [master]
GO
ALTER DATABASE [HotelManagement] SET  READ_WRITE 
GO
