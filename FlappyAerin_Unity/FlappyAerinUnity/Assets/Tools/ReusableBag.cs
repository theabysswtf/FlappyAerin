using System.Collections.Concurrent;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tools
{
    
    /// <summary>
    /// Base Interface
    /// </summary>
    public delegate void ReturnDelegate(IReusable reusable);
    public interface IReusable
    {
        ReturnDelegate ReturnToBag { [UsedImplicitly] get; set; }
    }

    /// <summary>
    /// Interface
    /// </summary>
    public interface IReusableBag<out T> where T : IReusable
    {
        [UsedImplicitly] public void Return(IReusable t);
        public T Get();
    }
    
    /// <summary>
    /// Data Structure
    /// </summary>
    public class ReusableBag<T> : IReusableBag<T> where T : class, IReusable
    {
        ConcurrentBag<T> Bag { get; } = new();
        readonly Object _prefab;
        readonly Transform _sceneAnchor;

        public ReusableBag(ref Object baseObject, Transform transform)
        {
            if (_prefab == null)
            {
                _prefab = baseObject;   
            }

            _sceneAnchor = transform;
        }

        T Generate()
        {
            GameObject obj = Object.Instantiate(_prefab, _sceneAnchor, true) as GameObject;
            // ReSharper disable once PossibleNullReferenceException
            IReusable pb = obj.GetComponent<T>();
            pb.ReturnToBag = Return;

            return (T) pb;
        }

        public T Get()
        {
            return (Bag.TryTake(out var t)) ? t : Generate();
        }

        public void Return(IReusable t)
        {
            Bag.Add(t as T);
        }
    }
}