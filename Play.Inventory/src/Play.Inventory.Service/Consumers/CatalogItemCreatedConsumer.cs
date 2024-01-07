using MassTransit;
using Play.Catalog.Contracts;
using Play.Common;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Consumers;

public class CatalogItemCreatedConsumer : IConsumer<CatalogItemCreated>
{
    private readonly IRepository<CatalogItem> repository;

    public CatalogItemCreatedConsumer(IRepository<CatalogItem> repository) => this.repository = repository;

    public async Task Consume(ConsumeContext<CatalogItemCreated> context)
    {
        var message = context.Message;

        var item = await repository.Get(message.ItemId);

        if (item != null)
        {
            return;
        }

        await repository.Create(new CatalogItem
        {
            Id = message.ItemId,
            Name = message.Name,
            Description = message.Description
        });
    }
}
