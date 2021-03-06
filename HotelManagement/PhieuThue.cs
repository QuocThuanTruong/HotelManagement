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

    public partial class PhieuThue
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PhieuThue()
        {
            this.ChiTietPhieuThues = new HashSet<ChiTietPhieuThue>();
            this.HoaDons = new HashSet<HoaDon>();
        }

        public int ID_PhieuThue { get; set; }
        public Nullable<System.DateTime> NgayBatDau { get; set; }
        public int Active { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChiTietPhieuThue> ChiTietPhieuThues { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HoaDon> HoaDons { get; set; }

        public int TotalPrice { get; set; }

        //For Binding
        public int Total_Customer_For_Binding { get; set; }
        public int Total_Day_For_Binding { get; set; }
        public double Ratio_For_Binding { get; set; }
        public string Price_Per_Day_For_Binding { get; set; }
        public string Total_Price_For_Binding { get; set; }
        public int SoPhong_For_Binding { get; set; }
        public string TenNhanVienLapPhieu { get; set; }
        public string Status { get; set; }
        public int ID_NhanVien { get; set; }
        public string Visible_View_For_Bingding { get; set; }
        public string Visible_Edit_Delete_For_Bingding { get; set; }
    }
}