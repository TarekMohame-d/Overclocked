namespace Application.Services.Product.Mapping;

public static class CollectionExtensions
{
    public static void Reconcile<TEntity, TDto, TKey>(
        this ICollection<TEntity> dbCollection,
        IEnumerable<TDto> dtoCollection,
        Func<TEntity, TKey> entityKeySelector,
        Func<TDto, TKey> dtoKeySelector,
        Func<TDto, TEntity> createFromDto,
        Action<TEntity, TDto>? updateExisting = null)
    {
        var dtoList = dtoCollection.ToList();

        // 1. DELETE: Find items in DB that are NOT in the DTO list
        var idsInDto = dtoList.Select(dtoKeySelector).ToHashSet();
        var toRemove = dbCollection
            .Where(e => !idsInDto.Contains(entityKeySelector(e)))
            .ToList();

        foreach(TEntity? item in toRemove)
            dbCollection.Remove(item);

        // 2. ADD & UPDATE
        foreach(TDto? dto in dtoList)
        {
            TEntity? existingItem = dbCollection
                .FirstOrDefault(e => EqualityComparer<TKey>.Default.Equals(entityKeySelector(e), dtoKeySelector(dto)));

            if(existingItem == null)
            {
                dbCollection.Add(createFromDto(dto));
            }
            else
            {
                updateExisting?.Invoke(existingItem, dto);
            }
        }
    }
}
