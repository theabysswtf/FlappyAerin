using System;
using System.Collections.Concurrent;
using Tools;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Engine
{
    
    /// <summary>
    /// Base Interface
    /// </summary>
    public delegate void ReturnDelegate(IReusable reusable);
    public interface IReusable
    {
        public void LoadParams(ref ReusableParams p);
        ReturnDelegate ReturnToBag { get; set; }
    }

    public interface IReusableService<T> : IService
    {
        public void Instance(ref ReusableParams p, out T ret);
    }

    /// <summary>
    /// Interface
    /// </summary>
    public interface IReusableBag<out T> where T : IReusable
    {
        public void Return(IReusable t);
        public T Get();
    }
    
    /* ProjectileService : Monobehaviour, IReusableService<Projectile>
     * SoundBoxService : Monobehaviour, IReusableService<SoundBox>
     *  
     *
     * 
     */
    
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
            pb.ReturnToBag = ((IReusableBag<T>) this).Return;

            return (T) pb;
        }

        T IReusableBag<T>.Get()
        {
            return (Bag.TryTake(out var t)) ? t : Generate();
        }

        void IReusableBag<T>.Return(IReusable t)
        {
            Bag.Add(t as T);
        }
    }
}