using AdminAPI.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using X.PagedList;
namespace AdminAPI.Utilities;
public static class MiscellaneousExtensionUtilities
{
    public static bool HasMoreThanNDecimalPlaces(this decimal value, int n) => decimal.Round(value, n) != value;
    public static bool HasMoreThanTwoDecimalPlaces(this decimal value) => value.HasMoreThanNDecimalPlaces(2);

    public static IPagedList<Transaction> ConverterUtcTimeToLocalTime(IPagedList<Transaction> list)
    {
        foreach(var transaction in list)
        {
            transaction.TransactionTimeUtc = transaction.TransactionTimeUtc.ToLocalTime(); 
        }
        return list;
    }
}

