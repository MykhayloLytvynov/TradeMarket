using AutoMapper;
using Business.Models;
using Data.Entities;
using System.Linq;

namespace Business
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<Receipt, ReceiptModel>()
                .ForMember(rm => rm.ReceiptDetailsIds, r => r.MapFrom(x => x.ReceiptDetails.Select(rd => rd.Id)))
                .ReverseMap();

            CreateMap<Product, ProductModel>()
             .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id)) 
             .ForMember(dest => dest.ProductCategoryId, opt => opt.MapFrom(src => src.ProductCategoryId))
             .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName)) 
             .ForMember(dest => dest.ReceiptDetailIds, opt => opt.MapFrom(src => src.ReceiptDetails.Select(rd => rd.Id))).ReverseMap(); 

            CreateMap<ReceiptDetail, ReceiptDetailModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
                .ForMember(dest => dest.DiscountUnitPrice, opt => opt.MapFrom(src => src.DiscountUnitPrice))
                .ForMember(dest => dest.ReceiptId, opt => opt.MapFrom(src => src.ReceiptId)).ReverseMap();

            CreateMap<Customer, CustomerModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id)) 
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Person.Name)) 
            .ForMember(dest => dest.Surname, opt => opt.MapFrom(src => src.Person.Surname)) 
            .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.Person.BirthDate))
            .ForMember(dest => dest.ReceiptsIds, opt => opt.MapFrom(src => src.Receipts.Select(r => r.Id))).ReverseMap();

            CreateMap<ProductCategory, ProductCategoryModel>()
            .ForMember(dest => dest.ProductIds, opt => opt.MapFrom(src => src.Products.Select(p => p.ProductCategoryId))).ReverseMap();
        }
    }
}
