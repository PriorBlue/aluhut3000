using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;

namespace U3D.KVO
{
    public class ReadOnlyListObserving<T>
    {
		protected List<T> m_previous_value = new List<T>();
        protected List<T> m_value = new List<T>();
        public List<T> get
        {
            get
            {
                return m_value;
            }
        }    
        protected void SetValue(List<T> v)
        {
			m_previous_value = m_value;
            m_value = v;
            ScopedAction<T>[] list = new ScopedAction<T>[m_observers.Count];
            m_observers.Values.CopyTo(list, 0);
            var doCleanup = NotifyObserversAndCheckForCleanup(list);
            if (doCleanup) CleanupObservers();
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
			public Action<ReadOnlyCollection<TT>, ReadOnlyCollection<TT>> action;
            public bool isScoped;
            public GameObject gameObjectScope;
        }

        Dictionary<Guid, ScopedAction<T>> m_observers = new Dictionary<Guid, ScopedAction<T>>();
		public Guid RegisterObserver(Action<ReadOnlyCollection<T>, ReadOnlyCollection<T>> action)
        {
            Guid ret = Guid.NewGuid();
            m_observers.Add(ret, new ScopedAction<T>
            {
                action = action,
                gameObjectScope = null,
                isScoped = false,
            });
            action(m_previous_value.AsReadOnly(), m_value.AsReadOnly());
            return ret;
        }

        /// <summary>
        /// Automatically removes the observer if the linked gameobject gets deleted
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="action"></param>
        /// <returns></returns>
		public Guid RegisterObserverScoped(GameObject scope, Action<ReadOnlyCollection<T>, ReadOnlyCollection<T>> action)
        {
            Guid ret = Guid.NewGuid();
            m_observers.Add(ret, new ScopedAction<T>
            {
                action = action,
                gameObjectScope = scope,
                isScoped = true,
            });
            action(m_previous_value.AsReadOnly(), m_value.AsReadOnly());
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
                    if (a.gameObjectScope != null) a.action(m_previous_value.AsReadOnly(), m_value.AsReadOnly());
                    else cleanupNecessary = true;
                }
                else
                {
                    a.action(m_previous_value.AsReadOnly(), m_value.AsReadOnly());
                }
            }

            return cleanupNecessary;
        }
    }
    
    public class ListObserving<T> : ReadOnlyListObserving<T>
    {
        public List<T> set
        {
            set
            {
                SetValue(value);
            }
        }
    }
}