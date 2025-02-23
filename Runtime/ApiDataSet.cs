// using System.Collections;
// using System.Collections.Generic;
// using Cysharp.Threading.Tasks;
//
// public class ApiDataSet<TEntity> : IEnumerable<TEntity> where TEntity : IEntity
// {
//     private IApiCrudOperations<TEntity> _operations;
//     private Dictionary<string, TEntity> _cachedEntities = new(10);
//     private List<TEntity> _cachedPages = new();
//     public ApiDataSet(IApiCrudOperations<TEntity> operations)
//     {
//         _operations = operations;
//     }
//
//     public async UniTask<TEntity> Get(string id, bool useCached = true)
//     {
//         if (useCached)
//         {
//             if (_cachedEntities.TryGetValue(id, out var entity))
//             {
//                 return entity;
//             }
//         }
//         return await _operations.Get(id);
//     }
//     public async UniTask<List<TEntity>> GetAll(bool useCached = true, int page = 0, int pageSize = 10)
//     {
//         return await _operations.GetAll(page, pageSize);
//     }
//
//     public async UniTask<TEntity> Add(TEntity entity, bool invalidateCache = true)
//     {
//         var newEntity = await _operations.Add(entity);
//         if (_cachedEntities.ContainsKey(newEntity.Id))
//         {
//             _cachedEntities[newEntity.Id] = newEntity;
//         }
//         return newEntity;
//     }
//
//     public async UniTask<bool> Update(TEntity entity, bool invalidateCache = true)
//     {
//         if (await _operations.Update(entity))
//         {
//             if (invalidateCache)
//             {
//                 entity = await Get(entity.Id, false);
//                 _cachedEntities[entity.Id] = entity;
//             }
//             return true;
//         }
//         return false;
//     }
//
//     public async UniTask<bool> Delete(TEntity entity)
//     {
//         if (await _operations.Delete(entity))
//         {
//             _cachedEntities.Remove(entity.Id);
//             return true;
//         }
//         return false;
//     }
//
//     public IEnumerator<TEntity> GetEnumerator()
//     {
//         throw new System.NotImplementedException();
//     }
//
//     IEnumerator IEnumerable.GetEnumerator()
//     {
//         return GetEnumerator();
//     }
// }