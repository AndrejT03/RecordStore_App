using RecordStore.Domain.DomainModels;
using RecordStore.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordStore.Service.Interface
{
    public interface IShoppingCartService
    {
        ShoppingCart? GetByUserId(Guid userId);
        ShoppingCartDTO GetByUserIdWithIncludedPrducts(Guid userId, bool applyPoints = false);
        void IncreaseQuantityInShoppingCart(Guid userId, Guid recordId);
        void ReduceQuantityInShoppingCart(Guid userId, Guid recordId);
        void DeleteProductFromShoppingCart(Guid userId, Guid recordId);
        Task<(bool IsSuccess, string EmailContent)> OrderRecords(Guid userId, bool pointsApplied);
    }
}