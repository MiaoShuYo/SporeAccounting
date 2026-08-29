using AutoMapper;
using SP.CurrencyService.Models.Entity;
using SP.CurrencyService.Models.Response;

namespace SP.CurrencyService;

public class CurrencyProfile : Profile
{
    public CurrencyProfile()
    {
        CreateMap<Currency, CurrencyResponse>();
        CreateMap<ExchangeRateRecord, ExchangeRateRecordResponse>();
    }
}