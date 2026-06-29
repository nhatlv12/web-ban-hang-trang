using App.Trang.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace App.Trang.Api.Data;

/// <summary>
/// Seed dữ liệu ban đầu từ Bảng sao kê Nhật Minh 2026
/// </summary>
public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        // Nếu đã có sản phẩm seed rồi thì bỏ qua
        if (await db.Products.AnyAsync(p => p.Code == "SP001")) return;

        // Xóa dữ liệu cũ theo thứ tự FK (con trước, cha sau)
        db.OrderDetails.RemoveRange(db.OrderDetails);
        db.Orders.RemoveRange(db.Orders);
        db.WareHouses.RemoveRange(db.WareHouses);
        db.Products.RemoveRange(db.Products);
        db.Customers.RemoveRange(db.Customers);
        db.Providers.RemoveRange(db.Providers);
        db.Categories.RemoveRange(db.Categories);
        await db.SaveChangesAsync();

        // Seed theo thứ tự dependency
        await SeedCategories(db);
        await SeedProviders(db);
        await SeedCustomers(db);
        await SeedProducts(db);
        await db.SaveChangesAsync();
    }

    // =============================================
    // PHẦN 1: DANH MỤC SẢN PHẨM
    // =============================================
    private static readonly Guid CatKhiNen   = Guid.Parse("a0000001-0000-0000-0000-000000000001");
    private static readonly Guid CatCuRoa    = Guid.Parse("a0000001-0000-0000-0000-000000000002");
    private static readonly Guid CatDungCu   = Guid.Parse("a0000001-0000-0000-0000-000000000003");
    private static readonly Guid CatDien     = Guid.Parse("a0000001-0000-0000-0000-000000000004");
    private static readonly Guid CatCoKhi    = Guid.Parse("a0000001-0000-0000-0000-000000000005");
    private static readonly Guid CatHoaChat  = Guid.Parse("a0000001-0000-0000-0000-000000000006");
    private static readonly Guid CatBaoHo    = Guid.Parse("a0000001-0000-0000-0000-000000000007");
    private static readonly Guid CatThietBi  = Guid.Parse("a0000001-0000-0000-0000-000000000008");

    private static async Task SeedCategories(AppDbContext db)
    {


        var categories = new List<Category>
        {
            new() { Id = CatKhiNen,  Code = "DM001", Name = "Phụ kiện khí nén",    Icon = "pi pi-cog",          SortOrder = 1, Description = "Nối khí, van, xi lanh, ống khí, bộ chia khí" },
            new() { Id = CatCuRoa,   Code = "DM002", Name = "Dây curoa & Vòng bi",  Icon = "pi pi-sync",         SortOrder = 2, Description = "Dây cu roa, vòng bi các loại, ống thuỷ lực" },
            new() { Id = CatDungCu,  Code = "DM003", Name = "Dụng cụ cầm tay",      Icon = "pi pi-wrench",       SortOrder = 3, Description = "Tua vít, bộ khẩu, dao, dũa mài, súng khí" },
            new() { Id = CatDien,    Code = "DM004", Name = "Vật tư điện",           Icon = "pi pi-bolt",         SortOrder = 4, Description = "Dây điện, đèn, công tắc, aptomat, tủ điện" },
            new() { Id = CatCoKhi,   Code = "DM005", Name = "Vật tư cơ khí",         Icon = "pi pi-th-large",     SortOrder = 5, Description = "Bulong, vít, sắt, đinh, long đen, ecu" },
            new() { Id = CatHoaChat, Code = "DM006", Name = "Hóa chất & Keo",        Icon = "pi pi-flask",        SortOrder = 6, Description = "Keo silicone, WD40, sơn, sơn xịt, keo tản nhiệt" },
            new() { Id = CatBaoHo,   Code = "DM007", Name = "Bảo hộ & Đóng gói",     Icon = "pi pi-shield",       SortOrder = 7, Description = "Găng tay, bạt, xốp, băng dính, bao dứa" },
            new() { Id = CatThietBi, Code = "DM008", Name = "Thiết bị & Máy móc",    Icon = "pi pi-server",       SortOrder = 8, Description = "Mô tơ, quạt, điều hòa, đồng hồ đo, máy hàn" },
        };

        db.Categories.AddRange(categories);
        await db.SaveChangesAsync();
    }

    // =============================================
    // PHẦN 2: NHÀ CUNG CẤP
    // =============================================
    private static readonly Guid NccChienThao   = Guid.Parse("b0000001-0000-0000-0000-000000000001");
    private static readonly Guid NccHueMuoi     = Guid.Parse("b0000001-0000-0000-0000-000000000002");
    private static readonly Guid NccHuongThinh  = Guid.Parse("b0000001-0000-0000-0000-000000000003");
    private static readonly Guid NccThuyPhong   = Guid.Parse("b0000001-0000-0000-0000-000000000004");
    private static readonly Guid NccShopee      = Guid.Parse("b0000001-0000-0000-0000-000000000005");
    private static readonly Guid NccDucHung     = Guid.Parse("b0000001-0000-0000-0000-000000000006");
    private static readonly Guid NccTuanAnh     = Guid.Parse("b0000001-0000-0000-0000-000000000007");
    private static readonly Guid NccHangThu     = Guid.Parse("b0000001-0000-0000-0000-000000000008");
    private static readonly Guid NccCoHuong     = Guid.Parse("b0000001-0000-0000-0000-000000000009");
    private static readonly Guid NccHienDanh    = Guid.Parse("b0000001-0000-0000-0000-000000000010");
    private static readonly Guid NccAltekPvc    = Guid.Parse("b0000001-0000-0000-0000-000000000011");
    private static readonly Guid NccNamDinh     = Guid.Parse("b0000001-0000-0000-0000-000000000012");
    private static readonly Guid NccSangPham    = Guid.Parse("b0000001-0000-0000-0000-000000000013");
    private static readonly Guid NccTheBao      = Guid.Parse("b0000001-0000-0000-0000-000000000014");
    private static readonly Guid NccBanhXeHod   = Guid.Parse("b0000001-0000-0000-0000-000000000015");
    private static readonly Guid NccThaiQuang   = Guid.Parse("b0000001-0000-0000-0000-000000000016");
    private static readonly Guid NccAduha       = Guid.Parse("b0000001-0000-0000-0000-000000000017");
    private static readonly Guid NccHiepHung    = Guid.Parse("b0000001-0000-0000-0000-000000000018");
    private static readonly Guid NccBigshop     = Guid.Parse("b0000001-0000-0000-0000-000000000019");
    private static readonly Guid NccQuiNhi      = Guid.Parse("b0000001-0000-0000-0000-000000000020");
    private static readonly Guid NccVinachi     = Guid.Parse("b0000001-0000-0000-0000-000000000021");
    private static readonly Guid NccDucThiep    = Guid.Parse("b0000001-0000-0000-0000-000000000022");
    private static readonly Guid NccCoLan       = Guid.Parse("b0000001-0000-0000-0000-000000000023");
    private static readonly Guid NccDucPhat     = Guid.Parse("b0000001-0000-0000-0000-000000000024");

    private static async Task SeedProviders(AppDbContext db)
    {


        var providers = new List<Provider>
        {
            new() { Id = NccChienThao,  Code = "NCC001", Name = "Cửa hàng Chiến Thảo",                         Phone = "0936463953", Address = "43 Tôn Đản, Hải Phòng" },
            new() { Id = NccHueMuoi,    Code = "NCC002", Name = "Cửa hàng Huệ Mười",                            Address = "Quán Toan, Hải Phòng" },
            new() { Id = NccHuongThinh, Code = "NCC003", Name = "Cửa hàng Hường Thịnh",                         Phone = "0902951994", Address = "80 Tôn Đản, Hải Phòng" },
            new() { Id = NccThuyPhong,  Code = "NCC004", Name = "Cửa hàng Thuỷ Phòng",                          Address = "Quán Toan, Hải Phòng" },
            new() { Id = NccShopee,     Code = "NCC005", Name = "Shopee",                                        Note = "Mua hàng online qua Shopee" },
            new() { Id = NccDucHung,    Code = "NCC006", Name = "Cửa hàng Đức Hưng",                            Phone = "0904634853", Address = "Quỳnh Cư, Hùng Vương, Hồng Bàng, Hải Phòng" },
            new() { Id = NccTuanAnh,    Code = "NCC007", Name = "Cửa hàng Tuấn Anh",                            Phone = "0379779279", Address = "Quán Toán, Hồng Bàng, Hải Phòng" },
            new() { Id = NccHangThu,    Code = "NCC008", Name = "Cửa hàng Hằng Thu",                             Phone = "0915301080", Address = "86 Tôn Đản, Hải Phòng" },
            new() { Id = NccCoHuong,    Code = "NCC009", Name = "Cửa hàng Cô Hương",                             Address = "An Lão, Hải Phòng" },
            new() { Id = NccHienDanh,   Code = "NCC010", Name = "Công ty TNHH SX & TM Hiển Danh",               Phone = "0969285800" },
            new() { Id = NccAltekPvc,   Code = "NCC011", Name = "Công ty TNHH Altek PVC Film",                   Address = "Thanh Trì, Hà Nội" },
            new() { Id = NccNamDinh,    Code = "NCC012", Name = "NCC Nam Định (0987892928)",                      Phone = "0987892928", Address = "Yên Đồng, Ý Yên, Nam Định" },
            new() { Id = NccSangPham,   Code = "NCC013", Name = "Công ty TNHH Sáng Phạm Giang",                  Phone = "0345211115", Address = "16/65 đường Vòng Cầu Niệm, Hải Phòng" },
            new() { Id = NccTheBao,     Code = "NCC014", Name = "Công ty CP SX-TM-DV Thế Bảo",                   Phone = "0934141116", Address = "Hồ Chí Minh" },
            new() { Id = NccBanhXeHod,  Code = "NCC015", Name = "Công ty TNHH Bánh Xe Đẩy HOD",                  Address = "Lê Văn Lương, Hà Nội" },
            new() { Id = NccThaiQuang,  Code = "NCC016", Name = "Công ty Thương Mại Thái Quảng",                 },
            new() { Id = NccAduha,      Code = "NCC017", Name = "Công ty Aduha Tech Việt Nam",                    Address = "Tràng Duệ, Hải Phòng" },
            new() { Id = NccHiepHung,   Code = "NCC018", Name = "Công ty Thương Mại Hiệp Hưng (Hải Toàn)",       },
            new() { Id = NccBigshop,    Code = "NCC019", Name = "Công ty CP Bigshop Việt Nam",                    Phone = "0932268131", Address = "Hà Nội" },
            new() { Id = NccQuiNhi,     Code = "NCC020", Name = "Công ty TM & DV Quý Nhi",                       },
            new() { Id = NccVinachi,    Code = "NCC021", Name = "Công ty Vinachi",                                Phone = "0961008118" },
            new() { Id = NccDucThiep,   Code = "NCC022", Name = "Cửa hàng Đức Thiệp",                            Address = "Hải Phòng" },
            new() { Id = NccCoLan,      Code = "NCC023", Name = "Cô Lan Quán Toan",                               Address = "Quán Toan, Hải Phòng" },
            new() { Id = NccDucPhat,    Code = "NCC024", Name = "Cửa hàng Đức Phát",                              Address = "Quán Toan, Hải Phòng" },
        };

        db.Providers.AddRange(providers);
        await db.SaveChangesAsync();
    }

    // =============================================
    // PHẦN 3: KHÁCH HÀNG (CÔNG TY BÁN)
    // =============================================
    private static readonly Guid KhRegal      = Guid.Parse("c0000001-0000-0000-0000-000000000001");
    private static readonly Guid KhDongA      = Guid.Parse("c0000001-0000-0000-0000-000000000002");
    private static readonly Guid KhShunCheong = Guid.Parse("c0000001-0000-0000-0000-000000000003");
    private static readonly Guid KhKyungsung  = Guid.Parse("c0000001-0000-0000-0000-000000000004");
    private static readonly Guid KhHuada      = Guid.Parse("c0000001-0000-0000-0000-000000000005");
    private static readonly Guid KhRapidAid   = Guid.Parse("c0000001-0000-0000-0000-000000000006");
    private static readonly Guid KhAutel      = Guid.Parse("c0000001-0000-0000-0000-000000000007");
    private static readonly Guid KhBaoBi      = Guid.Parse("c0000001-0000-0000-0000-000000000008");
    private static readonly Guid KhBond       = Guid.Parse("c0000001-0000-0000-0000-000000000009");
    private static readonly Guid KhVinatic    = Guid.Parse("c0000001-0000-0000-0000-000000000010");
    private static readonly Guid KhVinachi    = Guid.Parse("c0000001-0000-0000-0000-000000000011");

    private static async Task SeedCustomers(AppDbContext db)
    {


        var customers = new List<Customer>
        {
            new() { Id = KhRegal,      Code = "KH001", FullName = "Công ty Vật liệu mới Regal",       Phone = "0000000001" },
            new() { Id = KhDongA,      Code = "KH002", FullName = "Công ty DONG-A",                    Phone = "0000000002" },
            new() { Id = KhShunCheong, Code = "KH003", FullName = "Công ty Shun Cheong",               Phone = "0000000003" },
            new() { Id = KhKyungsung,  Code = "KH004", FullName = "Công ty Kyungsung Việt Nam",         Phone = "0000000004" },
            new() { Id = KhHuada,      Code = "KH005", FullName = "Công ty Huada",                     Phone = "0000000005" },
            new() { Id = KhRapidAid,   Code = "KH006", FullName = "Công ty Rapid Aid",                 Phone = "0000000006" },
            new() { Id = KhAutel,      Code = "KH007", FullName = "Công ty TNHH AUTEL Việt Nam",       Phone = "0981795741" },
            new() { Id = KhBaoBi,      Code = "KH008", FullName = "Công ty Bao Bì",                    Phone = "0000000008" },
            new() { Id = KhBond,       Code = "KH009", FullName = "Công ty Công Nghiệp Bond",          Phone = "0000000009" },
            new() { Id = KhVinatic,    Code = "KH010", FullName = "Công ty Vinatic",                   Phone = "0000000010" },
            new() { Id = KhVinachi,    Code = "KH011", FullName = "Công ty Vinachi",                   Phone = "0961008118" },
        };

        db.Customers.AddRange(customers);
        await db.SaveChangesAsync();
    }

    // =============================================
    // PHẦN 4: SẢN PHẨM + KHO HÀNG
    // =============================================
    private static async Task SeedProducts(AppDbContext db)
    {


        // Danh sách sản phẩm unique trích từ PDF
        var products = new List<(Product p, Guid? providerId)>
        {
            // ===== Phụ kiện khí nén (DM001) =====
            (new Product { Code = "SP001", Name = "Nối khí ren 13 ống 8",               Unit = "cái",   CategoryId = CatKhiNen,  CostPrice = 3500,     SellingPrice = 15000   }, NccChienThao),
            (new Product { Code = "SP002", Name = "Bộ chia khí 3 chạc",                 Unit = "cái",   CategoryId = CatKhiNen,  CostPrice = 32000,    SellingPrice = 64000   }, NccChienThao),
            (new Product { Code = "SP003", Name = "Bộ chia khí 2 chạc",                 Unit = "cái",   CategoryId = CatKhiNen,  CostPrice = 27000,    SellingPrice = 54000   }, NccChienThao),
            (new Product { Code = "SP004", Name = "PM20",                                Unit = "cái",   CategoryId = CatKhiNen,  CostPrice = 4500,     SellingPrice = 8000    }, NccChienThao),
            (new Product { Code = "SP005", Name = "Đầu nối thẳng ống khí phi 8",         Unit = "cái",   CategoryId = CatKhiNen,  CostPrice = 0,        SellingPrice = 5000    }, null),
            (new Product { Code = "SP006", Name = "Đầu nối thẳng ống khí phi 10",        Unit = "cái",   CategoryId = CatKhiNen,  CostPrice = 0,        SellingPrice = 5000    }, null),
            (new Product { Code = "SP007", Name = "Đầu nối thẳng ống khí phi 12",        Unit = "cái",   CategoryId = CatKhiNen,  CostPrice = 0,        SellingPrice = 6000    }, null),
            (new Product { Code = "SP008", Name = "Đầu nối khí phi 10 ren 13",           Unit = "cái",   CategoryId = CatKhiNen,  CostPrice = 4500,     SellingPrice = 10000   }, NccChienThao),
            (new Product { Code = "SP009", Name = "Đầu nối khí ren 13 ống 10",           Unit = "cái",   CategoryId = CatKhiNen,  CostPrice = 0,        SellingPrice = 13000   }, null),
            (new Product { Code = "SP010", Name = "Xi lanh SC 100-20",                   Unit = "cái",   CategoryId = CatKhiNen,  CostPrice = 610000,   SellingPrice = 854000  }, NccChienThao),
            (new Product { Code = "SP011", Name = "Cb-100",                              Unit = "cái",   CategoryId = CatKhiNen,  CostPrice = 90000,    SellingPrice = 135000  }, NccChienThao),
            (new Product { Code = "SP012", Name = "Mắt trâu",                           Unit = "cái",   CategoryId = CatKhiNen,  CostPrice = 85000,    SellingPrice = 130000  }, NccChienThao),
            (new Product { Code = "SP013", Name = "PC 12-04",                            Unit = "cái",   CategoryId = CatKhiNen,  CostPrice = 9000,     SellingPrice = 14000   }, NccChienThao),
            (new Product { Code = "SP014", Name = "Ống khí 12x8",                        Unit = "m",     CategoryId = CatKhiNen,  CostPrice = 0,        SellingPrice = 15000   }, NccChienThao),
            (new Product { Code = "SP015", Name = "Van gạt 4HV330-10",                  Unit = "cái",   CategoryId = CatKhiNen,  CostPrice = 160000,   SellingPrice = 240000  }, NccChienThao),
            (new Product { Code = "SP016", Name = "KAFC-2000",                           Unit = "cái",   CategoryId = CatKhiNen,  CostPrice = 100000,   SellingPrice = 150000  }, NccChienThao),
            (new Product { Code = "SP017", Name = "PC 12-03",                            Unit = "cái",   CategoryId = CatKhiNen,  CostPrice = 7000,     SellingPrice = 12000   }, NccChienThao),
            (new Product { Code = "SP018", Name = "CT-03",                               Unit = "cái",   CategoryId = CatKhiNen,  CostPrice = 8000,     SellingPrice = 12000   }, NccChienThao),
            (new Product { Code = "SP019", Name = "Ống khí 12",                          Unit = "cái",   CategoryId = CatKhiNen,  CostPrice = 0,        SellingPrice = 5000    }, NccChienThao),
            (new Product { Code = "SP020", Name = "Ống khí 8x5",                         Unit = "m",     CategoryId = CatKhiNen,  CostPrice = 0,        SellingPrice = 11000   }, null),
            (new Product { Code = "SP021", Name = "Van tiết lưu phi 8",                  Unit = "cái",   CategoryId = CatKhiNen,  CostPrice = 0,        SellingPrice = 45000   }, NccHuongThinh),
            (new Product { Code = "SP022", Name = "Nối khí nhanh phi 8",                 Unit = "cái",   CategoryId = CatKhiNen,  CostPrice = 8000,     SellingPrice = 25000   }, NccAduha),
            (new Product { Code = "SP023", Name = "Nối khí phi 8 (đầu đực)",             Unit = "cái",   CategoryId = CatKhiNen,  CostPrice = 0,        SellingPrice = 10000   }, NccDucHung),
            (new Product { Code = "SP024", Name = "Bộ van khí nén DN65",                 Unit = "bộ",    CategoryId = CatKhiNen,  CostPrice = 3500000,  SellingPrice = 5400000 }, null),

            // ===== Dây curoa & Vòng bi (DM002) =====
            (new Product { Code = "SP025", Name = "Dây cu roa B94",                      Unit = "cái",   CategoryId = CatCuRoa,   CostPrice = 37600,    SellingPrice = 80000   }, NccHangThu),
            (new Product { Code = "SP026", Name = "Dây cu roa A65",                      Unit = "cái",   CategoryId = CatCuRoa,   CostPrice = 26000,    SellingPrice = 40000   }, NccHangThu),
            (new Product { Code = "SP027", Name = "Dây cu roa B92",                      Unit = "cái",   CategoryId = CatCuRoa,   CostPrice = 36800,    SellingPrice = 75000   }, NccHangThu),
            (new Product { Code = "SP028", Name = "Vòng bi UCT205",                      Unit = "cái",   CategoryId = CatCuRoa,   CostPrice = 65000,    SellingPrice = 130000  }, null),
            (new Product { Code = "SP029", Name = "Vòng bi 6006",                        Unit = "cái",   CategoryId = CatCuRoa,   CostPrice = 25000,    SellingPrice = 60000   }, NccHuongThinh),
            (new Product { Code = "SP030", Name = "Vòng bi 6003",                        Unit = "cái",   CategoryId = CatCuRoa,   CostPrice = 15000,    SellingPrice = 40000   }, NccHuongThinh),
            (new Product { Code = "SP031", Name = "Vòng bi 6901 2RS",                    Unit = "cái",   CategoryId = CatCuRoa,   CostPrice = 15000,    SellingPrice = 30000   }, NccTheBao),
            (new Product { Code = "SP032", Name = "Vòng bi XLZY LM16UU",                 Unit = "cái",   CategoryId = CatCuRoa,   CostPrice = 30000,    SellingPrice = 100000  }, NccHuongThinh),
            (new Product { Code = "SP033", Name = "Vòng bi RN307",                       Unit = "cái",   CategoryId = CatCuRoa,   CostPrice = 130000,   SellingPrice = 260000  }, NccHuongThinh),
            (new Product { Code = "SP034", Name = "Vòng bi 6205",                        Unit = "cái",   CategoryId = CatCuRoa,   CostPrice = 25000,    SellingPrice = 50000   }, NccHuongThinh),
            (new Product { Code = "SP035", Name = "Vòng bi 32311J2 (d55xd120xb43)",      Unit = "cái",   CategoryId = CatCuRoa,   CostPrice = 280000,   SellingPrice = 560000  }, NccHangThu),
            (new Product { Code = "SP036", Name = "Vòng bi 30616J2 (d80xd130xb31)",      Unit = "cái",   CategoryId = CatCuRoa,   CostPrice = 350000,   SellingPrice = 700000  }, NccHangThu),
            (new Product { Code = "SP037", Name = "Vòng bi 32313J2 (d65xd140xb48)",      Unit = "cái",   CategoryId = CatCuRoa,   CostPrice = 400000,   SellingPrice = 800000  }, NccHangThu),
            (new Product { Code = "SP038", Name = "Vòng bi 32310J (D50xd110xb40)",       Unit = "cái",   CategoryId = CatCuRoa,   CostPrice = 230000,   SellingPrice = 450000  }, NccHangThu),
            (new Product { Code = "SP039", Name = "Ống giảm chấn",                      Unit = "cái",   CategoryId = CatCuRoa,   CostPrice = 730000,   SellingPrice = 1100000 }, null),
            (new Product { Code = "SP040", Name = "Ống thuỷ lực 3/8 2AT L1.35",          Unit = "cái",   CategoryId = CatCuRoa,   CostPrice = 150000,   SellingPrice = 250000  }, NccDucThiep),
            (new Product { Code = "SP041", Name = "Ống thuỷ lực 1 1/4 R12 L2.82m",       Unit = "cái",   CategoryId = CatCuRoa,   CostPrice = 1720000,  SellingPrice = 2580000 }, NccHienDanh),
            (new Product { Code = "SP042", Name = "Roong 20*28*5.5",                     Unit = "cái",   CategoryId = CatCuRoa,   CostPrice = 0,        SellingPrice = 40000   }, null),

            // ===== Dụng cụ cầm tay (DM003) =====
            (new Product { Code = "SP043", Name = "Phanh tay nhanh",                    Unit = "cái",   CategoryId = CatDungCu,  CostPrice = 170000,   SellingPrice = 255000  }, NccChienThao),
            (new Product { Code = "SP044", Name = "Bộ khẩu dài 1/2",                    Unit = "bộ",    CategoryId = CatDungCu,  CostPrice = 120000,   SellingPrice = 180000  }, NccHuongThinh),
            (new Product { Code = "SP045", Name = "Bộ tua vít",                          Unit = "hộp",   CategoryId = CatDungCu,  CostPrice = 430000,   SellingPrice = 602000  }, NccShopee),
            (new Product { Code = "SP046", Name = "Tua vít HISD81PH060",                Unit = "cái",   CategoryId = CatDungCu,  CostPrice = 36200,    SellingPrice = 70000   }, NccShopee),
            (new Product { Code = "SP047", Name = "Tua vít PH1*100mm Ingco",            Unit = "cái",   CategoryId = CatDungCu,  CostPrice = 25000,    SellingPrice = 40000   }, NccShopee),
            (new Product { Code = "SP048", Name = "Tua vít lục giác 3mm",               Unit = "cái",   CategoryId = CatDungCu,  CostPrice = 58800,    SellingPrice = 100000  }, NccShopee),
            (new Product { Code = "SP049", Name = "Bộ dũa mài",                         Unit = "bộ",    CategoryId = CatDungCu,  CostPrice = 60000,    SellingPrice = 130000  }, NccShopee),
            (new Product { Code = "SP050", Name = "Bộ mài dũa",                         Unit = "bộ",    CategoryId = CatDungCu,  CostPrice = 79000,    SellingPrice = 158000  }, NccShopee),
            (new Product { Code = "SP051", Name = "Súng khí",                            Unit = "cái",   CategoryId = CatDungCu,  CostPrice = 27000,    SellingPrice = 54000   }, NccChienThao),
            (new Product { Code = "SP052", Name = "Dao dọc giấy",                       Unit = "cái",   CategoryId = CatDungCu,  CostPrice = 20000,    SellingPrice = 30000   }, NccShopee),
            (new Product { Code = "SP053", Name = "Bát đánh rỉ 4 inch",                 Unit = "cái",   CategoryId = CatDungCu,  CostPrice = 8000,     SellingPrice = 16000   }, NccHuongThinh),
            (new Product { Code = "SP054", Name = "Lục giác chìm M10x9",                Unit = "cái",   CategoryId = CatDungCu,  CostPrice = 6000,     SellingPrice = 12000   }, NccThuyPhong),
            (new Product { Code = "SP055", Name = "Thước cuộn inox 7.5m",               Unit = "cái",   CategoryId = CatDungCu,  CostPrice = 310000,   SellingPrice = 470000  }, NccHuongThinh),
            (new Product { Code = "SP056", Name = "Bộ đầu khẩu bulong dài 1/2 13mm",    Unit = "cái",   CategoryId = CatDungCu,  CostPrice = 18000,    SellingPrice = 44000   }, NccNamDinh),

            // ===== Vật tư điện (DM004) =====
            (new Product { Code = "SP057", Name = "Đèn led vuông 600*600*220v*48w",     Unit = "cái",   CategoryId = CatDien,    CostPrice = 420000,   SellingPrice = 567000  }, NccSangPham),
            (new Product { Code = "SP058", Name = "Dây chịu nhiệt 1*4",                 Unit = "m",     CategoryId = CatDien,    CostPrice = 33000,    SellingPrice = 45000   }, NccSangPham),
            (new Product { Code = "SP059", Name = "Công tắc hành trình ME-8104",        Unit = "cái",   CategoryId = CatDien,    CostPrice = 40000,    SellingPrice = 60000   }, NccSangPham),
            (new Product { Code = "SP060", Name = "Đồng hồ thời gian 220v",             Unit = "cái",   CategoryId = CatDien,    CostPrice = 135000,   SellingPrice = 203000  }, NccChienThao),
            (new Product { Code = "SP061", Name = "Phao điện (10m dây điện)",            Unit = "cái",   CategoryId = CatDien,    CostPrice = 440000,   SellingPrice = 854000  }, NccTheBao),
            (new Product { Code = "SP062", Name = "Bóng trụ 40W TR120N1 Rạng Đông",     Unit = "cái",   CategoryId = CatDien,    CostPrice = 128500,   SellingPrice = 174000  }, NccSangPham),
            (new Product { Code = "SP063", Name = "Đèn tuýp led",                       Unit = "cái",   CategoryId = CatDien,    CostPrice = 0,        SellingPrice = 57000   }, NccSangPham),
            (new Product { Code = "SP064", Name = "Đèn sửa chữa",                      Unit = "cái",   CategoryId = CatDien,    CostPrice = 251000,   SellingPrice = 384000  }, NccShopee),
            (new Product { Code = "SP065", Name = "Đèn soi",                            Unit = "cái",   CategoryId = CatDien,    CostPrice = 169000,   SellingPrice = 270000  }, NccShopee),
            (new Product { Code = "SP066", Name = "Đèn nam châm",                       Unit = "cái",   CategoryId = CatDien,    CostPrice = 153000,   SellingPrice = 210000  }, NccShopee),
            (new Product { Code = "SP067", Name = "Công tắc cảm biến quang đối xứng",   Unit = "cái",   CategoryId = CatDien,    CostPrice = 610000,   SellingPrice = 880000  }, null),
            (new Product { Code = "SP068", Name = "Biến áp nguồn",                      Unit = "cái",   CategoryId = CatDien,    CostPrice = 950000,   SellingPrice = 1280000 }, null),
            (new Product { Code = "SP069", Name = "Đồng hồ đo nhiệt 220V",             Unit = "cái",   CategoryId = CatDien,    CostPrice = 270000,   SellingPrice = 405000  }, NccSangPham),
            (new Product { Code = "SP070", Name = "Đèn led panel 48W",                  Unit = "cái",   CategoryId = CatDien,    CostPrice = 420000,   SellingPrice = 567000  }, NccTheBao),
            (new Product { Code = "SP071", Name = "Đèn led 60*1200 96W",                Unit = "cái",   CategoryId = CatDien,    CostPrice = 1280000,  SellingPrice = 1782000 }, NccSangPham),
            (new Product { Code = "SP072", Name = "Công tắc tơ MC-40a 380VAC",          Unit = "cái",   CategoryId = CatDien,    CostPrice = 812000,   SellingPrice = 1096000 }, NccSangPham),
            (new Product { Code = "SP073", Name = "Aptomat đơn 32A LS",                 Unit = "cái",   CategoryId = CatDien,    CostPrice = 62400,    SellingPrice = 85000   }, NccSangPham),
            (new Product { Code = "SP074", Name = "Dây điện 1*1.5",                     Unit = "cuộn",  CategoryId = CatDien,    CostPrice = 604800,   SellingPrice = 1000000 }, NccSangPham),
            (new Product { Code = "SP075", Name = "Dây điện 1*2.5",                     Unit = "cuộn",  CategoryId = CatDien,    CostPrice = 986900,   SellingPrice = 1400000 }, NccSangPham),
            (new Product { Code = "SP076", Name = "Dây điện 1*4",                       Unit = "cuộn",  CategoryId = CatDien,    CostPrice = 1527800,  SellingPrice = 2100000 }, NccSangPham),
            (new Product { Code = "SP077", Name = "Dây điện 1*6",                       Unit = "cuộn",  CategoryId = CatDien,    CostPrice = 2273100,  SellingPrice = 3050000 }, NccSangPham),
            (new Product { Code = "SP078", Name = "Dây cáp điện 4*16",                  Unit = "m",     CategoryId = CatDien,    CostPrice = 227777,   SellingPrice = 332000  }, null),
            (new Product { Code = "SP079", Name = "Tủ điện 30*40*15cm",                 Unit = "cái",   CategoryId = CatDien,    CostPrice = 213000,   SellingPrice = 280000  }, NccHiepHung),
            (new Product { Code = "SP080", Name = "Biến trở 250k",                      Unit = "cái",   CategoryId = CatDien,    CostPrice = 3500,     SellingPrice = 10000   }, null),
            (new Product { Code = "SP081", Name = "Ổ cắm 3m",                           Unit = "cái",   CategoryId = CatDien,    CostPrice = 69000,    SellingPrice = 105000  }, NccSangPham),
            (new Product { Code = "SP082", Name = "Đồng hồ vạn năng Fluke 15B Mã-01",  Unit = "cái",   CategoryId = CatDien,    CostPrice = 2940000,  SellingPrice = 3969000 }, NccNamDinh),
            (new Product { Code = "SP083", Name = "Cảm biến từ phi 18-BY",              Unit = "cái",   CategoryId = CatDien,    CostPrice = 60000,    SellingPrice = 120000  }, NccDucHung),
            (new Product { Code = "SP084", Name = "Đồng hồ đo nhiệt 0-100 độ",         Unit = "cái",   CategoryId = CatDien,    CostPrice = 210000,   SellingPrice = 340000  }, null),

            // ===== Vật tư cơ khí (DM005) =====
            (new Product { Code = "SP085", Name = "Tấm mã sắt 100x200x8",              Unit = "cái",   CategoryId = CatCoKhi,   CostPrice = 24000,    SellingPrice = 50000   }, NccDucHung),
            (new Product { Code = "SP086", Name = "Vít bắn tôn 3cm (200c/gói)",         Unit = "gói",   CategoryId = CatCoKhi,   CostPrice = 41000,    SellingPrice = 73000   }, NccTuanAnh),
            (new Product { Code = "SP087", Name = "Vít đầu dù 1cm",                     Unit = "gói",   CategoryId = CatCoKhi,   CostPrice = 59000,    SellingPrice = 120000  }, NccTuanAnh),
            (new Product { Code = "SP088", Name = "Vít đầu dù 2cm",                     Unit = "gói",   CategoryId = CatCoKhi,   CostPrice = 72000,    SellingPrice = 130000  }, NccTuanAnh),
            (new Product { Code = "SP089", Name = "Long đen phẳng lỗ 6x30mm",           Unit = "kg",    CategoryId = CatCoKhi,   CostPrice = 40000,    SellingPrice = 60000   }, null),
            (new Product { Code = "SP090", Name = "Bulong xiết M12",                     Unit = "cái",   CategoryId = CatCoKhi,   CostPrice = 4536,     SellingPrice = 9000    }, null),
            (new Product { Code = "SP091", Name = "Chếch PVC 110 Tiền Phong",           Unit = "cái",   CategoryId = CatCoKhi,   CostPrice = 33000,    SellingPrice = 60000   }, null),
            (new Product { Code = "SP092", Name = "Đinh 4",                              Unit = "kg",    CategoryId = CatCoKhi,   CostPrice = 30000,    SellingPrice = 27000   }, null),
            (new Product { Code = "SP093", Name = "Đinh 5",                              Unit = "kg",    CategoryId = CatCoKhi,   CostPrice = 25000,    SellingPrice = 27000   }, null),
            (new Product { Code = "SP094", Name = "Đinh vít 5",                          Unit = "gói",   CategoryId = CatCoKhi,   CostPrice = 70000,    SellingPrice = 100000  }, null),
            (new Product { Code = "SP095", Name = "Ecu M14",                             Unit = "cái",   CategoryId = CatCoKhi,   CostPrice = 1300,     SellingPrice = 2600    }, NccNamDinh),
            (new Product { Code = "SP096", Name = "Tay hùng",                            Unit = "cái",   CategoryId = CatCoKhi,   CostPrice = 3500,     SellingPrice = 6000    }, null),
            (new Product { Code = "SP097", Name = "Sắt buộc 2.5",                       Unit = "kg",    CategoryId = CatCoKhi,   CostPrice = 24000,    SellingPrice = 33000   }, null),
            (new Product { Code = "SP098", Name = "Sâu nở 6x30",                        Unit = "gói",   CategoryId = CatCoKhi,   CostPrice = 0,        SellingPrice = 12000   }, NccHangThu),
            (new Product { Code = "SP099", Name = "Chốt cửa",                           Unit = "cái",   CategoryId = CatCoKhi,   CostPrice = 30000,    SellingPrice = 60000   }, NccHangThu),
            (new Product { Code = "SP100", Name = "Móc sắt",                            Unit = "cái",   CategoryId = CatCoKhi,   CostPrice = 3000,     SellingPrice = 7000    }, NccHangThu),
            (new Product { Code = "SP101", Name = "Khuy tam giác to",                    Unit = "kg",    CategoryId = CatCoKhi,   CostPrice = 55000,    SellingPrice = 78000   }, null),
            (new Product { Code = "SP102", Name = "Sắt hộp mã kẽm 40*40*1.4",           Unit = "cây",   CategoryId = CatCoKhi,   CostPrice = 201952,   SellingPrice = 290000  }, NccDucPhat),
            (new Product { Code = "SP103", Name = "Cos 50",                              Unit = "cái",   CategoryId = CatCoKhi,   CostPrice = 10000,    SellingPrice = 20000   }, NccHiepHung),
            (new Product { Code = "SP104", Name = "Ghíp nhựa nối 2 bulong",             Unit = "cái",   CategoryId = CatCoKhi,   CostPrice = 17000,    SellingPrice = 30000   }, NccHiepHung),
            (new Product { Code = "SP105", Name = "Dây chun 5cm",                       Unit = "m",     CategoryId = CatCoKhi,   CostPrice = 6500,     SellingPrice = 13000   }, null),
            (new Product { Code = "SP106", Name = "Khoá PVC 27",                        Unit = "cái",   CategoryId = CatCoKhi,   CostPrice = 0,        SellingPrice = 31000   }, NccThuyPhong),
            (new Product { Code = "SP107", Name = "Khoá PVC 21",                        Unit = "cái",   CategoryId = CatCoKhi,   CostPrice = 0,        SellingPrice = 22000   }, NccThuyPhong),
            (new Product { Code = "SP108", Name = "Khoá HPDE phi 20",                   Unit = "cái",   CategoryId = CatCoKhi,   CostPrice = 0,        SellingPrice = 44000   }, NccThuyPhong),
            (new Product { Code = "SP109", Name = "Vòi xịt nhà vệ sinh",                Unit = "cái",   CategoryId = CatCoKhi,   CostPrice = 90000,    SellingPrice = 130000  }, NccHuongThinh),

            // ===== Hóa chất & Keo (DM006) =====
            (new Product { Code = "SP110", Name = "Keo silicone",                        Unit = "tuýp",  CategoryId = CatHoaChat, CostPrice = 18000,    SellingPrice = 27000   }, NccThuyPhong),
            (new Product { Code = "SP111", Name = "WD 40",                               Unit = "lọ",    CategoryId = CatHoaChat, CostPrice = 97000,    SellingPrice = 140000  }, null),
            (new Product { Code = "SP112", Name = "Keo chịu nhiệt silicone X'traseal xám (750F)", Unit = "lọ", CategoryId = CatHoaChat, CostPrice = 110000, SellingPrice = 176000 }, NccCoHuong),
            (new Product { Code = "SP113", Name = "Sơn NP Tilac 3L",                    Unit = "hộp",   CategoryId = CatHoaChat, CostPrice = 382000,   SellingPrice = 451000  }, null),
            (new Product { Code = "SP114", Name = "Sơn xịt",                            Unit = "lọ",    CategoryId = CatHoaChat, CostPrice = 35000,    SellingPrice = 50000   }, NccSangPham),
            (new Product { Code = "SP115", Name = "Keo tản nhiệt CPU Mx4-4g",           Unit = "tuýp",  CategoryId = CatHoaChat, CostPrice = 145000,   SellingPrice = 340000  }, NccShopee),
            (new Product { Code = "SP116", Name = "Que hàn 3.2",                         Unit = "hộp",   CategoryId = CatHoaChat, CostPrice = 145000,   SellingPrice = 196000  }, NccHueMuoi),
            (new Product { Code = "SP117", Name = "Que hàn 4.0",                         Unit = "hộp",   CategoryId = CatHoaChat, CostPrice = 145000,   SellingPrice = 196000  }, NccHueMuoi),
            (new Product { Code = "SP118", Name = "Dây hàn nhiệt 5mm",                  Unit = "cuộn",  CategoryId = CatHoaChat, CostPrice = 2628000,  SellingPrice = 3570000 }, null),

            // ===== Bảo hộ & Đóng gói (DM007) =====
            (new Product { Code = "SP119", Name = "Găng tay sợi (10k50g)",              Unit = "đôi",   CategoryId = CatBaoHo,   CostPrice = 2000,     SellingPrice = 3000    }, NccNamDinh),
            (new Product { Code = "SP120", Name = "Băng dính 47mm x80y",                 Unit = "cuộn",  CategoryId = CatBaoHo,   CostPrice = 10000,    SellingPrice = 14000   }, null),
            (new Product { Code = "SP121", Name = "Băng dính 47mm",                      Unit = "cuộn",  CategoryId = CatBaoHo,   CostPrice = 8800,     SellingPrice = 9500    }, null),
            (new Product { Code = "SP122", Name = "Băng dính 2 mặt 20*18y",             Unit = "cuộn",  CategoryId = CatBaoHo,   CostPrice = 60000,    SellingPrice = 6000    }, null),
            (new Product { Code = "SP123", Name = "Băng giấy 20*15y",                    Unit = "cuộn",  CategoryId = CatBaoHo,   CostPrice = 80000,    SellingPrice = 4000    }, null),
            (new Product { Code = "SP124", Name = "Băng dính điện",                      Unit = "cuộn",  CategoryId = CatBaoHo,   CostPrice = 6000,     SellingPrice = 9000    }, null),
            (new Product { Code = "SP125", Name = "Băng dính teflon 19mmx0.13mmx10mm",  Unit = "cái",   CategoryId = CatBaoHo,   CostPrice = 43000,    SellingPrice = 65000   }, NccTuanAnh),
            (new Product { Code = "SP126", Name = "Bạt xanh cam KT: 6x30",              Unit = "tấm",   CategoryId = CatBaoHo,   CostPrice = 2700000,  SellingPrice = 3645000 }, NccSangPham),
            (new Product { Code = "SP127", Name = "Bạt nhựa trắng trong KT: 3x30",      Unit = "tấm",   CategoryId = CatBaoHo,   CostPrice = 630000,   SellingPrice = 3645000 }, null),
            (new Product { Code = "SP128", Name = "Bạt nhựa trắng trong KT: 4x25",      Unit = "tấm",   CategoryId = CatBaoHo,   CostPrice = 800000,   SellingPrice = 0       }, NccSangPham),
            (new Product { Code = "SP129", Name = "Bạt nhựa PVC KT: 1700x1620",         Unit = "kg",    CategoryId = CatBaoHo,   CostPrice = 400000,   SellingPrice = 490000  }, NccCoLan),
            (new Product { Code = "SP130", Name = "Bạt nhựa PVC KT: 1700x1800",         Unit = "kg",    CategoryId = CatBaoHo,   CostPrice = 420000,   SellingPrice = 510000  }, NccCoLan),
            (new Product { Code = "SP131", Name = "Xốp KT: 260*340*28mm",               Unit = "cái",   CategoryId = CatBaoHo,   CostPrice = 118000,   SellingPrice = 145066  }, NccChienThao),
            (new Product { Code = "SP132", Name = "Bao dứa",                            Unit = "cái",   CategoryId = CatBaoHo,   CostPrice = 0,        SellingPrice = 5000    }, null),
            (new Product { Code = "SP133", Name = "Lạt nhựa",                           Unit = "gói",   CategoryId = CatBaoHo,   CostPrice = 35000,    SellingPrice = 50000   }, null),
            (new Product { Code = "SP134", Name = "Silicon xốp dạng tấm 1000mmx5m dày 8mm", Unit = "tấm", CategoryId = CatBaoHo, CostPrice = 4400000, SellingPrice = 6700000 }, NccHienDanh),
            (new Product { Code = "SP135", Name = "Màng PVC 1.4x30m dày 0.5mm",         Unit = "cuộn",  CategoryId = CatBaoHo,   CostPrice = 1116000,  SellingPrice = 1620000 }, NccAltekPvc),
            (new Product { Code = "SP136", Name = "Thảm silicone chống tĩnh điện 1.2x10m", Unit = "cuộn", CategoryId = CatBaoHo, CostPrice = 0,       SellingPrice = 1780000 }, null),
            (new Product { Code = "SP137", Name = "Kệ CPU",                             Unit = "cái",   CategoryId = CatBaoHo,   CostPrice = 111000,   SellingPrice = 148000  }, NccSangPham),
            (new Product { Code = "SP138", Name = "Dây điện 4*50",                      Unit = "m",     CategoryId = CatDien,    CostPrice = 65000,    SellingPrice = 94000   }, NccHiepHung),

            // ===== Thiết bị & Máy móc (DM008) =====
            (new Product { Code = "SP139", Name = "Quấn mô tơ 0.75kw",                  Unit = "cái",   CategoryId = CatThietBi, CostPrice = 500000,   SellingPrice = 800000  }, NccQuiNhi),
            (new Product { Code = "SP140", Name = "Quấn mô tơ 0.75kw (thay vòng bi)",   Unit = "cái",   CategoryId = CatThietBi, CostPrice = 600000,   SellingPrice = 910000  }, NccQuiNhi),
            (new Product { Code = "SP141", Name = "Quấn lại mô tơ 0,75kw",              Unit = "cái",   CategoryId = CatThietBi, CostPrice = 500000,   SellingPrice = 800000  }, NccQuiNhi),
            (new Product { Code = "SP142", Name = "Sửa máy hàn siêu âm cao tần",        Unit = "cái",   CategoryId = CatThietBi, CostPrice = 2800000,  SellingPrice = 3500000 }, null),
            (new Product { Code = "SP143", Name = "Gia công trục inox phi 30*200mm 2 đầu ren", Unit = "cái", CategoryId = CatThietBi, CostPrice = 250000, SellingPrice = 325000 }, NccAduha),
            (new Product { Code = "SP144", Name = "Sửa khuôn nhôm",                     Unit = "cái",   CategoryId = CatThietBi, CostPrice = 140000,   SellingPrice = 150000  }, null),
            (new Product { Code = "SP145", Name = "Sửa mô tơ 3 pha 7,5kw",              Unit = "cuộn",  CategoryId = CatThietBi, CostPrice = 2100000,  SellingPrice = 2500000 }, null),
            (new Product { Code = "SP146", Name = "Đá cắt sắt hải dương D100mm",        Unit = "viên",  CategoryId = CatThietBi, CostPrice = 6000,     SellingPrice = 9000    }, NccHuongThinh),
            (new Product { Code = "SP147", Name = "Bánh xe đẩy cọc vít TPU xám F100 xoay",          Unit = "cái", CategoryId = CatThietBi, CostPrice = 39000, SellingPrice = 55600 }, NccBanhXeHod),
            (new Product { Code = "SP148", Name = "Bánh xe đẩy cọc vít TPU xám F100 xoay có khoá",  Unit = "cái", CategoryId = CatThietBi, CostPrice = 34000, SellingPrice = 61000 }, NccBanhXeHod),
            (new Product { Code = "SP149", Name = "Bánh xe L130",                        Unit = "cái",   CategoryId = CatThietBi, CostPrice = 241750,   SellingPrice = 322000  }, null),
            (new Product { Code = "SP150", Name = "Điều hoà di động",                   Unit = "cái",   CategoryId = CatThietBi, CostPrice = 3840000,  SellingPrice = 6250000 }, NccBigshop),
            (new Product { Code = "SP151", Name = "Điều hoà Casper 9000BTU",             Unit = "cái",   CategoryId = CatThietBi, CostPrice = 0,        SellingPrice = 10855000}, NccBigshop),
            (new Product { Code = "SP152", Name = "Quạt thổi BK 5600",                  Unit = "cái",   CategoryId = CatThietBi, CostPrice = 1350000,  SellingPrice = 1823000 }, NccThaiQuang),
            (new Product { Code = "SP153", Name = "Trục khuấy D76xL1300mm",             Unit = "cái",   CategoryId = CatThietBi, CostPrice = 3200000,  SellingPrice = 4480000 }, NccAduha),
            (new Product { Code = "SP154", Name = "Trục khuấy D48xL900mm",              Unit = "cái",   CategoryId = CatThietBi, CostPrice = 1600000,  SellingPrice = 2080000 }, NccAduha),
            (new Product { Code = "SP155", Name = "Cánh khuấy D420xH50mm",              Unit = "cái",   CategoryId = CatThietBi, CostPrice = 650000,   SellingPrice = 845000  }, NccAduha),
            (new Product { Code = "SP156", Name = "Trục khuấy inox D50xH1500",          Unit = "cái",   CategoryId = CatThietBi, CostPrice = 4500000,  SellingPrice = 6075000 }, NccAduha),
            (new Product { Code = "SP157", Name = "Quấn lại động cơ 380V*7,5kw",        Unit = "cái",   CategoryId = CatThietBi, CostPrice = 2300000,  SellingPrice = 121000  }, null),
            (new Product { Code = "SP158", Name = "Sửa lại bép",                        Unit = "cái",   CategoryId = CatThietBi, CostPrice = 60000,    SellingPrice = 100000  }, NccAduha),
            (new Product { Code = "SP159", Name = "Mài lại lưỡi dao",                   Unit = "cái",   CategoryId = CatThietBi, CostPrice = 200000,   SellingPrice = 340000  }, null),
        };

        var productEntities = new List<Product>();
        var warehouseEntities = new List<WareHouse>();

        foreach (var (p, providerId) in products)
        {
            p.Id = Guid.NewGuid();
            p.IsActive = true;
            productEntities.Add(p);

            // Tạo WareHouse cho mỗi sản phẩm
            warehouseEntities.Add(new WareHouse
            {
                Id = Guid.NewGuid(),
                ProductId = p.Id,
                ProviderId = providerId,
                Quantity = 0,
                MinQuantity = 5,
                MaxQuantity = 100,
                Location = "Kho chính",
                LastStockUpdate = DateTime.UtcNow
            });
        }

        db.Products.AddRange(productEntities);
        db.WareHouses.AddRange(warehouseEntities);
    }
}
