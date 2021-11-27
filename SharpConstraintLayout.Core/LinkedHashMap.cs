using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

/// <summary>
/// 参考:
/// 1. https://stackoverflow.com/questions/29205934/c-sharp-equivalent-of-linkedhashmap
/// 2. https://stackoverflow.com/questions/754233/is-it-there-any-lru-implementation-of-idictionary/3719378#3719378
/// </summary>
/// <typeparam name="K"></typeparam>
/// <typeparam name="V"></typeparam>
internal class LinkedHashMap<K, V>
{
    private Dictionary<K, LinkedListNode<Tuple<K, V>>> D = new Dictionary<K, LinkedListNode<Tuple<K, V>>>();
    private LinkedList<Tuple<K, V>> LL = new LinkedList<Tuple<K, V>>();

    public LinkedHashMap()
    {
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public V get(K key)
    {
        return D[key].Value.Item2;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void put(K key, V val)
    {
        if (D.TryGetValue(key, out var existingNode))
        {
            LL.Remove(existingNode);
        }

        Tuple<K, V> cacheItem = new Tuple<K, V>(key, val);
        LinkedListNode<Tuple<K, V>> node = new LinkedListNode<Tuple<K, V>>(cacheItem);
        LL.AddLast(node);
        D[key] = node;
    }

    private void RemoveFirst()
    {
        // Remove from LRUPriority
        LinkedListNode<Tuple<K, V>> node = LL.First;
        LL.RemoveFirst();

        // Remove from cache
        D.Remove(node.Value.Item1);
    }

    public int Count
    {
        get
        {
            return D.Count;
        }
    }

    public bool containsKey(K key)
    {
        return D.ContainsKey(key);
    }
}
