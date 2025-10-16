using AutoMapper;
using Common.Pagination;
using Common.Utilities;
using Core.Entities;
using Data.Dto;

namespace WebFrameWork.Mapper
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            ///User
            CreateMap<UserDto, User>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();

           
            ///Address
            CreateMap<SettingsDto, Settings>().ReverseMap();
            CreateMap<Settings, SettingsDto>().ReverseMap();

            ///Address
            CreateMap<AddressDto, Address>().ReverseMap();
            CreateMap<Address, AddressDto>().ReverseMap();

            CreateMap<LawyerDto, Lawyer>().ReverseMap();
            CreateMap<Lawyer, LawyerDto>().ReverseMap();

            CreateMap<PagedResponse<Address>, PagedResponse<AddressDto>>().ReverseMap();
            CreateMap<PagedResponse<IEnumerable<Address>>, PagedResponse<IEnumerable<AddressDto>>>().ReverseMap();

            ///Agreement
            CreateMap<AgreementDto, Agreement>().ReverseMap();
            CreateMap<Agreement, AgreementDto>().ReverseMap();

            CreateMap<PagedResponse<Agreement>, PagedResponse<AgreementDto>>().ReverseMap();
            CreateMap<PagedResponse<IEnumerable<Agreement>>, PagedResponse<IEnumerable<AgreementDto>>>().ReverseMap();

            ///AgreementDetail
            CreateMap<AgreementDetailDto, AgreementDetail>().ReverseMap();
            CreateMap<AgreementDetail, AgreementDetailDto>().ReverseMap();

            CreateMap<PagedResponse<AgreementDetail>, PagedResponse<AgreementDetailDto>>().ReverseMap();
            CreateMap<PagedResponse<IEnumerable<AgreementDetail>>, PagedResponse<IEnumerable<AgreementDetailDto>>>().ReverseMap();

            ///Bank
            CreateMap<BankDto, Bank>().ReverseMap();
            CreateMap<Bank, BankDto>().ReverseMap();

            CreateMap<PagedResponse<Bank>, PagedResponse<BankDto>>().ReverseMap();
            CreateMap<PagedResponse<IEnumerable<Bank>>, PagedResponse<IEnumerable<BankDto>>>().ReverseMap();

            ///Box
            CreateMap<BoxDto, Box>().ReverseMap();
            CreateMap<Box, BoxDto>().ReverseMap();
            
            CreateMap<PermissionDto, Permission>().ReverseMap();
            CreateMap<Permission, PermissionDto>().ReverseMap();


            CreateMap<RolePermissionDto, RolePermission>().ReverseMap();
            CreateMap<RolePermission, RolePermissionDto>().ReverseMap();


            CreateMap<PagedResponse<Box>, PagedResponse<BoxDto>>().ReverseMap();
            CreateMap<PagedResponse<IEnumerable<Box>>, PagedResponse<IEnumerable<BoxDto>>>().ReverseMap();

            ///BoxType
            CreateMap<BoxTypeDto, BoxType>().ReverseMap();
            CreateMap<BoxType, BoxTypeDto>().ReverseMap();

            CreateMap<PagedResponse<BoxType>, PagedResponse<BoxTypeDto>>().ReverseMap();
            CreateMap<PagedResponse<IEnumerable<BoxType>>, PagedResponse<IEnumerable<BoxTypeDto>>>().ReverseMap();


            ///Branch
            CreateMap<BranchDto, Branch>().ReverseMap();
            CreateMap<Branch, BranchDto>().ReverseMap();

            CreateMap<PagedResponse<Branch>, PagedResponse<BranchDto>>().ReverseMap();
            CreateMap<PagedResponse<IEnumerable<Branch>>, PagedResponse<IEnumerable<BranchDto>>>().ReverseMap();

            ///Branch Manager
            CreateMap<BranchManagerDto, BranchManager>().ReverseMap();
            CreateMap<BranchManager, BranchManagerDto>().ReverseMap();

            CreateMap<PagedResponse<BranchManager>, PagedResponse<BranchManagerDto>>().ReverseMap();
            CreateMap<PagedResponse<IEnumerable<BranchManager>>, PagedResponse<IEnumerable<BranchManagerDto>>>().ReverseMap();

            // City
            CreateMap<CityDto, City>().ReverseMap();
            CreateMap<City, CityDto>().ReverseMap();

            CreateMap<PagedResponse<City>, PagedResponse<CityDto>>().ReverseMap();
            CreateMap<PagedResponse<IEnumerable<City>>, PagedResponse<IEnumerable<CityDto>>>().ReverseMap();

            ///Customer
            CreateMap<CustomerDto, Customer>().ReverseMap();
            CreateMap<Customer, CustomerDto>().ReverseMap();

            CreateMap<PagedResponse<Customer>, PagedResponse<CustomerDto>>().ReverseMap();
            CreateMap<PagedResponse<IEnumerable<Customer>>, PagedResponse<IEnumerable<CustomerDto>>>().ReverseMap();

            ///Customer Type
            CreateMap<CustomerTypeDto, CustomerType>().ReverseMap();
            CreateMap<CustomerType, CustomerTypeDto>().ReverseMap();

            CreateMap<PagedResponse<CustomerType>, PagedResponse<CustomerTypeDto>>().ReverseMap();
            CreateMap<PagedResponse<IEnumerable<CustomerType>>, PagedResponse<IEnumerable<CustomerTypeDto>>>().ReverseMap();

            ///Degree
            CreateMap<DegreeDto, Degree>().ReverseMap();
            CreateMap<Degree, DegreeDto>().ReverseMap();

            CreateMap<PagedResponse<Degree>, PagedResponse<DegreeDto>>().ReverseMap();
            CreateMap<PagedResponse<IEnumerable<Degree>>, PagedResponse<IEnumerable<DegreeDto>>>().ReverseMap();


            ///Foregion Customet
            CreateMap<ForeignCustomerDto, ForeignCustomer>().ReverseMap();
            CreateMap<ForeignCustomer, ForeignCustomerDto>().ReverseMap();

            CreateMap<PagedResponse<ForeignCustomer>, PagedResponse<ForeignCustomerDto>>().ReverseMap();
            CreateMap<PagedResponse<IEnumerable<ForeignCustomer>>, PagedResponse<IEnumerable<ForeignCustomerDto>>>().ReverseMap();

            ///IBan
            CreateMap<IbanDto, Iban>().ReverseMap();
            CreateMap<Iban, IbanDto>().ReverseMap();

            CreateMap<PagedResponse<Iban>, PagedResponse<IbanDto>>().ReverseMap();
            CreateMap<PagedResponse<IEnumerable<Iban>>, PagedResponse<IEnumerable<IbanDto>>>().ReverseMap();

            ///Insurance
            CreateMap<InsuranceDto, Insurance>().ReverseMap();
            CreateMap<Insurance, InsuranceDto>().ReverseMap();

            CreateMap<PagedResponse<Insurance>, PagedResponse<InsuranceDto>>().ReverseMap();
            CreateMap<PagedResponse<IEnumerable<Insurance>>, PagedResponse<IEnumerable<InsuranceDto>>>().ReverseMap();

            ///Lawyer
            CreateMap<LawyerDto, Lawyer>().ReverseMap();
            CreateMap<Lawyer, LawyerDto>().ReverseMap();

            CreateMap<PagedResponse<Lawyer>, PagedResponse<LawyerDto>>().ReverseMap();
            CreateMap<PagedResponse<IEnumerable<Lawyer>>, PagedResponse<IEnumerable<LawyerDto>>>().ReverseMap();


            ///Legal Customer
            CreateMap<LegalCustomerDto, LegalCustomer>().ReverseMap();
            CreateMap<LegalCustomer, LegalCustomerDto>().ReverseMap();

            CreateMap<PagedResponse<LegalCustomer>, PagedResponse<LegalCustomerDto>>().ReverseMap();
            CreateMap<PagedResponse<IEnumerable<LegalCustomer>>, PagedResponse<IEnumerable<LegalCustomerDto>>>().ReverseMap();

            ///Region Code
            CreateMap<RegionCodeDto, RegionCode>().ReverseMap();
            CreateMap<RegionCode, RegionCodeDto>().ReverseMap();

            CreateMap<PagedResponse<RegionCode>, PagedResponse<RegionCodeDto>>().ReverseMap();
            CreateMap<PagedResponse<IEnumerable<RegionCode>>, PagedResponse<IEnumerable<RegionCodeDto>>>().ReverseMap();


            ///Real Customer
            CreateMap<RealCustomerDto, RealCustomer>().ReverseMap();
            CreateMap<RealCustomer, RealCustomerDto>().ReverseMap().ForMember(x => x.BirthDate,
                     opt => opt.MapFrom(src => ((ConvertDate.ToMiladi(src.PersianBirthDate))))); 

            CreateMap<PagedResponse<RealCustomer>, PagedResponse<RealCustomerDto>>().ReverseMap();
            CreateMap<PagedResponse<IEnumerable<RealCustomer>>, PagedResponse<IEnumerable<RealCustomerDto>>>().ReverseMap();

            ///Log
            CreateMap<LogDto, Log>().ReverseMap();
            CreateMap<Log, LogDto>().ReverseMap();

            CreateMap<PagedResponse<Log>, PagedResponse<LogDto>>().ReverseMap();
            CreateMap<PagedResponse<IEnumerable<Log>>, PagedResponse<IEnumerable<LogDto>>>().ReverseMap();


            ///Repsoitory
            CreateMap<RepositoryDto, Repository>().ReverseMap();
            CreateMap<Repository, RepositoryDto>().ReverseMap();

            CreateMap<PagedResponse<Repository>, PagedResponse<RepositoryDto>>().ReverseMap();
            CreateMap<PagedResponse<IEnumerable<Repository>>, PagedResponse<IEnumerable<RepositoryDto>>>().ReverseMap();

            ///Repsoitory Column
            CreateMap<RepositoryColumnDto, RepositoryColumn>().ReverseMap();
            CreateMap<RepositoryColumn, RepositoryColumnDto>().ReverseMap();

            CreateMap<PagedResponse<RepositoryColumn>, PagedResponse<RepositoryColumnDto>>().ReverseMap();
            CreateMap<PagedResponse<IEnumerable<RepositoryColumn>>, PagedResponse<IEnumerable<RepositoryColumnDto>>>().ReverseMap();

            ///Sms
            CreateMap<SmsDto, Sms>().ReverseMap();
            CreateMap<Sms, SmsDto>().ReverseMap();

            CreateMap<PagedResponse<Sms>, PagedResponse<SmsDto>>().ReverseMap();
            CreateMap<PagedResponse<IEnumerable<Sms>>, PagedResponse<IEnumerable<SmsDto>>>().ReverseMap();

            // State
            CreateMap<StateDto, State>().ReverseMap();
            CreateMap<State, StateDto>().ReverseMap();

            CreateMap<PagedResponse<State>, PagedResponse<StateDto>>().ReverseMap();
            CreateMap<PagedResponse<IEnumerable<State>>, PagedResponse<IEnumerable<StateDto>>>().ReverseMap();

            // Transaction
            CreateMap<TransactionAgreementDto, TransactionAgreement>().ReverseMap();
            CreateMap<TransactionAgreement, TransactionAgreementDto>().ReverseMap();

            CreateMap<PagedResponse<TransactionAgreement>, PagedResponse<TransactionAgreementDto>>().ReverseMap();
            CreateMap<PagedResponse<IEnumerable<TransactionAgreement>>, PagedResponse<IEnumerable<TransactionAgreementDto>>>().ReverseMap();

            //Role 
            CreateMap<RoleDto, Role>().ReverseMap();
            CreateMap<Role, RoleDto>().ReverseMap();

            CreateMap<ColumnTypesDto, ColumnTypes>().ReverseMap();
            CreateMap<ColumnTypes, ColumnTypesDto>().ReverseMap();
            
        }
    }
}
