using HMS.Staff.Domain.Entities;
using HMS.Staff.Domain.Enums;
using HMS.Staff.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HMS.Staff.Infrastructure.Seeders
{
    public class StaffDataSeeder
    {
        private readonly StaffDbContext _context;
        private readonly ILogger<StaffDataSeeder> _logger;

        public StaffDataSeeder(StaffDbContext context, ILogger<StaffDataSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            try
            {
                _logger.LogInformation("Starting staff data seeding...");

                // Check if data already exists
                if (await _context.Staff.AnyAsync())
                {
                    _logger.LogInformation("Staff data already exists. Skipping seeding.");
                    return;
                }

                var staffMembers = new List<Domain.Entities.Staff>();
                var adminUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");

                // Seed Doctors
                staffMembers.AddRange(GetDoctors(adminUserId));

                // Seed Nurses
                staffMembers.AddRange(GetNurses(adminUserId));

                // Seed Pharmacists
                staffMembers.AddRange(GetPharmacists(adminUserId));

                // Seed Lab Technicians
                staffMembers.AddRange(GetLabTechnicians(adminUserId));

                // Seed Receptionists
                staffMembers.AddRange(GetReceptionists(adminUserId));

                // Seed Administrative Staff
                staffMembers.AddRange(GetAdministrativeStaff(adminUserId));

                await _context.Staff.AddRangeAsync(staffMembers);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Successfully seeded {staffMembers.Count} staff members");

                // Seed additional data
                await SeedStaffEducation(staffMembers);
                await SeedStaffExperience(staffMembers);
                await SeedStaffCertifications(staffMembers);
                await SeedStaffAttendance(staffMembers);
                await SeedStaffLeaves(staffMembers);
                await SeedStaffPerformanceReviews(staffMembers);

                _logger.LogInformation("Staff data seeding completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while seeding staff data");
                throw;
            }
        }

        private List<Domain.Entities.Staff> GetDoctors(Guid createdBy)
        {
            var doctors = new List<Domain.Entities.Staff>
            {
                new Domain.Entities.Staff
                {
                    Id = Guid.NewGuid(),
                    StaffNumber = "DOC001",
                    UserId = Guid.Parse("81aa0b14-6373-4ea7-2923-08de56c6f3ad"),
                    StaffType = StaffType.Doctor,
                    Department = "Cardiology",
                    Specialization = "Interventional Cardiology",
                    Position = "Senior Consultant",
                    JoinDate = new DateTime(2018, 3, 15),
                    EmploymentStatus = EmploymentStatus.Active,
                    EmploymentType = EmploymentType.FullTime,
                    LicenseNumber = "MED-2018-001234",
                    LicenseIssueDate = new DateTime(2018, 1, 10),
                    LicenseExpiryDate = new DateTime(2028, 1, 10),
                    Qualifications = "MD, DM (Cardiology), FACC",
                    YearsOfExperience = 15,
                    ShiftType = ShiftType.Morning,
                    WorkSchedule = "Monday-Friday, 9:00 AM - 5:00 PM",
                    WeeklyWorkHours = 40,
                    BasicSalary = 180000,
                    BankAccountNumber = "1234567890",
                    BankName = "National Bank",
                    TaxId = "TAX123456789",
                    AddressLine1 = "123 Medical Plaza",
                    City = "Cairo",
                    State = "Cairo Governorate",
                    Country = "Egypt",
                    PostalCode = "11511",
                    EmergencyContactName = "Sarah Ahmed",
                    EmergencyContactPhone = "+201234567890",
                    EmergencyContactRelationship = "Spouse",
                    BloodGroup = "A+",
                    Languages = "Arabic, English",
                    IsActive = true,
                    CreatedBy = createdBy,
                    CreatedAt = DateTime.UtcNow
                },
                new Domain.Entities.Staff
                {
                    Id = Guid.NewGuid(),
                    StaffNumber = "DOC002",
                    UserId = Guid.NewGuid(),
                    StaffType = StaffType.Doctor,
                    Department = "Pediatrics",
                    Specialization = "Neonatology",
                    Position = "Consultant",
                    JoinDate = new DateTime(2020, 6, 1),
                    EmploymentStatus = EmploymentStatus.Active,
                    EmploymentType = EmploymentType.FullTime,
                    LicenseNumber = "MED-2020-002345",
                    LicenseIssueDate = new DateTime(2019, 12, 15),
                    LicenseExpiryDate = new DateTime(2029, 12, 15),
                    Qualifications = "MBBS, MD (Pediatrics), Fellowship in Neonatology",
                    YearsOfExperience = 10,
                    ShiftType = ShiftType.Morning,
                    WorkSchedule = "Monday-Saturday, 8:00 AM - 4:00 PM",
                    WeeklyWorkHours = 48,
                    BasicSalary = 150000,
                    BankAccountNumber = "2345678901",
                    BankName = "Commercial Bank",
                    TaxId = "TAX234567890",
                    AddressLine1 = "456 Healthcare Street",
                    City = "Alexandria",
                    State = "Alexandria Governorate",
                    Country = "Egypt",
                    PostalCode = "21500",
                    EmergencyContactName = "Mohamed Hassan",
                    EmergencyContactPhone = "+201345678901",
                    EmergencyContactRelationship = "Brother",
                    BloodGroup = "O+",
                    Languages = "Arabic, English, French",
                    IsActive = true,
                    CreatedBy = createdBy,
                    CreatedAt = DateTime.UtcNow
                },
                new Domain.Entities.Staff
                {
                    Id = Guid.NewGuid(),
                    StaffNumber = "DOC003",
                    UserId = Guid.NewGuid(),
                    StaffType = StaffType.Doctor,
                    Department = "Orthopedics",
                    Specialization = "Sports Medicine",
                    Position = "Associate Consultant",
                    JoinDate = new DateTime(2021, 9, 15),
                    EmploymentStatus = EmploymentStatus.Active,
                    EmploymentType = EmploymentType.FullTime,
                    LicenseNumber = "MED-2021-003456",
                    LicenseIssueDate = new DateTime(2021, 8, 1),
                    LicenseExpiryDate = new DateTime(2031, 8, 1),
                    Qualifications = "MBBS, MS (Orthopedics), Fellowship in Sports Medicine",
                    YearsOfExperience = 8,
                    ShiftType = ShiftType.Morning,
                    WorkSchedule = "Monday-Friday, 10:00 AM - 6:00 PM",
                    WeeklyWorkHours = 40,
                    BasicSalary = 140000,
                    BankAccountNumber = "3456789012",
                    BankName = "Development Bank",
                    TaxId = "TAX345678901",
                    AddressLine1 = "789 Hospital Avenue",
                    City = "Giza",
                    State = "Giza Governorate",
                    Country = "Egypt",
                    PostalCode = "12511",
                    EmergencyContactName = "Fatima Ali",
                    EmergencyContactPhone = "+201456789012",
                    EmergencyContactRelationship = "Wife",
                    BloodGroup = "B+",
                    Languages = "Arabic, English",
                    IsActive = true,
                    CreatedBy = createdBy,
                    CreatedAt = DateTime.UtcNow
                }
            };

            return doctors;
        }

        private List<Domain.Entities.Staff> GetNurses(Guid createdBy)
        {
            var nurses = new List<Domain.Entities.Staff>
            {
                new Domain.Entities.Staff
                {
                    Id = Guid.NewGuid(),
                    StaffNumber = "NUR001",
                    UserId = Guid.NewGuid(),
                    StaffType = StaffType.Nurse,
                    Department = "Emergency",
                    Specialization = "Critical Care",
                    Position = "Head Nurse",
                    JoinDate = new DateTime(2017, 5, 20),
                    EmploymentStatus = EmploymentStatus.Active,
                    EmploymentType = EmploymentType.FullTime,
                    LicenseNumber = "NUR-2017-001234",
                    LicenseIssueDate = new DateTime(2017, 4, 1),
                    LicenseExpiryDate = new DateTime(2027, 4, 1),
                    Qualifications = "BSN, Critical Care Certification",
                    YearsOfExperience = 12,
                    ShiftType = ShiftType.Rotational,
                    WorkSchedule = "Rotating 12-hour shifts",
                    WeeklyWorkHours = 48,
                    BasicSalary = 55000,
                    BankAccountNumber = "4567890123",
                    BankName = "National Bank",
                    TaxId = "TAX456789012",
                    AddressLine1 = "321 Nursing Lane",
                    City = "Cairo",
                    State = "Cairo Governorate",
                    Country = "Egypt",
                    PostalCode = "11511",
                    EmergencyContactName = "Ahmed Mahmoud",
                    EmergencyContactPhone = "+201567890123",
                    EmergencyContactRelationship = "Husband",
                    BloodGroup = "AB+",
                    Languages = "Arabic, English",
                    IsActive = true,
                    CreatedBy = createdBy,
                    CreatedAt = DateTime.UtcNow
                },
                new Domain.Entities.Staff
                {
                    Id = Guid.NewGuid(),
                    StaffNumber = "NUR002",
                    UserId = Guid.NewGuid(),
                    StaffType = StaffType.Nurse,
                    Department = "Pediatrics",
                    Specialization = "Pediatric Nursing",
                    Position = "Staff Nurse",
                    JoinDate = new DateTime(2019, 8, 10),
                    EmploymentStatus = EmploymentStatus.Active,
                    EmploymentType = EmploymentType.FullTime,
                    LicenseNumber = "NUR-2019-002345",
                    LicenseIssueDate = new DateTime(2019, 7, 15),
                    LicenseExpiryDate = new DateTime(2029, 7, 15),
                    Qualifications = "BSN, Pediatric Care Certification",
                    YearsOfExperience = 6,
                    ShiftType = ShiftType.Morning   ,
                    WorkSchedule = "Monday-Friday, 7:00 AM - 3:00 PM",
                    WeeklyWorkHours = 40,
                    BasicSalary = 45000,
                    BankAccountNumber = "5678901234",
                    BankName = "Commercial Bank",
                    TaxId = "TAX567890123",
                    AddressLine1 = "654 Care Street",
                    City = "Alexandria",
                    State = "Alexandria Governorate",
                    Country = "Egypt",
                    PostalCode = "21500",
                    EmergencyContactName = "Mona Ibrahim",
                    EmergencyContactPhone = "+201678901234",
                    EmergencyContactRelationship = "Mother",
                    BloodGroup = "O-",
                    Languages = "Arabic, English",
                    IsActive = true,
                    CreatedBy = createdBy,
                    CreatedAt = DateTime.UtcNow
                },
                new Domain.Entities.Staff
                {
                    Id = Guid.NewGuid(),
                    StaffNumber = "NUR003",
                    UserId = Guid.NewGuid(),
                    StaffType = StaffType.Nurse,
                    Department = "Surgery",
                    Specialization = "Surgical Nursing",
                    Position = "Operating Room Nurse",
                    JoinDate = new DateTime(2020, 11, 5),
                    EmploymentStatus = EmploymentStatus.Active,
                    EmploymentType = EmploymentType.FullTime,
                    LicenseNumber = "NUR-2020-003456",
                    LicenseIssueDate = new DateTime(2020, 10, 1),
                    LicenseExpiryDate = new DateTime(2030, 10, 1),
                    Qualifications = "BSN, OR Nursing Certification",
                    YearsOfExperience = 5,
                    ShiftType = ShiftType.Morning,
                    WorkSchedule = "Monday-Saturday, 8:00 AM - 4:00 PM",
                    WeeklyWorkHours = 48,
                    BasicSalary = 48000,
                    BankAccountNumber = "6789012345",
                    BankName = "Development Bank",
                    TaxId = "TAX678901234",
                    AddressLine1 = "987 Medical Way",
                    City = "Giza",
                    State = "Giza Governorate",
                    Country = "Egypt",
                    PostalCode = "12511",
                    EmergencyContactName = "Hala Youssef",
                    EmergencyContactPhone = "+201789012345",
                    EmergencyContactRelationship = "Sister",
                    BloodGroup = "A-",
                    Languages = "Arabic, English",
                    IsActive = true,
                    CreatedBy = createdBy,
                    CreatedAt = DateTime.UtcNow
                }
            };

            return nurses;
        }

        private List<Domain.Entities.Staff> GetPharmacists(Guid createdBy)
        {
            var pharmacists = new List<Domain.Entities.Staff>
            {
                new Domain.Entities.Staff
                {
                    Id = Guid.NewGuid(),
                    StaffNumber = "PHA001",
                    UserId = Guid.NewGuid(),
                    StaffType = StaffType.Pharmacist,
                    Department = "Pharmacy",
                    Specialization = "Clinical Pharmacy",
                    Position = "Chief Pharmacist",
                    JoinDate = new DateTime(2016, 4, 1),
                    EmploymentStatus = EmploymentStatus.Active,
                    EmploymentType = EmploymentType.FullTime,
                    LicenseNumber = "PHA-2016-001234",
                    LicenseIssueDate = new DateTime(2016, 3, 1),
                    LicenseExpiryDate = new DateTime(2026, 3, 1),
                    Qualifications = "PharmD, Clinical Pharmacy Certification",
                    YearsOfExperience = 14,
                    ShiftType = ShiftType.Morning,
                    WorkSchedule = "Monday-Friday, 8:00 AM - 4:00 PM",
                    WeeklyWorkHours = 40,
                    BasicSalary = 75000,
                    BankAccountNumber = "7890123456",
                    BankName = "National Bank",
                    TaxId = "TAX789012345",
                    AddressLine1 = "147 Pharmacy Road",
                    City = "Cairo",
                    State = "Cairo Governorate",
                    Country = "Egypt",
                    PostalCode = "11511",
                    EmergencyContactName = "Khaled Samir",
                    EmergencyContactPhone = "+201890123456",
                    EmergencyContactRelationship = "Brother",
                    BloodGroup = "B-",
                    Languages = "Arabic, English",
                    IsActive = true,
                    CreatedBy = createdBy,
                    CreatedAt = DateTime.UtcNow
                },
                new Domain.Entities.Staff
                {
                    Id = Guid.NewGuid(),
                    StaffNumber = "PHA002",
                    UserId = Guid.NewGuid(),
                    StaffType = StaffType.Pharmacist,
                    Department = "Pharmacy",
                    Specialization = "Hospital Pharmacy",
                    Position = "Staff Pharmacist",
                    JoinDate = new DateTime(2021, 2, 15),
                    EmploymentStatus = EmploymentStatus.Active,
                    EmploymentType = EmploymentType.FullTime,
                    LicenseNumber = "PHA-2021-002345",
                    LicenseIssueDate = new DateTime(2021, 1, 10),
                    LicenseExpiryDate = new DateTime(2031, 1, 10),
                    Qualifications = "PharmD",
                    YearsOfExperience = 4,
                    ShiftType = ShiftType.Morning,
                    WorkSchedule = "Monday-Saturday, 9:00 AM - 5:00 PM",
                    WeeklyWorkHours = 48,
                    BasicSalary = 50000,
                    BankAccountNumber = "8901234567",
                    BankName = "Commercial Bank",
                    TaxId = "TAX890123456",
                    AddressLine1 = "258 Health Plaza",
                    City = "Alexandria",
                    State = "Alexandria Governorate",
                    Country = "Egypt",
                    PostalCode = "21500",
                    EmergencyContactName = "Layla Hassan",
                    EmergencyContactPhone = "+201901234567",
                    EmergencyContactRelationship = "Mother",
                    BloodGroup = "O+",
                    Languages = "Arabic, English",
                    IsActive = true,
                    CreatedBy = createdBy,
                    CreatedAt = DateTime.UtcNow
                }
            };

            return pharmacists;
        }

        private List<Domain.Entities.Staff> GetLabTechnicians(Guid createdBy)
        {
            var labTechs = new List<Domain.Entities.Staff>
            {
                new Domain.Entities.Staff
                {
                    Id = Guid.NewGuid(),
                    StaffNumber = "LAB001",
                    UserId = Guid.NewGuid(),
                    StaffType = StaffType.LabTechnician,
                    Department = "Laboratory",
                    Specialization = "Clinical Pathology",
                    Position = "Senior Lab Technician",
                    JoinDate = new DateTime(2018, 7, 10),
                    EmploymentStatus = EmploymentStatus.Active,
                    EmploymentType = EmploymentType.FullTime,
                    LicenseNumber = "LAB-2018-001234",
                    LicenseIssueDate = new DateTime(2018, 6, 15),
                    LicenseExpiryDate = new DateTime(2028, 6, 15),
                    Qualifications = "BSc Medical Laboratory Technology",
                    YearsOfExperience = 10,
                    ShiftType = ShiftType.Morning,
                    WorkSchedule = "Monday-Friday, 7:00 AM - 3:00 PM",
                    WeeklyWorkHours = 40,
                    BasicSalary = 42000,
                    BankAccountNumber = "9012345678",
                    BankName = "National Bank",
                    TaxId = "TAX901234567",
                    AddressLine1 = "369 Lab Avenue",
                    City = "Cairo",
                    State = "Cairo Governorate",
                    Country = "Egypt",
                    PostalCode = "11511",
                    EmergencyContactName = "Nadia Kamel",
                    EmergencyContactPhone = "+202012345678",
                    EmergencyContactRelationship = "Wife",
                    BloodGroup = "A+",
                    Languages = "Arabic, English",
                    IsActive = true,
                    CreatedBy = createdBy,
                    CreatedAt = DateTime.UtcNow
                },
                new Domain.Entities.Staff
                {
                    Id = Guid.NewGuid(),
                    StaffNumber = "LAB002",
                    UserId = Guid.NewGuid(),
                    StaffType = StaffType.LabTechnician,
                    Department = "Laboratory",
                    Specialization = "Microbiology",
                    Position = "Lab Technician",
                    JoinDate = new DateTime(2022, 1, 20),
                    EmploymentStatus = EmploymentStatus.Active,
                    EmploymentType = EmploymentType.FullTime,
                    LicenseNumber = "LAB-2022-002345",
                    LicenseIssueDate = new DateTime(2022, 1, 1),
                    LicenseExpiryDate = new DateTime(2032, 1, 1),
                    Qualifications = "BSc Medical Laboratory Science",
                    YearsOfExperience = 3,
                    ShiftType = ShiftType.Morning,
                    WorkSchedule = "Monday-Saturday, 8:00 AM - 4:00 PM",
                    WeeklyWorkHours = 48,
                    BasicSalary = 35000,
                    BankAccountNumber = "0123456789",
                    BankName = "Commercial Bank",
                    TaxId = "TAX012345678",
                    AddressLine1 = "741 Science Street",
                    City = "Giza",
                    State = "Giza Governorate",
                    Country = "Egypt",
                    PostalCode = "12511",
                    EmergencyContactName = "Omar Fathy",
                    EmergencyContactPhone = "+202123456789",
                    EmergencyContactRelationship = "Father",
                    BloodGroup = "B+",
                    Languages = "Arabic, English",
                    IsActive = true,
                    CreatedBy = createdBy,
                    CreatedAt = DateTime.UtcNow
                }
            };

            return labTechs;
        }

        private List<Domain.Entities.Staff> GetReceptionists(Guid createdBy)
        {
            var receptionists = new List<Domain.Entities.Staff>
            {
                new Domain.Entities.Staff
                {
                    Id = Guid.NewGuid(),
                    StaffNumber = "REC001",
                    UserId = Guid.NewGuid(),
                    StaffType = StaffType.Receptionist,
                    Department = "Front Desk",
                    Position = "Senior Receptionist",
                    JoinDate = new DateTime(2019, 3, 1),
                    EmploymentStatus = EmploymentStatus.Active,
                    EmploymentType = EmploymentType.FullTime,
                    Qualifications = "Diploma in Hospital Administration",
                    YearsOfExperience = 7,
                    ShiftType = ShiftType.Morning,
                    WorkSchedule = "Monday-Friday, 8:00 AM - 4:00 PM",
                    WeeklyWorkHours = 40,
                    BasicSalary = 28000,
                    BankAccountNumber = "1234567891",
                    BankName = "National Bank",
                    TaxId = "TAX123456790",
                    AddressLine1 = "852 Reception Road",
                    City = "Cairo",
                    State = "Cairo Governorate",
                    Country = "Egypt",
                    PostalCode = "11511",
                    EmergencyContactName = "Salma Adel",
                    EmergencyContactPhone = "+202234567890",
                    EmergencyContactRelationship = "Sister",
                    BloodGroup = "O+",
                    Languages = "Arabic, English",
                    IsActive = true,
                    CreatedBy = createdBy,
                    CreatedAt = DateTime.UtcNow
                },
                new Domain.Entities.Staff
                {
                    Id = Guid.NewGuid(),
                    StaffNumber = "REC002",
                    UserId = Guid.NewGuid(),
                    StaffType = StaffType.Receptionist,
                    Department = "Emergency",
                    Position = "Receptionist",
                    JoinDate = new DateTime(2022, 6, 15),
                    EmploymentStatus = EmploymentStatus.Active,
                    EmploymentType = EmploymentType.PartTime,
                    Qualifications = "High School Diploma, Customer Service Training",
                    YearsOfExperience = 2,
                    ShiftType = ShiftType.Evening,
                    WorkSchedule = "Monday-Friday, 3:00 PM - 11:00 PM",
                    WeeklyWorkHours = 40,
                    BasicSalary = 22000,
                    BankAccountNumber = "2345678902",
                    BankName = "Commercial Bank",
                    TaxId = "TAX234567891",
                    AddressLine1 = "963 Front Street",
                    City = "Alexandria",
                    State = "Alexandria Governorate",
                    Country = "Egypt",
                    PostalCode = "21500",
                    EmergencyContactName = "Yasmin Fouad",
                    EmergencyContactPhone = "+202345678901",
                    EmergencyContactRelationship = "Mother",
                    BloodGroup = "A-",
                    Languages = "Arabic, English",
                    IsActive = true,
                    CreatedBy = createdBy,
                    CreatedAt = DateTime.UtcNow
                }
            };

            return receptionists;
        }

        private List<Domain.Entities.Staff> GetAdministrativeStaff(Guid createdBy)
        {
            var adminStaff = new List<Domain.Entities.Staff>
            {
                new Domain.Entities.Staff
                {
                    Id = Guid.NewGuid(),
                    StaffNumber = "ADM001",
                    UserId = Guid.NewGuid(),
                    StaffType = StaffType.Administrator,
                    Department = "Human Resources",
                    Position = "HR Manager",
                    JoinDate = new DateTime(2015, 8, 1),
                    EmploymentStatus = EmploymentStatus.Active,
                    EmploymentType = EmploymentType.FullTime,
                    Qualifications = "MBA in Human Resource Management",
                    YearsOfExperience = 12,
                    ShiftType = ShiftType.Morning,
                    WorkSchedule = "Monday-Friday, 9:00 AM - 5:00 PM",
                    WeeklyWorkHours = 40,
                    BasicSalary = 85000,
                    BankAccountNumber = "3456789013",
                    BankName = "National Bank",
                    TaxId = "TAX345678902",
                    AddressLine1 = "159 Admin Boulevard",
                    City = "Cairo",
                    State = "Cairo Governorate",
                    Country = "Egypt",
                    PostalCode = "11511",
                    EmergencyContactName = "Tarek Nabil",
                    EmergencyContactPhone = "+202456789012",
                    EmergencyContactRelationship = "Husband",
                    BloodGroup = "AB-",
                    Languages = "Arabic, English",
                    IsActive = true,
                    CreatedBy = createdBy,
                    CreatedAt = DateTime.UtcNow
                },
                new Domain.Entities.Staff
                {
                    Id = Guid.NewGuid(),
                    StaffNumber = "ADM002",
                    UserId = Guid.NewGuid(),
                    StaffType = StaffType.Administrator,
                    Department = "Finance",
                    Position = "Finance Officer",
                    JoinDate = new DateTime(2020, 10, 1),
                    EmploymentStatus = EmploymentStatus.Active,
                    EmploymentType = EmploymentType.FullTime,
                    Qualifications = "Bachelor of Commerce, CPA",
                    YearsOfExperience = 6,
                    ShiftType = ShiftType.Morning,
                    WorkSchedule = "Monday-Friday, 8:30 AM - 4:30 PM",
                    WeeklyWorkHours = 40,
                    BasicSalary = 65000,
                    BankAccountNumber = "4567890124",
                    BankName = "Commercial Bank",
                    TaxId = "TAX456789013",
                    AddressLine1 = "357 Finance Avenue",
                    City = "Giza",
                    State = "Giza Governorate",
                    Country = "Egypt",
                    PostalCode = "12511",
                    EmergencyContactName = "Dina Ashraf",
                    EmergencyContactPhone = "+202567890123",
                    EmergencyContactRelationship = "Sister",
                    BloodGroup = "B+",
                    Languages = "Arabic, English",
                    IsActive = true,
                    CreatedBy = createdBy,
                    CreatedAt = DateTime.UtcNow
                }
            };

            return adminStaff;
        }

        private async Task SeedStaffEducation(List<Domain.Entities.Staff> staffMembers)
        {
            var educationRecords = new List<StaffEducation>();

            foreach (var staff in staffMembers.Where(s => s.StaffType == StaffType.Doctor).Take(3))
            {
                educationRecords.Add(new StaffEducation
                {
                    Id = Guid.NewGuid(),
                    StaffId = staff.Id,
                    Degree = "MBBS",
                    Institution = "Cairo University",
                    FieldOfStudy = "Medicine",
                    StartYear = 2005,
                    EndYear = 2011,
                    Country = "Egypt",
                    CreatedAt = DateTime.UtcNow
                });

                educationRecords.Add(new StaffEducation
                {
                    Id = Guid.NewGuid(),
                    StaffId = staff.Id,
                    Degree = "MD",
                    Institution = "Ain Shams University",
                    FieldOfStudy = staff.Specialization,
                    StartYear = 2012,
                    EndYear = 2015,
                    Country = "Egypt",
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _context.StaffEducations.AddRangeAsync(educationRecords);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Seeded {educationRecords.Count} education records");
        }

        private async Task SeedStaffExperience(List<Domain.Entities.Staff> staffMembers)
        {
            var experienceRecords = new List<StaffExperience>();

            foreach (var staff in staffMembers.Where(s => s.YearsOfExperience > 5).Take(5))
            {
                experienceRecords.Add(new StaffExperience
                {
                    Id = Guid.NewGuid(),
                    StaffId = staff.Id,
                    Organization = "City General Hospital",
                    Position = "Junior " + staff.Position,
                    Department = staff.Department,
                    StartDate = staff.JoinDate.AddYears(-6),
                    EndDate = staff.JoinDate.AddDays(-1),
                    IsCurrentPosition = false,
                    Description = $"Previous experience in {staff.Department} department",
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _context.StaffExperiences.AddRangeAsync(experienceRecords);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Seeded {experienceRecords.Count} experience records");
        }

        private async Task SeedStaffCertifications(List<Domain.Entities.Staff> staffMembers)
        {
            var certifications = new List<StaffCertification>();

            foreach (var staff in staffMembers.Where(s => s.StaffType == StaffType.Doctor || s.StaffType == StaffType.Nurse).Take(5))
            {
                certifications.Add(new StaffCertification
                {
                    Id = Guid.NewGuid(),
                    StaffId = staff.Id,
                    CertificationName = "Advanced Life Support (ALS)",
                    IssuingOrganization = "American Heart Association",
                    CertificateNumber = $"ALS-{Random.Shared.Next(10000, 99999)}",
                    IssueDate = DateTime.UtcNow.AddYears(-2),
                    ExpiryDate = DateTime.UtcNow.AddYears(1),
                    NeverExpires = false,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _context.StaffCertifications.AddRangeAsync(certifications);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Seeded {certifications.Count} certifications");
        }

        private async Task SeedStaffAttendance(List<Domain.Entities.Staff> staffMembers)
        {
            var attendanceRecords = new List<StaffAttendance>();
            var today = DateTime.Today;

            for (int i = 0; i < 7; i++)
            {
                var date = today.AddDays(-i);
                foreach (var staff in staffMembers.Where(s => s.IsActive))
                {
                    var status = Random.Shared.Next(100) < 95 ? AttendanceStatus.Present : AttendanceStatus.Absent;
                    attendanceRecords.Add(new StaffAttendance
                    {
                        Id = Guid.NewGuid(),
                        StaffId = staff.Id,
                        Date = date,
                        CheckInTime = status == AttendanceStatus.Present ? new TimeSpan(8, Random.Shared.Next(0, 30), 0) : null,
                        CheckOutTime = status == AttendanceStatus.Present ? new TimeSpan(16, Random.Shared.Next(0, 60), 0) : null,
                        Status = status,
                        Notes = status == AttendanceStatus.Absent ? "Sick leave" : null,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            await _context.StaffAttendances.AddRangeAsync(attendanceRecords);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Seeded {attendanceRecords.Count} attendance records");
        }

        private async Task SeedStaffLeaves(List<Domain.Entities.Staff> staffMembers)
        {
            var leaves = new List<StaffLeave>();

            foreach (var staff in staffMembers.Take(5))
            {
                leaves.Add(new StaffLeave
                {
                    Id = Guid.NewGuid(),
                    StaffId = staff.Id,
                    Type = LeaveType.Annual,
                    StartDate = DateTime.Today.AddDays(30),
                    EndDate = DateTime.Today.AddDays(37),
                    TotalDays = 7,
                    Reason = "Family vacation",
                    Status = LeaveStatus.Approved,
                    CreatedAt = DateTime.UtcNow,
                    ApprovedBy = staff.CreatedBy,
                    ApprovedAt = DateTime.UtcNow.AddDays(-5)
                });

                leaves.Add(new StaffLeave
                {
                    Id = Guid.NewGuid(),
                    StaffId = staff.Id,
                    Type = LeaveType.Sick,
                    StartDate = DateTime.Today.AddDays(-10),
                    EndDate = DateTime.Today.AddDays(-8),
                    TotalDays = 2,
                    Reason = "Medical consultation",
                    Status = LeaveStatus.Approved,
                    CreatedAt = DateTime.UtcNow.AddDays(-12),
                    ApprovedBy = staff.CreatedBy,
                    ApprovedAt = DateTime.UtcNow.AddDays(-11)
                });
            }

            await _context.StaffLeaves.AddRangeAsync(leaves);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Seeded {leaves.Count} leave records");
        }

        private async Task SeedStaffPerformanceReviews(List<Domain.Entities.Staff> staffMembers)
        {
            var reviews = new List<StaffPerformanceReview>();

            foreach (var staff in staffMembers.Where(s => s.JoinDate < DateTime.UtcNow.AddYears(-1)).Take(8))
            {
                reviews.Add(new StaffPerformanceReview
                {
                    Id = Guid.NewGuid(),
                    StaffId = staff.Id,
                    ReviewerId = staff.CreatedBy,
                    ReviewDate = DateTime.UtcNow.AddMonths(-3),
                    ReviewPeriod = "Q4 2024",
                    OverallRating = Random.Shared.Next(3, 6),
                    Strengths = "Excellent communication skills, dedicated to patient care, punctual",
                    AreasForImprovement = "Time management during peak hours, documentation accuracy",
                    Goals = "Complete advanced training certification, mentor junior staff",
                    Comments = "Valuable team member with consistent performance",
                    CreatedAt = DateTime.UtcNow.AddMonths(-3)
                });
            }

            await _context.StaffPerformanceReviews.AddRangeAsync(reviews);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Seeded {reviews.Count} performance reviews");
        }
    }
}