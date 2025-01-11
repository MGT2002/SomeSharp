using System.Numerics;

namespace GenericProject;

public class LRUCache<T> : ILRUCache where T : struct, INumber<T>, IIncrementOperators<T>
{
    //1. Key-Id dictionary
    //2. Id-(Value, key) dictionary
    //3. Id(ulong) is update number

    int capacity;
    int size = 0;
    T smallestId = T.Zero;
    T biggestId = T.Zero;
    Dictionary<int, T> keyIdMap = [];
    Dictionary<T, (int value, int key)> cache = [];

    T GetId(int key) => keyIdMap[key];

    public LRUCache(int capacity)
    {
        this.capacity = capacity;
    }

    public int Get(int key)
    {
        if (!keyIdMap.TryGetValue(key, out T id))
            return -1;

        var value = cache[++biggestId] = cache[id];
        cache.Remove(id);
        keyIdMap[key] = biggestId;

        return value.value;
    }

    public void Put(int key, int value)
    {
        bool isNewKey = !keyIdMap.TryGetValue(key, out T id);

        if (size == capacity)
        {
            if (isNewKey)
                EvictLast(); 
        }
        else if(isNewKey)
            size++;

        if (!isNewKey)
            cache.Remove(id);

        keyIdMap[key] = ++biggestId;
        cache[GetId(key)] = (value, key);
    }

    private void EvictLast()
    {
        while (!cache.ContainsKey(smallestId))
            smallestId++;

        cache.Remove(smallestId, out (int, int key) value);
        keyIdMap.Remove(value.key);
    }
}
