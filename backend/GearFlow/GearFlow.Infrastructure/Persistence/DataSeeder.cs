using GearFlow.Domain.Entities;
using GearFlow.Domain.Enums;
using GearFlow.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GearFlow.Infrastructure.Persistence;

public static class DataSeeder
{
    public static async Task WipeAsync(GearFlowDbContext context)
    {
        await context.Database.ExecuteSqlRawAsync(@"
            TRUNCATE TABLE ""Notifications"", ""Defects"", ""Reservations"", ""Equipments"", ""EquipmentTypes"", ""Locations"", ""Users"", ""EquipmentStatuses"", ""Roles""
            RESTART IDENTITY CASCADE;
        ");
    }

    public static async Task SeedAsync(GearFlowDbContext context, IPasswordHasher hasher)
    {
        var hash = hasher.HashPassword("12345678");
        var now = DateTime.UtcNow;

        // ── Roles ─────────────────────────────────────────────────────────────────
        context.Roles.AddRange(
            new Role { Id = 1, Name = "Admin" },
            new Role { Id = 2, Name = "Employee" }
        );
        await context.SaveChangesAsync();

        // ── Equipment Statuses ────────────────────────────────────────────────────
        context.EquipmentStatuses.AddRange(
            new EquipmentStatus { Id = 1, Name = "Available" },
            new EquipmentStatus { Id = 2, Name = "Reserved" },
            new EquipmentStatus { Id = 3, Name = "Borrowed" },
            new EquipmentStatus { Id = 4, Name = "Serviced" },
            new EquipmentStatus { Id = 5, Name = "Destroyed" }
        );
        await context.SaveChangesAsync();

        // ── Locations ────────────────────────────────────────────────────────────
        var locations = new List<Location>
        {
            new() { Name = "Warsaw HQ" },
            new() { Name = "Kraków Branch" },
            new() { Name = "Gdańsk Office" },
            new() { Name = "Wrocław Office" },
            new() { Name = "Storage Room" },
            new() { Name = "Remote" },
        };
        context.Locations.AddRange(locations);
        await context.SaveChangesAsync();

        // ── Equipment Types ───────────────────────────────────────────────────────
        var types = new List<EquipmentType>
        {
            new() { Name = "Laptop" },
            new() { Name = "Camera" },
            new() { Name = "Projector" },
            new() { Name = "Power Tool" },
            new() { Name = "Audio Equipment" },
            new() { Name = "Networking" },
            new() { Name = "Measurement" },
        };
        context.EquipmentTypes.AddRange(types);
        await context.SaveChangesAsync();

        // ── Users ─────────────────────────────────────────────────────────────────
        var users = new List<User>
        {
            // Admins
            new() { FirstName = "Jan",       LastName = "Kowalski",     Email = "jan.kowalski@gearflow.com",        RoleId = 1, PasswordHash = hash },
            new() { FirstName = "Anna",      LastName = "Wiśniewska",   Email = "anna.wisniewska@gearflow.com",     RoleId = 1, PasswordHash = hash },
            // Employees
            new() { FirstName = "Piotr",     LastName = "Nowak",        Email = "piotr.nowak@gearflow.com",         RoleId = 2, PasswordHash = hash },
            new() { FirstName = "Marta",     LastName = "Zielińska",    Email = "marta.zielinska@gearflow.com",     RoleId = 2, PasswordHash = hash },
            new() { FirstName = "Tomasz",    LastName = "Wójcik",       Email = "tomasz.wojcik@gearflow.com",       RoleId = 2, PasswordHash = hash },
            new() { FirstName = "Karolina",  LastName = "Kamińska",     Email = "karolina.kaminska@gearflow.com",   RoleId = 2, PasswordHash = hash },
            new() { FirstName = "Michał",    LastName = "Lewandowski",  Email = "michal.lewandowski@gearflow.com",  RoleId = 2, PasswordHash = hash },
            new() { FirstName = "Agnieszka", LastName = "Dąbrowska",    Email = "agnieszka.dabrowska@gearflow.com", RoleId = 2, PasswordHash = hash },
            new() { FirstName = "Bartosz",   LastName = "Mazur",        Email = "bartosz.mazur@gearflow.com",       RoleId = 2, PasswordHash = hash },
            new() { FirstName = "Natalia",   LastName = "Piotrowska",   Email = "natalia.piotrowska@gearflow.com",  RoleId = 2, PasswordHash = hash },
            new() { FirstName = "Krzysztof", LastName = "Grabowski",    Email = "krzysztof.grabowski@gearflow.com", RoleId = 2, PasswordHash = hash },
            new() { FirstName = "Ewa",       LastName = "Nowicka",      Email = "ewa.nowicka@gearflow.com",         RoleId = 2, PasswordHash = hash },
        };
        context.Users.AddRange(users);
        await context.SaveChangesAsync();

        // ── Equipment ─────────────────────────────────────────────────────────────
        // StatusId: 1=Available, 2=Reserved, 3=Borrowed, 4=Serviced, 5=Destroyed
        var equipment = new List<Equipment>
        {
            // Laptops (type[0])
            new() { Name = "Dell XPS 15",               SerialNumber = "LPT-001-2024", Specification = "Intel Core i7-13700H, 32GB RAM, 1TB SSD, NVIDIA RTX 4060, 15.6\" OLED",     MaxLoanDays = 14, StatusId = 2, TypeId = types[0].Id, LocationId = locations[0].Id },
            new() { Name = "Lenovo ThinkPad X1 Carbon",  SerialNumber = "LPT-002-2024", Specification = "Intel Core i7-1365U, 16GB RAM, 512GB SSD, 14\" IPS display, 1.12kg",        MaxLoanDays = 14, StatusId = 1, TypeId = types[0].Id, LocationId = locations[1].Id },
            new() { Name = "MacBook Pro 14\"",            SerialNumber = "LPT-003-2024", Specification = "Apple M3 Pro, 18GB RAM, 512GB SSD, Liquid Retina XDR, 18h battery",        MaxLoanDays = 7,  StatusId = 2, TypeId = types[0].Id, LocationId = locations[0].Id },
            new() { Name = "HP EliteBook 840 G10",       SerialNumber = "LPT-004-2024", Specification = "Intel Core i5-1345U, 16GB RAM, 256GB SSD, 14\" FHD, vPro security",         MaxLoanDays = 14, StatusId = 1, TypeId = types[0].Id, LocationId = locations[2].Id },
            new() { Name = "ASUS ProArt Studiobook 16",  SerialNumber = "LPT-005-2023", Specification = "AMD Ryzen 9 7945HX, 64GB RAM, 2TB SSD, RTX 4070, OLED touchscreen",        MaxLoanDays = 7,  StatusId = 2, TypeId = types[0].Id, LocationId = locations[3].Id },
            new() { Name = "Microsoft Surface Pro 9",    SerialNumber = "LPT-006-2023", Specification = "Intel Core i7-1265U, 16GB RAM, 256GB SSD, 13\" PixelSense 2-in-1",          MaxLoanDays = 10, StatusId = 1, TypeId = types[0].Id, LocationId = locations[0].Id },

            // Cameras (type[1])
            new() { Name = "Sony Alpha A7 IV",           SerialNumber = "CAM-001-2024", Specification = "33MP Full-Frame, 4K 60fps, 5-axis IBIS, dual CFexpress/SD slots",           MaxLoanDays = 7,  StatusId = 2, TypeId = types[1].Id, LocationId = locations[2].Id },
            new() { Name = "Canon EOS R5",               SerialNumber = "CAM-002-2024", Specification = "45MP Full-Frame, 8K RAW video, IBIS, dual card slots, weather-sealed",      MaxLoanDays = 7,  StatusId = 1, TypeId = types[1].Id, LocationId = locations[0].Id },
            new() { Name = "DJI Pocket 3",               SerialNumber = "CAM-003-2023", Specification = "4K/120fps, 3-axis gimbal, 1\" CMOS sensor, built-in mic array",             MaxLoanDays = 5,  StatusId = 1, TypeId = types[1].Id, LocationId = locations[4].Id },
            new() { Name = "Nikon Z6 III",               SerialNumber = "CAM-004-2024", Specification = "24.5MP partial-stacked CMOS, 6K RAW, 120fps FHD, IBIS, dual card",         MaxLoanDays = 7,  StatusId = 2, TypeId = types[1].Id, LocationId = locations[1].Id },
            new() { Name = "GoPro Hero 12 Black",        SerialNumber = "CAM-005-2023", Specification = "5.3K/60fps, HyperSmooth 6.0, waterproof 10m, HDR, Enduro battery",          MaxLoanDays = 5,  StatusId = 1, TypeId = types[1].Id, LocationId = locations[4].Id },
            new() { Name = "DJI Air 3 Drone",            SerialNumber = "CAM-006-2024", Specification = "4K/60fps dual camera, 46min flight time, 20km range, obstacle avoidance",   MaxLoanDays = 3,  StatusId = 1, TypeId = types[1].Id, LocationId = locations[4].Id },

            // Projectors (type[2])
            new() { Name = "Epson EB-L200F",             SerialNumber = "PRJ-001-2023", Specification = "Full HD 1080p, 4500 lumens, laser, wireless LAN, portrait projection",      MaxLoanDays = 3,  StatusId = 1, TypeId = types[2].Id, LocationId = locations[0].Id },
            new() { Name = "BenQ MH560",                 SerialNumber = "PRJ-002-2022", Specification = "Full HD 1080p, 3800 lumens, SmartEco technology, 10W speaker",              MaxLoanDays = 3,  StatusId = 4, TypeId = types[2].Id, LocationId = locations[4].Id },
            new() { Name = "Optoma UHD38x",              SerialNumber = "PRJ-003-2024", Specification = "4K UHD, 4000 lumens, 240Hz gaming mode, HDR10, eARC HDMI",                  MaxLoanDays = 3,  StatusId = 1, TypeId = types[2].Id, LocationId = locations[1].Id },
            new() { Name = "ViewSonic M2e",              SerialNumber = "PRJ-004-2023", Specification = "Full HD portable, 1000 lumens, Harman Kardon speakers, WiFi/BT, 3h battery",MaxLoanDays = 5,  StatusId = 2, TypeId = types[2].Id, LocationId = locations[3].Id },

            // Power Tools (type[3])
            new() { Name = "Bosch GSR 18V-55",           SerialNumber = "PTL-001-2023", Specification = "18V cordless drill/driver, 55Nm torque, 2x2.0Ah batteries, charger",       MaxLoanDays = 5,  StatusId = 1, TypeId = types[3].Id, LocationId = locations[4].Id },
            new() { Name = "Makita DGA452Z",             SerialNumber = "PTL-002-2023", Specification = "18V angle grinder, 115mm disc, brushless motor, electronic brake",          MaxLoanDays = 5,  StatusId = 2, TypeId = types[3].Id, LocationId = locations[4].Id },
            new() { Name = "DeWalt DCS331N",             SerialNumber = "PTL-003-2024", Specification = "18V jigsaw, 135mm cut depth, tool-free blade change, variable speed",       MaxLoanDays = 5,  StatusId = 1, TypeId = types[3].Id, LocationId = locations[4].Id },
            new() { Name = "Festool TSC 55",             SerialNumber = "PTL-004-2023", Specification = "18V cordless circular saw, 55mm depth, Bluetooth, anti-splinter plate",     MaxLoanDays = 5,  StatusId = 1, TypeId = types[3].Id, LocationId = locations[4].Id },
            new() { Name = "Hilti TE 30-A36",           SerialNumber = "PTL-005-2022", Specification = "36V SDS-Plus rotary hammer, 3.2J, AVR vibration reduction, dust removal",   MaxLoanDays = 5,  StatusId = 2, TypeId = types[3].Id, LocationId = locations[4].Id },

            // Audio (type[4])
            new() { Name = "Rode NT-USB Mini",           SerialNumber = "AUD-001-2023", Specification = "USB condenser, cardioid, 24-bit/48kHz, built-in headphone amp, zero-lat",  MaxLoanDays = 7,  StatusId = 1, TypeId = types[4].Id, LocationId = locations[1].Id },
            new() { Name = "JBL Eon615",                 SerialNumber = "AUD-002-2022", Specification = "1000W active PA speaker, 15\" woofer, DSP, Bluetooth, 3-band EQ",           MaxLoanDays = 3,  StatusId = 1, TypeId = types[4].Id, LocationId = locations[2].Id },
            new() { Name = "Shure SM58",                 SerialNumber = "AUD-003-2021", Specification = "Dynamic vocal mic, cardioid, 50–15000Hz, pneumatic shock mount, XLR",       MaxLoanDays = 7,  StatusId = 2, TypeId = types[4].Id, LocationId = locations[1].Id },
            new() { Name = "Yamaha MG10XU",              SerialNumber = "AUD-004-2022", Specification = "10-ch mixer, 4 mic preamps, built-in effects, USB audio interface",         MaxLoanDays = 5,  StatusId = 1, TypeId = types[4].Id, LocationId = locations[2].Id },
            new() { Name = "Sennheiser EW 135P G4",      SerialNumber = "AUD-005-2023", Specification = "Wireless handheld mic system, 100m range, 42MHz tuning bandwidth, AA",      MaxLoanDays = 5,  StatusId = 1, TypeId = types[4].Id, LocationId = locations[3].Id },

            // Networking (type[5])
            new() { Name = "Cisco Catalyst 9200L",       SerialNumber = "NET-001-2023", Specification = "24-port PoE+ switch, 4x1G uplinks, 370W PoE budget, Cisco DNA ready",      MaxLoanDays = 14, StatusId = 1, TypeId = types[5].Id, LocationId = locations[0].Id },
            new() { Name = "Ubiquiti UniFi Dream Machine",SerialNumber = "NET-002-2022", Specification = "All-in-one router/switch/AP, 10G SFP+, IDS/IPS, UniFi controller",        MaxLoanDays = 14, StatusId = 1, TypeId = types[5].Id, LocationId = locations[4].Id },

            // Measurement (type[6])
            new() { Name = "Fluke 87V Multimeter",       SerialNumber = "MSR-001-2021", Specification = "True-RMS, 1000V CAT III, frequency, capacitance, temperature, min/max",    MaxLoanDays = 7,  StatusId = 1, TypeId = types[6].Id, LocationId = locations[4].Id },
            new() { Name = "Testo 875-1i Thermal Camera",SerialNumber = "MSR-002-2023", Specification = "160×120px IR, ±2°C accuracy, JPEG + radiometric storage, App control",     MaxLoanDays = 5,  StatusId = 2, TypeId = types[6].Id, LocationId = locations[4].Id },
        };
        context.Equipments.AddRange(equipment);
        await context.SaveChangesAsync();

        // ── Reservations ──────────────────────────────────────────────────────────
        var reservations = new List<Reservation>
        {
            // ── Active (equipment currently in use) ──────────────────────────────
            new() { EquipmentId = equipment[0].Id,  UserId = users[2].Id,  StartDate = now.AddDays(-3),  EndDate = now.AddDays(4),   Status = ReservationStatus.Active },
            new() { EquipmentId = equipment[2].Id,  UserId = users[4].Id,  StartDate = now.AddDays(-1),  EndDate = now.AddDays(6),   Status = ReservationStatus.Active },
            new() { EquipmentId = equipment[4].Id,  UserId = users[6].Id,  StartDate = now.AddDays(-4),  EndDate = now.AddDays(3),   Status = ReservationStatus.Active },
            new() { EquipmentId = equipment[6].Id,  UserId = users[8].Id,  StartDate = now.AddDays(-2),  EndDate = now.AddDays(5),   Status = ReservationStatus.Active },
            new() { EquipmentId = equipment[9].Id,  UserId = users[3].Id,  StartDate = now.AddDays(-1),  EndDate = now.AddDays(6),   Status = ReservationStatus.Active },
            new() { EquipmentId = equipment[17].Id, UserId = users[5].Id,  StartDate = now.AddDays(-2),  EndDate = now.AddDays(3),   Status = ReservationStatus.Active },
            new() { EquipmentId = equipment[20].Id, UserId = users[10].Id, StartDate = now.AddDays(-3),  EndDate = now.AddDays(2),   Status = ReservationStatus.Active },
            new() { EquipmentId = equipment[23].Id, UserId = users[7].Id,  StartDate = now.AddDays(-1),  EndDate = now.AddDays(4),   Status = ReservationStatus.Active },
            new() { EquipmentId = equipment[29].Id, UserId = users[9].Id,  StartDate = now.AddDays(-2),  EndDate = now.AddDays(3),   Status = ReservationStatus.Active },
            new() { EquipmentId = equipment[15].Id, UserId = users[11].Id, StartDate = now.AddDays(-1),  EndDate = now.AddDays(4),   Status = ReservationStatus.Active },

            // ── Pending (user returned, waiting admin confirmation) ───────────────
            new() { EquipmentId = equipment[3].Id,  UserId = users[9].Id,  StartDate = now.AddDays(-10), EndDate = now.AddDays(-4),  Status = ReservationStatus.Pending, PendingAt = now.AddDays(-1) },
            new() { EquipmentId = equipment[8].Id,  UserId = users[2].Id,  StartDate = now.AddDays(-7),  EndDate = now.AddDays(-2),  Status = ReservationStatus.Pending, PendingAt = now.AddDays(-2) },
            new() { EquipmentId = equipment[13].Id, UserId = users[5].Id,  StartDate = now.AddDays(-6),  EndDate = now.AddDays(-1),  Status = ReservationStatus.Pending, PendingAt = now.AddHours(-8) },
            new() { EquipmentId = equipment[18].Id, UserId = users[7].Id,  StartDate = now.AddDays(-5),  EndDate = now.AddDays(-1),  Status = ReservationStatus.Pending, PendingAt = now.AddHours(-5) },
            new() { EquipmentId = equipment[24].Id, UserId = users[4].Id,  StartDate = now.AddDays(-8),  EndDate = now.AddDays(-3),  Status = ReservationStatus.Pending, PendingAt = now.AddDays(-1) },

            // ── Completed ────────────────────────────────────────────────────────
            new() { EquipmentId = equipment[1].Id,  UserId = users[3].Id,  StartDate = now.AddDays(-30), EndDate = now.AddDays(-23), Status = ReservationStatus.Completed, PendingAt = now.AddDays(-23), CompletedAt = now.AddDays(-22) },
            new() { EquipmentId = equipment[5].Id,  UserId = users[11].Id, StartDate = now.AddDays(-25), EndDate = now.AddDays(-18), Status = ReservationStatus.Completed, PendingAt = now.AddDays(-18), CompletedAt = now.AddDays(-17) },
            new() { EquipmentId = equipment[7].Id,  UserId = users[6].Id,  StartDate = now.AddDays(-20), EndDate = now.AddDays(-13), Status = ReservationStatus.Completed, PendingAt = now.AddDays(-13), CompletedAt = now.AddDays(-13) },
            new() { EquipmentId = equipment[10].Id, UserId = users[8].Id,  StartDate = now.AddDays(-40), EndDate = now.AddDays(-35), Status = ReservationStatus.Completed, PendingAt = now.AddDays(-35), CompletedAt = now.AddDays(-34) },
            new() { EquipmentId = equipment[12].Id, UserId = users[2].Id,  StartDate = now.AddDays(-15), EndDate = now.AddDays(-12), Status = ReservationStatus.Completed, PendingAt = now.AddDays(-12), CompletedAt = now.AddDays(-11) },
            new() { EquipmentId = equipment[14].Id, UserId = users[10].Id, StartDate = now.AddDays(-50), EndDate = now.AddDays(-47), Status = ReservationStatus.Completed, PendingAt = now.AddDays(-47), CompletedAt = now.AddDays(-46) },
            new() { EquipmentId = equipment[16].Id, UserId = users[4].Id,  StartDate = now.AddDays(-60), EndDate = now.AddDays(-55), Status = ReservationStatus.Completed, PendingAt = now.AddDays(-55), CompletedAt = now.AddDays(-55) },
            new() { EquipmentId = equipment[19].Id, UserId = users[11].Id, StartDate = now.AddDays(-35), EndDate = now.AddDays(-30), Status = ReservationStatus.Completed, PendingAt = now.AddDays(-30), CompletedAt = now.AddDays(-29) },
            new() { EquipmentId = equipment[21].Id, UserId = users[3].Id,  StartDate = now.AddDays(-45), EndDate = now.AddDays(-38), Status = ReservationStatus.Completed, PendingAt = now.AddDays(-38), CompletedAt = now.AddDays(-37) },
            new() { EquipmentId = equipment[22].Id, UserId = users[9].Id,  StartDate = now.AddDays(-12), EndDate = now.AddDays(-9),  Status = ReservationStatus.Completed, PendingAt = now.AddDays(-9),  CompletedAt = now.AddDays(-8) },

            // ── Cancelled ────────────────────────────────────────────────────────
            new() { EquipmentId = equipment[11].Id, UserId = users[5].Id,  StartDate = now.AddDays(-5),  EndDate = now.AddDays(-2),  Status = ReservationStatus.Cancelled },
            new() { EquipmentId = equipment[25].Id, UserId = users[2].Id,  StartDate = now.AddDays(-8),  EndDate = now.AddDays(-5),  Status = ReservationStatus.Cancelled },
            new() { EquipmentId = equipment[26].Id, UserId = users[8].Id,  StartDate = now.AddDays(-12), EndDate = now.AddDays(-9),  Status = ReservationStatus.Cancelled },
            new() { EquipmentId = equipment[27].Id, UserId = users[6].Id,  StartDate = now.AddDays(-20), EndDate = now.AddDays(-17), Status = ReservationStatus.Cancelled },
            new() { EquipmentId = equipment[28].Id, UserId = users[10].Id, StartDate = now.AddDays(-3),  EndDate = now.AddDays(-1),  Status = ReservationStatus.Cancelled },
        };
        context.Reservations.AddRange(reservations);
        await context.SaveChangesAsync();

        // ── Defects ───────────────────────────────────────────────────────────────
        var defects = new List<Defect>
        {
            new() { Comment = "Screen has a hairline crack in the bottom-left corner, visible under bright light",   Status = DefectStatus.InProgress, UserId = users[3].Id,  EquipmentId = equipment[1].Id,  ReservationId = reservations[15].Id },
            new() { Comment = "Battery holds charge for ~2h instead of advertised 8h — likely needs replacement",   Status = DefectStatus.New,        UserId = users[2].Id,  EquipmentId = equipment[0].Id,  ReservationId = reservations[0].Id  },
            new() { Comment = "Autofocus emits audible clicking during video recording, visible in audio track",     Status = DefectStatus.Resolved,   UserId = users[8].Id,  EquipmentId = equipment[6].Id,  ReservationId = reservations[3].Id  },
            new() { Comment = "HDMI port does not recognise cable on first plug-in, requires re-seating 2–3 times", Status = DefectStatus.InProgress, UserId = users[5].Id,  EquipmentId = equipment[13].Id, ReservationId = reservations[12].Id },
            new() { Comment = "Trigger requires excessive force to engage — spring mechanism feels damaged",          Status = DefectStatus.New,        UserId = users[5].Id,  EquipmentId = equipment[17].Id, ReservationId = reservations[5].Id  },
            new() { Comment = "USB-C port is loose; connection drops intermittently under slight cable movement",    Status = DefectStatus.Resolved,   UserId = users[9].Id,  EquipmentId = equipment[23].Id, ReservationId = reservations[8].Id  },
            new() { Comment = "Right speaker channel cuts out during playback at volumes above 60%",                 Status = DefectStatus.New,        UserId = users[11].Id, EquipmentId = equipment[22].Id, ReservationId = reservations[24].Id },
            new() { Comment = "Fan noise noticeably loud under moderate load, possibly blocked intake",              Status = DefectStatus.InProgress, UserId = users[4].Id,  EquipmentId = equipment[2].Id,  ReservationId = reservations[1].Id  },
            new() { Comment = "Lens cap hinge is broken — cap falls off during transport",                           Status = DefectStatus.Resolved,   UserId = users[7].Id,  EquipmentId = equipment[9].Id,  ReservationId = reservations[4].Id  },
            new() { Comment = "Battery indicator stuck at 99% — does not reflect actual charge level",              Status = DefectStatus.New,        UserId = users[6].Id,  EquipmentId = equipment[4].Id,  ReservationId = reservations[2].Id  },
            new() { Comment = "Power cable insulation is frayed near the connector — potential safety hazard",      Status = DefectStatus.InProgress, UserId = users[10].Id, EquipmentId = equipment[20].Id, ReservationId = reservations[6].Id  },
            new() { Comment = "Touchpad occasionally freezes for 2–3 seconds, requires tap to wake",                Status = DefectStatus.Resolved,   UserId = users[3].Id,  EquipmentId = equipment[5].Id,  ReservationId = reservations[16].Id },
        };
        context.Defects.AddRange(defects);
        await context.SaveChangesAsync();

        // ── Notifications ─────────────────────────────────────────────────────────
        var notifications = new List<Notification>
        {
            new() { Message = "Your reservation for Dell XPS 15 is now active. Enjoy!",                            CreatedOn = now.AddDays(-3),  IsRead = true,  UserId = users[2].Id,  ReservationId = reservations[0].Id  },
            new() { Message = "MacBook Pro 14\" reservation starts today — please pick it up from Warsaw HQ.",      CreatedOn = now.AddDays(-1),  IsRead = false, UserId = users[4].Id,  ReservationId = reservations[1].Id  },
            new() { Message = "Reminder: ASUS ProArt Studiobook reservation ends in 2 days.",                       CreatedOn = now.AddDays(-2),  IsRead = false, UserId = users[6].Id,  ReservationId = reservations[2].Id  },
            new() { Message = "Your HP EliteBook 840 G10 return is awaiting admin confirmation.",                   CreatedOn = now.AddDays(-1),  IsRead = false, UserId = users[9].Id,  ReservationId = reservations[10].Id },
            new() { Message = "DJI Pocket 3 return received — admin will confirm shortly.",                        CreatedOn = now.AddDays(-2),  IsRead = true,  UserId = users[2].Id,  ReservationId = reservations[11].Id },
            new() { Message = "Your reservation for BenQ MH560 return is pending admin approval.",                  CreatedOn = now.AddHours(-8), IsRead = false, UserId = users[5].Id,  ReservationId = reservations[12].Id },
            new() { Message = "Lenovo ThinkPad X1 Carbon reservation completed. Thank you!",                       CreatedOn = now.AddDays(-22), IsRead = true,  UserId = users[3].Id,  ReservationId = reservations[15].Id },
            new() { Message = "Microsoft Surface Pro 9 reservation completed successfully.",                        CreatedOn = now.AddDays(-17), IsRead = true,  UserId = users[11].Id, ReservationId = reservations[16].Id },
            new() { Message = "Canon EOS R5 reservation has been completed. Equipment returned.",                   CreatedOn = now.AddDays(-13), IsRead = true,  UserId = users[6].Id,  ReservationId = reservations[17].Id },
            new() { Message = "Your reservation for GoPro Hero 12 Black has been cancelled.",                       CreatedOn = now.AddDays(-5),  IsRead = true,  UserId = users[5].Id,  ReservationId = reservations[25].Id },
            new() { Message = "DJI Air 3 Drone reservation was cancelled before it started.",                       CreatedOn = now.AddDays(-8),  IsRead = false, UserId = users[2].Id,  ReservationId = reservations[26].Id },
            new() { Message = "Reminder: Nikon Z6 III reservation ends in 3 days — please plan your return.",      CreatedOn = now.AddDays(-1),  IsRead = false, UserId = users[3].Id,  ReservationId = reservations[4].Id  },
            new() { Message = "Shure SM58 reservation is active. Pick up at Kraków Branch.",                        CreatedOn = now.AddDays(-1),  IsRead = true,  UserId = users[7].Id,  ReservationId = reservations[7].Id  },
            new() { Message = "Festool TSC 55 reservation completed. Equipment has been returned.",                 CreatedOn = now.AddDays(-55), IsRead = true,  UserId = users[4].Id,  ReservationId = reservations[21].Id },
            new() { Message = "Testo 875-1i Thermal Camera return is pending admin confirmation.",                  CreatedOn = now.AddDays(-2),  IsRead = false, UserId = users[9].Id,  ReservationId = reservations[8].Id  },
            new() { Message = "Sennheiser EW 135P G4 return is awaiting confirmation. Expected today.",             CreatedOn = now.AddDays(-1),  IsRead = false, UserId = users[4].Id,  ReservationId = reservations[14].Id },
        };
        context.Notifications.AddRange(notifications);
        await context.SaveChangesAsync();
    }
}
