#:sdk Microsoft.NET.Sdk.Web
#:package BTreeDotNet@1.0.0
#:property TargetFramework=net11.0
#:property Nullable=enable

using System;
using System.Collections.Concurrent;
using System.Threading;
using BTreeDotNet;

public class TreeNode
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int? ParentId { get; set; }
    public int Order { get; set; }
    public List<int> ChildrenIds { get; set; } = new();
}

public interface IBTreeService
{
    Task<TreeNode> GetNodeAsync(int id);
    Task<IEnumerable<TreeNode>> GetDescendantsAsync(int nodeId);
    Task<IEnumerable<TreeNode>> SearchNodesAsync(string keyword);
    Task<TreeNode> AddNodeAsync(TreeNode node);
    Task UpdateNodeAsync(TreeNode node);
    Task DeleteNodeAsync(int id);
    Task MoveNodeAsync(int nodeId, int? newParentId);
}

public class BTreeService : IBTreeService
{
    private readonly BTree<int, TreeNode> _bTree;
    private readonly ConcurrentDictionary<string, List<int>> _nameIndex = new();
    private readonly ConcurrentDictionary<int, int> _parentIndex = new();
    private readonly ReaderWriterLockSlim _rwLock = new();

    public BTreeService()
    {
        _bTree = new BTree<int, TreeNode>(Comparer<int>.Default, 32);
    }

    public async Task<TreeNode> GetNodeAsync(int id)
    {
        _rwLock.EnterReadLock();
        try
        {
            return _bTree.Search(id);
        }
        finally
        {
            _rwLock.ExitReadLock();
        }
    }

    public async Task<IEnumerable<TreeNode>> GetDescendantsAsync(int nodeId)
    {
        _rwLock.EnterReadLock();
        try
        {
            var result = new List<TreeNode>();
            var queue = new Queue<int>();
            queue.Enqueue(nodeId);

            while (queue.Count > 0)
            {
                var currentId = queue.Dequeue();
                var currentNode = _bTree.Search(currentId);
                if (currentNode != null)
                {
                    result.Add(currentNode);
                    foreach (var childId in currentNode.ChildrenIds)
                    {
                        queue.Enqueue(childId);
                    }
                }
            }
            return result;
        }
        finally
        {
            _rwLock.ExitReadLock();
        }
    }

    public async Task<TreeNode> AddNodeAsync(TreeNode node)
    {
        _rwLock.EnterWriteLock();
        try
        {
            _bTree.Insert(node.Id, node);
            UpdateIndexes(node);
            return node;
        }
        finally
        {
            _rwLock.ExitWriteLock();
        }
    }

    private void RemoveFromIndexes(TreeNode node)
    {
        // 从名称索引移除
        if (_nameIndex.TryGetValue(node.Name.ToLower(), out var nameList))
        {
            nameList.Remove(node.Id);
            if (nameList.Count == 0)
            {
                _nameIndex.TryRemove(node.Name.ToLower(), out _);
            }
        }

        // 从父节点索引移除
        _parentIndex.TryRemove(node.Id, out _);
    }

    private void UpdateIndexes(TreeNode node)
    {
        // 更新名称索引
        _nameIndex.AddOrUpdate(node.Name.ToLower(), 
            new List<int> { node.Id }, 
            (_, list) => { list.Add(node.Id); return list; });

        // 更新父节点索引
        if (node.ParentId.HasValue)
        {
            _parentIndex[node.Id] = node.ParentId.Value;
        }
        else
        {
            _parentIndex.TryRemove(node.Id, out _);
        }
    }

    public async Task UpdateNodeAsync(TreeNode node)
    {
        _rwLock.EnterWriteLock();
        try
        {
            if (_bTree.Search(node.Id) is { } existingNode)
            {
                _bTree.Delete(node.Id);
                _bTree.Insert(node.Id, node);
                UpdateIndexes(node);
            }
        }
        finally
        {
            _rwLock.ExitWriteLock();
        }
    }

    public async Task DeleteNodeAsync(int id)
    {
        _rwLock.EnterWriteLock();
        try
        {
            if (_bTree.Search(id) is { } node)
            {
                _bTree.Delete(id);
                RemoveFromIndexes(node);
            }
        }
        finally
        {
            _rwLock.ExitWriteLock();
        }
    }

    public async Task MoveNodeAsync(int nodeId, int? newParentId)
    {
        _rwLock.EnterWriteLock();
        try
        {
            if (_bTree.Search(nodeId) is { } node)
            {
                // 从原父节点移除
                if (node.ParentId.HasValue && _bTree.Search(node.ParentId.Value) is { } oldParent)
                {
                    oldParent.ChildrenIds.Remove(nodeId);
                    _bTree.Insert(oldParent.Id, oldParent);
                }

                // 添加到新父节点
                if (newParentId.HasValue && _bTree.Search(newParentId.Value) is { } newParent)
                {
                    newParent.ChildrenIds.Add(nodeId);
                    _bTree.Insert(newParent.Id, newParent);
                }

                // 更新节点
                node.ParentId = newParentId;
                _bTree.Insert(node.Id, node);
                UpdateIndexes(node);
            }
        }
        finally
        {
            _rwLock.ExitWriteLock();
        }
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBTreeServices(this IServiceCollection services)
    {
        services.AddSingleton<IBTreeService, BTreeService>();
        return services;
    }
}