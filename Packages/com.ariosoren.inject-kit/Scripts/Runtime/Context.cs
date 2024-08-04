using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArioSoren.InjectKit
{
    public class Context : IContext
    {
        #region Protected Fields

        protected readonly Dictionary<Type, IModule> Modules;

        #endregion

        #region Private Fields

        private readonly IModuleFactory _moduleFactory;

        #endregion

        #region Constructors

        public Context(IModuleFactory moduleFactory)
        {
            _moduleFactory = moduleFactory;
            Modules = new Dictionary<Type, IModule>();
        }

        #endregion

        #region IContext Interface

        public virtual T Register<T>() where T : IModule, new()
        {
            IModule module = _moduleFactory.Create<T>();
            module.OnRegister(this);
            Modules.Add(module.GetType(), module);
            return (T)module;
        }


        public T Get<T>() where T : class, IModule
        {
            Type t = typeof(T);
            return Modules.TryGetValue(t, out IModule module) ? module as T : default;
        }

        public virtual T Remove<T>() where T : class, IModule
        {
            T module = Get<T>();
            if (module != default)
            {
                Modules.Remove(typeof(T));
            }

            return module;
        }

        public virtual void Init()
        {
            foreach (KeyValuePair<Type, IModule> module in Modules) module.Value.Init();
        }


        public IList<T> GetAll<T>()
        {
            List<T> result = new List<T>();
            Type reqType = typeof(T);
            foreach (var module in Modules)
            {
                Type type = module.Value.GetType();
                if (module.Value is T m)
                {
                    result.Add(m);

                }
            }

            return result;
        }

        public virtual void AddInstance(IModule module)
        {
            Modules.Add(module.GetType(), module);
        }

        public virtual void RemoveInstance(IModule module)
        {
            Modules.Remove(module.GetType());
        }

        #endregion



    }

}


