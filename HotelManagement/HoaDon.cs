//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HotelManagement
{
    using System;
    using System.Collections.Generic;
    
    public partial class HoaDon
    {
        public int ID_HoaDon { get; set; }
        public int ID_PhieuThue { get; set; }
        public int ID_NhanVien { get; set; }
        public Nullable<System.DateTime> NgayTraPhong { get; set; }
        public Nullable<double> TongTien { get; set; }
        public Nullable<bool> Active { get; set; }
    
        public virtual NhanVien NhanVien { get; set; }
        public virtual PhieuThue PhieuThue { get; set; }
    }
}