//#!/usr/bin/env dotnet
//#:sdk Microsoft.NET.Sdk.Web
//#:package Scrutor@7.0.0
//#:package Carter@10.0.0
//#:package Microsoft.EntityFrameworkCore.InMemory@10.0.3
//#:package Microsoft.EntityFrameworkCore.Sqlite@10.0.3
//#:package Microsoft.Extensions.Caching.Memory@10.0.3
//#:property TargetFramework=net10.0
//#:property RollForward=Major
//#:property Nullable=enable
//#:property ImplicitUsings=enable
//#:property PublishAot=false
//#:property PublishReadyToRun=false
//#:property PublishSingleFile=false

using System.Collections.Generic;

namespace System.Threading.Tasks.Go;

/// <summary>
/// 映射节点类，用于Map<TKey, TValue>中的红黑树节点
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
public class MapNode<TKey, TValue>
{
    /// <summary>
    /// 节点的键
    /// </summary>
    internal TKey key;
    /// <summary>
    /// 节点的值
    /// </summary>
    internal TValue value;
    /// <summary>
    /// 父节点
    /// </summary>
    internal MapNode<TKey, TValue> parent;
    /// <summary>
    /// 左子节点
    /// </summary>
    internal MapNode<TKey, TValue> left;
    /// <summary>
    /// 右子节点
    /// </summary>
    internal MapNode<TKey, TValue> right;
    /// <summary>
    /// 节点颜色（红黑树）
    /// </summary>
    internal Map<TKey, TValue>.rb_color color;
    /// <summary>
    /// 是否为nil节点
    /// </summary>
    internal readonly bool nil;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="n">是否为nil节点</param>
    internal MapNode(bool n)
    {
        nil = n;
    }

    /// <summary>
    /// 获取下一个节点
    /// </summary>
    public MapNode<TKey, TValue> Next
    {
        get
        {
            MapNode<TKey, TValue> pNode = Map<TKey, TValue>.next(this);
            return pNode.nil ? null : pNode;
        }
    }

    /// <summary>
    /// 获取上一个节点
    /// </summary>
    public MapNode<TKey, TValue> Prev
    {
        get
        {
            MapNode<TKey, TValue> pNode = Map<TKey, TValue>.previous(this);
            return pNode.nil ? null : pNode;
        }
    }

    /// <summary>
    /// 获取节点的键
    /// </summary>
    public TKey Key
    {
        get
        {
            return key;
        }
    }

    /// <summary>
    /// 获取或设置节点的值
    /// </summary>
    public TValue Value
    {
        get
        {
            return value;
        }
        set
        {
            this.value = value;
        }
    }

    /// <summary>
    /// 节点是否孤立（没有父节点）
    /// </summary>
    public bool Isolated
    {
        get
        {
            return null == parent;
        }
    }

    /// <summary>
    /// 重写ToString方法，返回节点的字符串表示
    /// </summary>
    /// <returns>节点的字符串表示</returns>
    public override string ToString()
    {
        return string.Format("({0},{1})", key, value);
    }
}

/// <summary>
/// 映射类，使用红黑树实现的有序映射
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
public class Map<TKey, TValue>
{
    /// <summary>
    /// 红黑树节点颜色枚举
    /// </summary>
    internal enum rb_color
    {
        /// <summary>
        /// 红色节点
        /// </summary>
        red,
        /// <summary>
        /// 黑色节点
        /// </summary>
        black
    }

    /// <summary>
    /// 节点数量
    /// </summary>
    int _count;
    /// <summary>
    /// 是否允许多个相同键的节点
    /// </summary>
    readonly bool _multi;
    /// <summary>
    /// 头节点（哨兵节点）
    /// </summary>
    readonly MapNode<TKey, TValue> _head;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="multi">是否允许多个相同键的节点</param>
    public Map(bool multi = false)
    {
        _count = 0;
        _multi = multi;
        _head = new MapNode<TKey, TValue>(true);
        _head.color = rb_color.black;
        root = lmost = rmost = _head;
    }

    /// <summary>
    /// 创建新节点
    /// </summary>
    /// <param name="key">节点的键</param>
    /// <param name="value">节点的值</param>
    /// <returns>新创建的节点</returns>
    static public MapNode<TKey, TValue> NewNode(TKey key, TValue value)
    {
        return new MapNode<TKey, TValue>(false) { key = key, value = value, color = rb_color.red };
    }

    /// <summary>
    /// 重新使用节点
    /// </summary>
    /// <param name="oldNode">旧节点</param>
    /// <param name="key">新的键</param>
    /// <param name="value">新的值</param>
    /// <returns>重新使用的节点</returns>
    public MapNode<TKey, TValue> ReNewNode(MapNode<TKey, TValue> oldNode, TKey key, TValue value)
    {
        if (!oldNode.Isolated)
        {
            Remove(oldNode);
        }
        oldNode.key = key;
        oldNode.value = value;
        oldNode.color = rb_color.red;
        return oldNode;
    }

    /// <summary>
    /// 比较两个值是否小于
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <param name="x">第一个值</param>
    /// <param name="y">第二个值</param>
    /// <returns>如果x小于y返回true，否则返回false</returns>
    static bool comp_lt<T>(T x, T y)
    {
        return Comparer<T>.Default.Compare(x, y) < 0;
    }

    /// <summary>
    /// 检查节点是否为nil节点
    /// </summary>
    /// <param name="node">要检查的节点</param>
    /// <returns>如果是nil节点返回true，否则返回false</returns>
    static bool is_nil(MapNode<TKey, TValue> node)
    {
        return node.nil;
    }

    /// <summary>
    /// 根节点
    /// </summary>
    MapNode<TKey, TValue> root
    {
        get
        {
            return _head.parent;
        }
        set
        {
            _head.parent = value;
        }
    }

    /// <summary>
    /// 最左节点（最小值节点）
    /// </summary>
    MapNode<TKey, TValue> lmost
    {
        get
        {
            return _head.left;
        }
        set
        {
            _head.left = value;
        }
    }

    /// <summary>
    /// 最右节点（最大值节点）
    /// </summary>
    MapNode<TKey, TValue> rmost
    {
        get
        {
            return _head.right;
        }
        set
        {
            _head.right = value;
        }
    }

    /// <summary>
    /// 左旋操作
    /// </summary>
    /// <param name="whereNode">要旋转的节点</param>
    void left_rotate(MapNode<TKey, TValue> whereNode)
    {
        MapNode<TKey, TValue> pNode = whereNode.right;
        whereNode.right = pNode.left;
        if (!is_nil(pNode.left))
        {
            pNode.left.parent = whereNode;
        }
        pNode.parent = whereNode.parent;
        if (whereNode == root)
        {
            root = pNode;
        }
        else if (whereNode == whereNode.parent.left)
        {
            whereNode.parent.left = pNode;
        }
        else
        {
            whereNode.parent.right = pNode;
        }
        pNode.left = whereNode;
        whereNode.parent = pNode;
    }

    /// <summary>
    /// 右旋操作
    /// </summary>
    /// <param name="whereNode">要旋转的节点</param>
    void right_rotate(MapNode<TKey, TValue> whereNode)
    {
        MapNode<TKey, TValue> pNode = whereNode.left;
        whereNode.left = pNode.right;
        if (!is_nil(pNode.right))
        {
            pNode.right.parent = whereNode;
        }
        pNode.parent = whereNode.parent;
        if (whereNode == root)
        {
            root = pNode;
        }
        else if (whereNode == whereNode.parent.right)
        {
            whereNode.parent.right = pNode;
        }
        else
        {
            whereNode.parent.left = pNode;
        }
        pNode.right = whereNode;
        whereNode.parent = pNode;
    }

    /// <summary>
    /// 在指定位置插入节点
    /// </summary>
    /// <param name="addLeft">是否插入到左侧</param>
    /// <param name="whereNode">插入位置的节点</param>
    /// <param name="newNode">要插入的新节点</param>
    void insert_at(bool addLeft, MapNode<TKey, TValue> whereNode, MapNode<TKey, TValue> newNode)
    {
        newNode.parent = whereNode;
        if (whereNode == _head)
        {
            root = lmost = rmost = newNode;
        }
        else if (addLeft)
        {
            whereNode.left = newNode;
            if (whereNode == lmost)
            {
                lmost = newNode;
            }
        }
        else
        {
            whereNode.right = newNode;
            if (whereNode == rmost)
            {
                rmost = newNode;
            }
        }
        for (MapNode<TKey, TValue> pNode = newNode; rb_color.red == pNode.parent.color;)
        {
            if (pNode.parent == pNode.parent.parent.left)
            {
                whereNode = pNode.parent.parent.right;
                if (rb_color.red == whereNode.color)
                {
                    pNode.parent.color = rb_color.black;
                    whereNode.color = rb_color.black;
                    pNode.parent.parent.color = rb_color.red;
                    pNode = pNode.parent.parent;
                }
                else
                {
                    if (pNode == pNode.parent.right)
                    {
                        pNode = pNode.parent;
                        left_rotate(pNode);
                    }
                    pNode.parent.color = rb_color.black;
                    pNode.parent.parent.color = rb_color.red;
                    right_rotate(pNode.parent.parent);
                }
            }
            else
            {
                whereNode = pNode.parent.parent.left;
                if (rb_color.red == whereNode.color)
                {
                    pNode.parent.color = rb_color.black;
                    whereNode.color = rb_color.black;
                    pNode.parent.parent.color = rb_color.red;
                    pNode = pNode.parent.parent;
                }
                else
                {
                    if (pNode == pNode.parent.left)
                    {
                        pNode = pNode.parent;
                        right_rotate(pNode);
                    }
                    pNode.parent.color = rb_color.black;
                    pNode.parent.parent.color = rb_color.red;
                    left_rotate(pNode.parent.parent);
                }
            }
        }
        root.color = rb_color.black;
        _count++;
    }

    /// <summary>
    /// 插入节点
    /// </summary>
    /// <param name="newNode">要插入的节点</param>
    /// <param name="priorityRight">优先级方向</param>
    void insert(MapNode<TKey, TValue> newNode, bool priorityRight)
    {
        newNode.parent = newNode.left = newNode.right = _head;
        MapNode<TKey, TValue> tryNode = root;
        MapNode<TKey, TValue> whereNode = _head;
        bool addLeft = true;
        while (!is_nil(tryNode))
        {
            whereNode = tryNode;
            addLeft = priorityRight ? comp_lt(newNode.key, tryNode.key) : !comp_lt(tryNode.key, newNode.key);
            tryNode = addLeft ? tryNode.left : tryNode.right;
        }
        if (_multi)
        {
            insert_at(addLeft, whereNode, newNode);
        }
        else
        {
            MapNode<TKey, TValue> where = whereNode;
            if (!addLeft)
            {
            }
            else if (where == lmost)
            {
                insert_at(true, whereNode, newNode);
                return;
            }
            else
            {
                where = previous(where);
            }

            if (comp_lt(where.key, newNode.key))
            {
                insert_at(addLeft, whereNode, newNode);
            }
            else
            {
                newNode.parent = newNode.left = newNode.right = null;
            }
        }
    }

    /// <summary>
    /// 创建内部节点
    /// </summary>
    /// <param name="key">节点的键</param>
    /// <param name="value">节点的值</param>
    /// <returns>创建的内部节点</returns>
    MapNode<TKey, TValue> new_inter_node(TKey key, TValue value)
    {
        MapNode<TKey, TValue> newNode = NewNode(key, value);
        newNode.parent = newNode.left = newNode.right = _head;
        return newNode;
    }

    /// <summary>
    /// 插入键值对
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <param name="priorityRight">优先级方向</param>
    /// <returns>插入的节点</returns>
    MapNode<TKey, TValue>? insert(TKey key, TValue value, bool priorityRight)
    {
        MapNode<TKey, TValue> tryNode = root;
        MapNode<TKey, TValue> whereNode = _head;
        bool addLeft = true;
        while (!is_nil(tryNode))
        {
            whereNode = tryNode;
            addLeft = priorityRight ? comp_lt(key, tryNode.key) : !comp_lt(tryNode.key, key);
            tryNode = addLeft ? tryNode.left : tryNode.right;
        }
        MapNode<TKey, TValue>? newNode = null;
        if (_multi)
        {
            newNode = new_inter_node(key, value);
            insert_at(addLeft, whereNode, newNode);
        }
        else
        {
            MapNode<TKey, TValue> where = whereNode;
            if (!addLeft)
            {
            }
            else if (where == lmost)
            {
                newNode = new_inter_node(key, value);
                insert_at(true, whereNode, newNode);
                return newNode;
            }
            else
            {
                where = previous(where);
            }
            if (comp_lt(where.key, key))
            {
                newNode = new_inter_node(key, value);
                insert_at(addLeft, whereNode, newNode);
            }
        }
        return newNode;
    }

    /// <summary>
    /// 删除节点
    /// </summary>
    /// <param name="where">要删除的节点</param>
    void remove(MapNode<TKey, TValue> where)
    {
        MapNode<TKey, TValue> erasedNode = where;
        where = next(where);
        MapNode<TKey, TValue>? fixNode = null;
        MapNode<TKey, TValue>? fixNodeParent = null;
        MapNode<TKey, TValue> pNode = erasedNode;
        if (is_nil(pNode.left))
        {
            fixNode = pNode.right;
        }
        else if (is_nil(pNode.right))
        {
            fixNode = pNode.left;
        }
        else
        {
            pNode = where;
            fixNode = pNode.right;
        }
        if (pNode == erasedNode)
        {
            fixNodeParent = erasedNode.parent;
            if (!is_nil(fixNode))
            {
                fixNode.parent = fixNodeParent;
            }
            if (root == erasedNode)
            {
                root = fixNode;
            }
            else if (fixNodeParent.left == erasedNode)
            {
                fixNodeParent.left = fixNode;
            }
            else
            {
                fixNodeParent.right = fixNode;
            }
            if (lmost == erasedNode)
            {
                lmost = is_nil(fixNode) ? fixNodeParent : min(fixNode);
            }
            if (rmost == erasedNode)
            {
                rmost = is_nil(fixNode) ? fixNodeParent : max(fixNode);
            }
        }
        else
        {
            erasedNode.left.parent = pNode;
            pNode.left = erasedNode.left;
            if (pNode == erasedNode.right)
            {
                fixNodeParent = pNode;
            }
            else
            {
                fixNodeParent = pNode.parent;
                if (!is_nil(fixNode))
                {
                    fixNode.parent = fixNodeParent;
                }
                fixNodeParent.left = fixNode;
                pNode.right = erasedNode.right;
                erasedNode.right.parent = pNode;
            }
            if (root == erasedNode)
            {
                root = pNode;
            }
            else if (erasedNode.parent.left == erasedNode)
            {
                erasedNode.parent.left = pNode;
            }
            else
            {
                erasedNode.parent.right = pNode;
            }
            pNode.parent = erasedNode.parent;
            rb_color tcol = pNode.color;
            pNode.color = erasedNode.color;
            erasedNode.color = tcol;
        }
        if (rb_color.black == erasedNode.color)
        {
            for (; fixNode != root && rb_color.black == fixNode.color; fixNodeParent = fixNode.parent)
            {
                if (fixNode == fixNodeParent.left)
                {
                    pNode = fixNodeParent.right;
                    if (rb_color.red == pNode.color)
                    {
                        pNode.color = rb_color.black;
                        fixNodeParent.color = rb_color.red;
                        left_rotate(fixNodeParent);
                        pNode = fixNodeParent.right;
                    }
                    if (is_nil(pNode))
                    {
                        fixNode = fixNodeParent;
                    }
                    else if (rb_color.black == pNode.left.color && rb_color.black == pNode.right.color)
                    {
                        pNode.color = rb_color.red;
                        fixNode = fixNodeParent;
                    }
                    else
                    {
                        if (rb_color.black == pNode.right.color)
                        {
                            pNode.left.color = rb_color.black;
                            pNode.color = rb_color.red;
                            right_rotate(pNode);
                            pNode = fixNodeParent.right;
                        }
                        pNode.color = fixNodeParent.color;
                        fixNodeParent.color = rb_color.black;
                        pNode.right.color = rb_color.black;
                        left_rotate(fixNodeParent);
                        break;
                    }
                }
                else
                {
                    pNode = fixNodeParent.left;
                    if (rb_color.red == pNode.color)
                    {
                        pNode.color = rb_color.black;
                        fixNodeParent.color = rb_color.red;
                        right_rotate(fixNodeParent);
                        pNode = fixNodeParent.left;
                    }
                    if (is_nil(pNode))
                    {
                        fixNode = fixNodeParent;
                    }
                    else if (rb_color.black == pNode.right.color && rb_color.black == pNode.left.color)
                    {
                        pNode.color = rb_color.red;
                        fixNode = fixNodeParent;
                    }
                    else
                    {
                        if (rb_color.black == pNode.left.color)
                        {
                            pNode.right.color = rb_color.black;
                            pNode.color = rb_color.red;
                            left_rotate(pNode);
                            pNode = fixNodeParent.left;
                        }
                        pNode.color = fixNodeParent.color;
                        fixNodeParent.color = rb_color.black;
                        pNode.left.color = rb_color.black;
                        right_rotate(fixNodeParent);
                        break;
                    }
                }
            }
            fixNode.color = rb_color.black;
        }
        erasedNode.parent = erasedNode.left = erasedNode.right = null;
        _count--;
    }

    /// <summary>
    /// 查找大于等于指定键的最小节点
    /// </summary>
    /// <param name="key">要查找的键</param>
    /// <returns>大于等于指定键的最小节点</returns>
    MapNode<TKey, TValue> lbound(TKey key)
    {
        MapNode<TKey, TValue> pNode = root;
        MapNode<TKey, TValue> whereNode = _head;
        while (!is_nil(pNode))
        {
            if (comp_lt(pNode.key, key))
            {
                pNode = pNode.right;
            }
            else
            {
                whereNode = pNode;
                pNode = pNode.left;
            }
        }
        return whereNode;
    }

    /// <summary>
    /// 查找小于等于指定键的最大节点
    /// </summary>
    /// <param name="key">要查找的键</param>
    /// <returns>小于等于指定键的最大节点</returns>
    MapNode<TKey, TValue> rbound(TKey key)
    {
        MapNode<TKey, TValue> pNode = root;
        MapNode<TKey, TValue> whereNode = _head;
        while (!is_nil(pNode))
        {
            if (comp_lt(key, pNode.key))
            {
                pNode = pNode.left;
            }
            else
            {
                whereNode = pNode;
                pNode = pNode.right;
            }
        }
        return whereNode;
    }

    /// <summary>
    /// 查找大于等于指定键的最小节点，如果不存在则插入
    /// </summary>
    /// <param name="key">要查找的键</param>
    /// <returns>返回一个元组，第一个元素表示是否找到，第二个元素是找到或插入的节点</returns>
    tuple<bool, MapNode<TKey, TValue>> lbound_insert(TKey key)
    {
        MapNode<TKey, TValue> pNode = root;
        MapNode<TKey, TValue> insertWhereNode = _head;
        MapNode<TKey, TValue> boundWhereNode = _head;
        bool addLeft = true;
        while (!is_nil(pNode))
        {
            insertWhereNode = pNode;
            if (comp_lt(pNode.key, key))
            {
                addLeft = false;
                pNode = pNode.right;
            }
            else
            {
                addLeft = true;
                boundWhereNode = pNode;
                pNode = pNode.left;
            }
        }
        if (!is_nil(boundWhereNode) && !comp_lt(key, boundWhereNode.key))
        {
            return tuple.make(true, boundWhereNode);
        }
        MapNode<TKey, TValue> newNode = new_inter_node(key, default(TValue));
        insert_at(addLeft, insertWhereNode, newNode);
        return tuple.make(false, newNode);
    }

    /// <summary>
    /// 查找小于等于指定键的最大节点，如果不存在则插入
    /// </summary>
    /// <param name="key">要查找的键</param>
    /// <returns>返回一个元组，第一个元素表示是否找到，第二个元素是找到或插入的节点</returns>
    tuple<bool, MapNode<TKey, TValue>> rbound_insert(TKey key)
    {
        MapNode<TKey, TValue> pNode = root;
        MapNode<TKey, TValue> insertWhereNode = _head;
        MapNode<TKey, TValue> boundWhereNode = _head;
        bool addLeft = true;
        while (!is_nil(pNode))
        {
            insertWhereNode = pNode;
            if (comp_lt(key, pNode.key))
            {
                addLeft = false;
                pNode = pNode.left;
            }
            else
            {
                addLeft = true;
                boundWhereNode = pNode;
                pNode = pNode.right;
            }
        }
        if (!is_nil(boundWhereNode) && !comp_lt(boundWhereNode.key, key))
        {
            return tuple.make(true, boundWhereNode);
        }
        MapNode<TKey, TValue> newNode = new_inter_node(key, default(TValue));
        insert_at(addLeft, insertWhereNode, newNode);
        return tuple.make(false, newNode);
    }

    /// <summary>
    /// 查找子树中的最大值节点
    /// </summary>
    /// <param name="pNode">子树的根节点</param>
    /// <returns>子树中的最大值节点</returns>
    static MapNode<TKey, TValue> max(MapNode<TKey, TValue> pNode)
    {
        while (!is_nil(pNode.right))
        {
            pNode = pNode.right;
        }
        return pNode;
    }

    /// <summary>
    /// 查找子树中的最小值节点
    /// </summary>
    /// <param name="pNode">子树的根节点</param>
    /// <returns>子树中的最小值节点</returns>
    static MapNode<TKey, TValue> min(MapNode<TKey, TValue> pNode)
    {
        while (!is_nil(pNode.left))
        {
            pNode = pNode.left;
        }
        return pNode;
    }

    /// <summary>
    /// 查找下一个节点
    /// </summary>
    /// <param name="ptr">当前节点</param>
    /// <returns>下一个节点</returns>
    static internal MapNode<TKey, TValue> next(MapNode<TKey, TValue> ptr)
    {
        if (is_nil(ptr))
        {
            return ptr;
        }
        else if (!is_nil(ptr.right))
        {
            return min(ptr.right);
        }
        else
        {
            MapNode<TKey, TValue> pNode;
            while (!is_nil(pNode = ptr.parent) && ptr == pNode.right)
            {
                ptr = pNode;
            }
            return pNode;
        }
    }

    /// <summary>
    /// 查找上一个节点
    /// </summary>
    /// <param name="ptr">当前节点</param>
    /// <returns>上一个节点</returns>
    static internal MapNode<TKey, TValue> previous(MapNode<TKey, TValue> ptr)
    {
        if (is_nil(ptr))
        {
            return ptr;
        }
        else if (!is_nil(ptr.left))
        {
            return max(ptr.left);
        }
        else
        {
            MapNode<TKey, TValue> pNode;
            while (!is_nil(pNode = ptr.parent) && ptr == pNode.left)
            {
                ptr = pNode;
            }
            return pNode;
        }
    }

    /// <summary>
    /// 递归删除子树
    /// </summary>
    /// <param name="rootNode">子树的根节点</param>
    static void erase(MapNode<TKey, TValue> rootNode)
    {
        for (MapNode<TKey, TValue> pNode = rootNode; !is_nil(pNode); rootNode = pNode)
        {
            erase(pNode.right);
            pNode = pNode.left;
            rootNode.parent = rootNode.left = rootNode.right = null;
        }
    }

    /// <summary>
    /// 获取节点数量
    /// </summary>
    public int Count
    {
        get
        {
            return _count;
        }
    }

    /// <summary>
    /// 清空映射
    /// </summary>
    public void Clear()
    {
        erase(root);
        root = lmost = rmost = _head;
        _count = 0;
    }

    /// <summary>
    /// 检查是否包含指定键
    /// </summary>
    /// <param name="key">要检查的键</param>
    /// <returns>如果包含指定键返回true，否则返回false</returns>
    public bool Has(TKey key)
    {
        MapNode<TKey, TValue> node = lbound(key);
        return !is_nil(node) && !comp_lt(key, node.key);
    }

    /// <summary>
    /// 查找大于等于指定键的第一个节点
    /// </summary>
    /// <param name="key">要查找的键</param>
    /// <returns>大于等于指定键的第一个节点</returns>
    public MapNode<TKey, TValue>? FindFirstGE(TKey key)
    {
        MapNode<TKey, TValue> node = lbound(key);
        return !is_nil(node) ? node : null;
    }

    /// <summary>
    /// 查找小于等于指定键的第一个节点
    /// </summary>
    /// <param name="key">要查找的键</param>
    /// <returns>小于等于指定键的第一个节点</returns>
    public MapNode<TKey, TValue>? FindFirstLE(TKey key)
    {
        MapNode<TKey, TValue> node = rbound(key);
        return !is_nil(node) ? node : null;
    }

    /// <summary>
    /// 查找等于指定键的第一个节点
    /// </summary>
    /// <param name="key">要查找的键</param>
    /// <returns>等于指定键的第一个节点</returns>
    public MapNode<TKey, TValue>? FindFirst(TKey key)
    {
        MapNode<TKey, TValue> node = lbound(key);
        return !is_nil(node) && !comp_lt(key, node.key) ? node : null;
    }

    /// <summary>
    /// 查找等于指定键的最后一个节点
    /// </summary>
    /// <param name="key">要查找的键</param>
    /// <returns>等于指定键的最后一个节点</returns>
    public MapNode<TKey, TValue>? FindLast(TKey key)
    {
        MapNode<TKey, TValue> node = rbound(key);
        return !is_nil(node) && !comp_lt(node.key, key) ? node : null;
    }

    /// <summary>
    /// 查找大于等于指定键的节点
    /// </summary>
    /// <param name="key">要查找的键</param>
    /// <returns>大于等于指定键的节点</returns>
    public MapNode<TKey, TValue>? FindRight(TKey key)
    {
        MapNode<TKey, TValue> node = lbound(key);
        return is_nil(node) ? null : node;
    }

    /// <summary>
    /// 查找小于等于指定键的节点
    /// </summary>
    /// <param name="key">要查找的键</param>
    /// <returns>小于等于指定键的节点</returns>
    public MapNode<TKey, TValue>? FindLeft(TKey key)
    {
        MapNode<TKey, TValue> node = rbound(key);
        return is_nil(node) ? null : node;
    }

    /// <summary>
    /// 获取第一个大于等于指定键的节点，如果不存在则插入
    /// </summary>
    /// <param name="key">要查找的键</param>
    /// <returns>返回一个元组，第一个元素表示是否找到，第二个元素是找到或插入的节点</returns>
    public tuple<bool, MapNode<TKey, TValue>> GetFirst(TKey key)
    {
        return lbound_insert(key);
    }

    /// <summary>
    /// 获取最后一个小于等于指定键的节点，如果不存在则插入
    /// </summary>
    /// <param name="key">要查找的键</param>
    /// <returns>返回一个元组，第一个元素表示是否找到，第二个元素是找到或插入的节点</returns>
    public tuple<bool, MapNode<TKey, TValue>> GetLast(TKey key)
    {
        return rbound_insert(key);
    }

    /// <summary>
    /// 删除节点
    /// </summary>
    /// <param name="node">要删除的节点</param>
    public void Remove(MapNode<TKey, TValue> node)
    {
        remove(node);
    }

    /// <summary>
    /// 插入键值对
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <param name="priorityRight">优先级方向</param>
    /// <returns>插入的节点</returns>
    public MapNode<TKey, TValue>? Insert(TKey key, TValue value, bool priorityRight = true)
    {
        return insert(key, value, _multi ? priorityRight : true);
    }

    /// <summary>
    /// 插入节点
    /// </summary>
    /// <param name="newNode">要插入的节点</param>
    /// <param name="priorityRight">优先级方向</param>
    /// <returns>如果插入成功返回true，否则返回false</returns>
    public bool Insert(MapNode<TKey, TValue> newNode, bool priorityRight = true)
    {
        insert(newNode, _multi ? priorityRight : true);
        return !newNode.Isolated;
    }

    /// <summary>
    /// 获取枚举器
    /// </summary>
    /// <returns>枚举器</returns>
    public IEnumerator<tuple<TKey, TValue>> GetEnumerator()
    {
        MapNode<TKey, TValue> it = First;
        while (null != it)
        {
            yield return tuple.make(it.key, it.value);
            it = it.Next;
        }
        yield break;
    }

    /// <summary>
    /// 转换为列表
    /// </summary>
    public tuple<TKey, TValue>[] ToList
    {
        get
        {
            int idx = 0;
            tuple<TKey, TValue>[] list = new tuple<TKey, TValue>[_count];
            MapNode<TKey, TValue> it = First;
            while (null != it)
            {
                list[idx++] = tuple.make(it.key, it.value);
                it = it.Next;
            }
            return list;
        }
    }

    /// <summary>
    /// 获取第一个节点
    /// </summary>
    public MapNode<TKey, TValue>? First
    {
        get
        {
            return is_nil(lmost) ? null : lmost;
        }
    }

    /// <summary>
    /// 获取最后一个节点
    /// </summary>
    public MapNode<TKey, TValue>? Last
    {
        get
        {
            return is_nil(rmost) ? null : rmost;
        }
    }
}

/// <summary>
/// 消息队列节点类
/// </summary>
/// <typeparam name="T">节点值类型</typeparam>
public class MsgQueueNode<T>
{
    /// <summary>
    /// 节点的值
    /// </summary>
    internal T _value;
    /// <summary>
    /// 下一个节点
    /// </summary>
    internal MsgQueueNode<T>? _next;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="value">节点的值</param>
    public MsgQueueNode(T value)
    {
        _value = value;
        _next = null;
    }

    /// <summary>
    /// 获取或设置节点的值
    /// </summary>
    public T Value
    {
        get
        {
            return _value;
        }
        set
        {
            _value = value;
        }
    }
}

/// <summary>
/// 消息队列类
/// </summary>
/// <typeparam name="T">队列元素类型</typeparam>
public class MsgQueue<T>
{
    /// <summary>
    /// 队列元素数量
    /// </summary>
    int _count;
    /// <summary>
    /// 队首节点
    /// </summary>
    MsgQueueNode<T>? _head;
    /// <summary>
    /// 队尾节点
    /// </summary>
    MsgQueueNode<T>? _tail;

    /// <summary>
    /// 构造函数
    /// </summary>
    public MsgQueue()
    {
        _count = 0;
        _head = _tail = null;
    }

    /// <summary>
    /// 在队尾添加元素
    /// </summary>
    /// <param name="value">要添加的元素</param>
    public void AddLast(T value)
    {
        AddLast(new MsgQueueNode<T>(value));
    }

    /// <summary>
    /// 在队首添加元素
    /// </summary>
    /// <param name="value">要添加的元素</param>
    public void AddFirst(T value)
    {
        AddFirst(new MsgQueueNode<T>(value));
    }

    /// <summary>
    /// 在队尾添加节点
    /// </summary>
    /// <param name="node">要添加的节点</param>
    public void AddLast(MsgQueueNode<T> node)
    {
        if (null == _tail)
        {
            _head = node;
        }
        else
        {
            _tail._next = node;
        }
        node._next = null;
        _tail = node;
        _count++;
    }

    /// <summary>
    /// 在队首添加节点
    /// </summary>
    /// <param name="node">要添加的节点</param>
    public void AddFirst(MsgQueueNode<T> node)
    {
        node._next = _head;
        _head = node;
        if (0 == _count++)
        {
            _tail = node;
        }
    }

    /// <summary>
    /// 移除队首元素
    /// </summary>
    public void RemoveFirst()
    {
        _head = _head?._next;
        if (0 == --_count)
        {
            _tail = null;
        }
    }

    /// <summary>
    /// 清空队列
    /// </summary>
    public void Clear()
    {
        _count = 0;
        _head = _tail = null;
    }

    /// <summary>
    /// 获取枚举器
    /// </summary>
    /// <returns>枚举器</returns>
    public IEnumerator<T> GetEnumerator()
    {
        MsgQueueNode<T>? it = _head;
        while (null != it)
        {
            yield return it._value;
            it = it._next;
        }
        yield break;
    }

    /// <summary>
    /// 转换为列表
    /// </summary>
    public T[] ToList
    {
        get
        {
            int idx = 0;
            T[] list = new T[_count];
            MsgQueueNode<T>? it = _head;
            while (null != it)
            {
                list[idx++] = it._value;
                it = it._next;
            }
            return list;
        }
    }

    /// <summary>
    /// 获取队首节点
    /// </summary>
    public MsgQueueNode<T>? First
    {
        get
        {
            return _head;
        }
    }

    /// <summary>
    /// 获取队尾节点
    /// </summary>
    public MsgQueueNode<T>? Last
    {
        get
        {
            return _tail;
        }
    }

    /// <summary>
    /// 获取队列元素数量
    /// </summary>
    public int Count
    {
        get
        {
            return _count;
        }
    }
}
