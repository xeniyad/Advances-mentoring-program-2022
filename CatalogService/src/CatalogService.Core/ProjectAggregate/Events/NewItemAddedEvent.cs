using CatalogService.SharedKernel;

namespace CatalogService.Core.ProjectAggregate.Events;

  public class NewItemAddedEvent : DomainEventBase
  {
      public Item NewItem { get; set; }
      public Category Category { get; set; }

      public NewItemAddedEvent(Category category,
          Item newItem)
      {
          Category = category;
          NewItem = newItem;
      }
  }
