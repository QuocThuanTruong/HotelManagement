--Lay danh sach phong
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
	INSERT INTO @result ([SoPhong], [ID_LoaiPhong], [TinhTrang], [GhiChu], [TenLoaiPhong], [DonGia], [SLKhachToiDa], [Active])
	SELECT P.SoPhong, P.ID_LoaiPhong, P.TinhTrang, P.GhiChu, LP.TenLoaiPhong, LP.DonGia, LP.SLKhachToiDa, P.Active FROM dbo.Phong P, dbo.LoaiPhong LP WHERE P.ID_LoaiPhong = LP.ID_LoaiPhong AND P.Active = 1

	RETURN
END

select func_AuthenticateUser('a', 'b');

--Kiem tra dang nhap
CREATE FUNCTION func_AuthenticateUser(@username varchar(20), @password varchar(20))
RETURNS int
AS
BEGIN
	IF (EXISTS (SELECT * FROM DBO.NhanVien WHERE Username = @username AND Password = @password))
	BEGIN
		RETURN 1
	END

	RETURN 0
END
GO
--Lay so luong checkin
CREATE FUNCTION func_GetCheckin()
RETURNS int
AS
BEGIN
	DECLARE @num int
	SET @num = (SELECT COUNT(*) FROM DBO.PhieuThue WHERE CONVERT(date, NgayBatDau) = CONVERT(date, getdate()))

	RETURN @num;
END
GO
--Lay so luong checkout
CREATE FUNCTION func_GetCheckout()
RETURNS int
AS
BEGIN
	DECLARE @num int
	SET @num = (SELECT COUNT(*) FROM DBO.HoaDon WHERE CONVERT(date, NgayTraPhong) = CONVERT(date, getdate()))

	RETURN @num;
END
GO
--Lay so luong dang thue
CREATE FUNCTION func_GetNumRenting()
RETURNS int
AS
BEGIN
	DECLARE @num int
	SET @num = (SELECT COUNT(*) FROM DBO.Phong WHERE TinhTrang = 1)

	RETURN @num;
END
GO

--Lay so luong phong trong
CREATE FUNCTION func_GetNumEmpty()
RETURNS int
AS
BEGIN
	DECLARE @num int
	SET @num = (SELECT COUNT(*) FROM DBO.Phong WHERE TinhTrang = 0)

	RETURN @num;
END
GO

--Lay mat do su dung phong
CREATE FUNCTION func_GetRoomDensity(@room int, @month int)
RETURNS int
AS
BEGIN
	DECLARE @num int
	SET @num = (SELECT COUNT(*) FROM DBO.PhieuThue PT, dbo.ChiTietPhieuThue C  
				WHERE PT.ID_PhieuThue = C.ID_PhieuThue 
				AND MONTH(PT.NgayBatDau) = @month AND C.SoPhong = @room)

	RETURN @num;
END
GO

--Lay doanh thu theo loai phong
CREATE FUNCTION func_GetRevenueByRoomCat(@roomCat int, @month int)
RETURNS int
AS
BEGIN
	DECLARE @num int
	SET @num = (SELECT COUNT(*) FROM dbo.Phong P, dbo.HoaDon H, dbo.ChiTietPhieuThue CT
				WHERE MONTH(H.NgayTraPhong) = @month AND H.ID_PhieuThue = CT.ID_PhieuThue AND CT.SoPhong = P.SoPhong AND P.ID_LoaiPhong = @roomCat )

	RETURN @num;
END
GO

--Lay so luong nguoi thue trong 1 phong
--Phong dang thue, binding them id phieu thue
CREATE FUNCTION func_GetCustomerInRoom(@ID_PhieuThue int)
RETURNS table
AS
	return (select kh.HoTen from dbo.KhachHang KH, dbo.ChiTietPhieuThue CT WHERE CT.ID_PhieuThue = @ID_PhieuThue and KH.ID_KhachHang = CT.ID_KhachHang)
GO
--Lay he so cua phieu thue
CREATE FUNCTION func_GetRentBillFactor(@ID_PhieuThue int)
RETURNS float
AS
BEGIN
	DECLARE @num float;
	SET @num = (select MAX(HeSo) from dbo.KhachHang KH, dbo.ChiTietPhieuThue CT, DBO.LoaiKhach LK WHERE 
				CT.ID_PhieuThue = @ID_PhieuThue and KH.ID_KhachHang = CT.ID_KhachHang AND LK.ID_LoaiKhach = KH.ID_LoaiKhach)
	RETURN @num
END
GO

Select dbo.func_GetRentBillFactor(18)