using gigadr3w.msauthflow.backoffice.iterator.Models;
using gigadr3w.msauthflow.common.Exceptions;
using gigadr3w.msauthflow.dataaccess.Interfaces;
using gigadr3w.msauthflow.entities;
using System.Reflection.Metadata.Ecma335;

namespace gigadr3w.msauthflow.backoffice.iterator.Services
{
    public interface IItemService
    {
        Task<List<ItemModel>> List();
        Task<ItemModel> GetOrThrow(int Id);
        Task<ItemModel> Add(ItemModel model);
        Task<ItemModel> UpdateOrThrow(ItemModel model);
        Task DeleteOrThrow(int Id);
    }

    public class ItemService : IItemService
    {
        private readonly IDataAccess<Item> _items;

        public ItemService(IDataAccess<Item> items)
            => _items = items;

        private Item Map(ItemModel model)
            => new Item { Id = model.Id, Name = model.Name, Description = model.Description, Value = model.Value };

        private ItemModel Map(Item entity)
            => new ItemModel { Id = entity.Id, Name = entity.Name, Description = entity.Description, Value = entity.Value };

        public async Task<ItemModel> Add(ItemModel model)
        {
            Item entity = Map(model);
            await _items.Add(entity);
            return Map(entity);
        }

        public async Task DeleteOrThrow(int Id)
        {
            ItemModel model = await GetOrThrow(Id);
            if (model == null) throw new EntityNotFoundException($"Item with Id:{Id} not found");

            Item item = Map(model);
            await _items.Delete(item);
        }

        public async Task<ItemModel> GetOrThrow(int Id)
        {
            Item? item = await _items.Get(Id);
            if (item == null) throw new EntityNotFoundException($"Item with Id:{Id} not found");

            return Map(item);
        }

        public async Task<List<ItemModel>> List()
            => ((await _items.List()).Select(item => Map(item)) ?? throw new EntityNotFoundException("Items not found")).ToList();

        public async Task<ItemModel> UpdateOrThrow(ItemModel model)
        {
            if(await _items.Get(model.Id) == null) throw new EntityNotFoundException($"Item with Id:{model.Id} not found");

            Item entity = Map(model);
            await _items.Update(entity);

            return Map(entity);
        }
    }
}
