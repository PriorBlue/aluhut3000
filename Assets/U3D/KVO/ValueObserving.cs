using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace U3D.KVO
{
    public class ReadOnlyValueObserving<T>
    {
        protected T m_value = default(T);
        public T get
        {
            get
            {
                return m_value;
            }
        }
        protected void SetValue(T v)
        {
            if ((m_value == null && v!= null) || !m_value.Equals(v))
            {
                m_value = v;
                ScopedAction<T>[] list = new ScopedAction<T>[m_observers.Count];
                m_observers.Values.CopyTo(list, 0);
                var doCleanup = NotifyObserversAndCheckForCleanup(list);
                if (doCleanup) CleanupObservers();
            }
        }

        private void CleanupObservers()
        {
            List<Guid> removeMe = new List<Guid>();

            foreach (var it in m_observers)
            {
                if (it.Value.isScoped && it.Value.gameObjectScope == null)
                {
                    removeMe.Add(it.Key);
                }
            }

            foreach (var guid in removeMe) m_observers.Remove(guid);
        }
        
        struct ScopedAction<TT>
        {
            public Action<TT> action;
            public bool isScoped;
            public GameObject gameObjectScope;
        }

        Dictionary<Guid, ScopedAction<T>> m_observers = new Dictionary<Guid, ScopedAction<T>>();
        
        public Guid RegisterObserver(Action<T> action)
        {
            Guid ret = Guid.NewGuid();
            m_observers.Add(ret, new ScopedAction<T>{
                action = action,
                gameObjectScope = null,
                isScoped = false,
            });
            action(m_value);
            return ret;
        }

        /// <summary>
        /// Automatically removes the observer if the linked gameobject gets deleted
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public Guid RegisterObserverScoped(GameObject scope, Action<T> action)
        {
            Guid ret = Guid.NewGuid();
            m_observers.Add(ret, new ScopedAction<T>
            {
                action = action,
                gameObjectScope = scope,
                isScoped = true,
            });
            action(m_value);
            return ret;
        }

        public void RemoveObserver(Guid g)
        {
            m_observers.Remove(g);
        }
        private bool NotifyObserversAndCheckForCleanup(ScopedAction<T>[] list)
        {
            bool cleanupNecessary = false;

            foreach (ScopedAction<T> a in list)
            {
                if (a.isScoped)
                {
                    if (a.gameObjectScope != null) a.action(m_value);
                    else cleanupNecessary = true;
                }
                else
                {
                    a.action(m_value);
                }
            }

            return cleanupNecessary;
        }
    }
    
    public class ValueObserving<T> : ReadOnlyValueObserving<T>
    {
        public T set
        {
            set
            {
                SetValue(value);
            }
        }
    }
}