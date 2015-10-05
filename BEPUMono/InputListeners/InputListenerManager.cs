﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;

namespace BEPUMono.InputListeners
{
    public class InputListenerManager
    {
        public InputListenerManager()
        {
            _listeners = new List<InputListener>();
        }

        private readonly List<InputListener> _listeners;

        public IEnumerable<InputListener> Listeners => _listeners;

	    public T AddListener<T>(InputListenerSettings<T> settings)
            where T : InputListener
        {
            var listener = settings.CreateListener();
            _listeners.Add(listener);
            return listener;
        }

        public T AddListener<T>()
            where T : InputListener
        {
            var constructors = typeof(T)
                .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(c => !c.GetParameters().Any())
                .ToArray();

            if (!constructors.Any())
                throw new InvalidOperationException($"No parameterless constructor defined for type {typeof (T).Name}");

            var listener = (T)constructors[0].Invoke(new object[0]);
            _listeners.Add(listener);
            return listener;
        }

        public void RemoveListener(InputListener listener)
        {
            _listeners.Remove(listener);
        }

        public void Update(GameTime gameTime) 
        {
            foreach (var listener in _listeners)
                listener.Update(gameTime);
        }
    }
}
