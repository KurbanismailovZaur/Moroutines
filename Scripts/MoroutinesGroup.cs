using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using static Redcode.Moroutines.Moroutine;

namespace Redcode.Moroutines
{
    /// <summary>
    /// Allows you to work with multiple moroutines.
    /// </summary>
    public class MoroutinesGroup
    {
        /// <summary>
        /// Moroutines added to the group.
        /// </summary>
        public List<Moroutine> Moroutines { get; } = new();

        /// <summary>
        /// Moroutines owner. If moroutines have different owners, <see langword="null"/> will be returned.
        /// </summary>
        public MoroutinesOwner Owner
        {
            get
            {
                if (Moroutines.Count == 0)
                    return null;

                var owner = Moroutines[0].Owner;
                
                for (int i = 1; i < Moroutines.Count; i++)
                {
                    if (Moroutines[i].Owner != owner)
                        return null;
                }

                return owner;
            }
        }

        #region State
        /// <summary>
        /// Are all moroutines in the reset state?
        /// </summary>
        public bool IsReseted => Moroutines.All(m => m.IsReseted);

        /// <summary>
        /// Are all moroutines in the running state?
        /// </summary>
        public bool IsRunning => Moroutines.All(m => m.IsRunning);

        /// <summary>
        /// Are all moroutines in the stopped state?
        /// </summary>
        public bool IsStopped => Moroutines.All(m => m.IsStopped);

        /// <summary>
        /// Are all moroutines in the completed state?
        /// </summary>
        public bool IsCompleted => Moroutines.All(m => m.IsCompleted);

        /// <summary>
        /// Are all moroutines in the destroyed state?
        /// </summary>
        public bool IsDestroyed => Moroutines.All(m => m.IsDestroyed);

        /// <summary>
        /// Are all moroutines have owner?
        /// </summary>
        public bool IsOwned => Moroutines.All(m => m.IsOwned);
        #endregion

        /// <summary>
        /// Sets auto destroy option to all moroutines.
        /// </summary>
        public bool AutoDestroy
        {
            get => Moroutines.All(m => m.AutoDestroy);
            set => Moroutines.ForEach(m => m.AutoDestroy = value);
        }

        #region Events
        /// <summary>
        /// The event that is redeemed when the group resets all moroutines to its initial state.
        /// Triggered after <see cref="Reset"/> method called.
        /// </summary>
        public event Action<MoroutinesGroup> Reseted;

        /// <summary>
        /// The event that is redeemed when the group starts performing.
        /// Triggered after <see cref="Run"/> method called.
        /// </summary>
        public event Action<MoroutinesGroup> Running;

        /// <summary>
        /// The event that is redeemed when the group stops executing.
        /// Triggered after <see cref="Stop"/> method called.
        /// </summary>
        public event Action<MoroutinesGroup> Stopped;

        /// <summary>
        /// The event that is emit when the moroutines destroyed.
        /// Triggered after <see cref="Destroy"/> method called.
        /// </summary>
        public event Action<MoroutinesGroup> Destroyed;
        #endregion

        #region Constructors
        /// <summary>
        /// Create empty group.
        /// </summary>
        public MoroutinesGroup() { }

        /// <summary>
        /// Create group with passed <paramref name="moroutines"/>.
        /// </summary>
        /// <param name="moroutines">Moroutines to add in group.</param>
        public MoroutinesGroup(params Moroutine[] moroutines) : this((IEnumerable<Moroutine>)moroutines) { }

        /// <summary>
        /// Create group with passed <paramref name="moroutines"/>.
        /// </summary>
        /// <param name="moroutines">Moroutines to add in group.</param>
        public MoroutinesGroup(IEnumerable<Moroutine> moroutines) => Moroutines.AddRange(moroutines);
        #endregion

        #region Owning
        /// <summary>
        /// Find all unowned moroutines in the group.
        /// </summary>
        /// <returns>Unowned moroutines.</returns>
        public List<Moroutine> GetUnownedMoroutines()
        {
            return GetUnownedMoroutines(State.Reseted | State.Running | State.Stopped | State.Completed | State.Destroyed);
        }

        /// <summary>
        /// Find all unowned moroutines in the group by mask.
        /// </summary>
        /// <param name="mask">Mask to filter moroutines.</param>
        /// <returns>Unowned moroutines.</returns>
        public List<Moroutine> GetUnownedMoroutines(State mask)
        {
            return Moroutine.GetUnownedMoroutines(mask).Where(m => Moroutines.Contains(m)).ToList();
        }

        /// <summary>
        /// Sets owner to all moroutines.
        /// </summary>
        /// <param name="component">Any owner's component.</param>
        public MoroutinesGroup SetOwner(Component component) => SetOwner(component.gameObject);

        /// <summary>
        /// Sets owner to all moroutines.
        /// </summary>
        /// <param name="gameObject">Owner game object.</param>
        public MoroutinesGroup SetOwner(GameObject gameObject)
        {
            Moroutines.ForEach(mor => mor.SetOwner(gameObject));
            return this;
        }

        /// <summary>
        /// Makes all moroutines unowned.
        /// </summary>
        public MoroutinesGroup MakeUnowned() => SetOwner((GameObject)null);
        #endregion

        #region Control
        /// <summary>
        /// Resets all moroutines in group.
        /// </summary>
        /// <returns>The moroutines group.</returns>
        public MoroutinesGroup Reset()
        {
            Moroutines.ForEach(m => m.Reset());
            Reseted?.Invoke(this);

            return this;
        }

        /// <summary>
        /// Runs all moroutines in group.
        /// </summary>
        /// <returns>The moroutines group.</returns>
        public MoroutinesGroup Run()
        {
            Moroutines.ForEach(m => m.Run());
            Running?.Invoke(this);

            return this;
        }

        /// <summary>
        /// Stops all moroutines in group.
        /// </summary>
        /// <returns>The moroutines group.</returns>
        public MoroutinesGroup Stop()
        {
            Moroutines.ForEach(m => m.Stop());
            Stopped?.Invoke(this);

            return this;
        }

        /// <summary>
        /// Reruns all moroutines in group.
        /// </summary>
        /// <returns>The moroutines group.</returns>
        public MoroutinesGroup Rerun()
        {
            Reset();
            Run();

            return this;
        }
        #endregion

        #region Subscribing
        private MoroutinesGroup OnSubscribe(ref Action<MoroutinesGroup> ev, Action<MoroutinesGroup> action)
        {
            ev += action;
            return this;
        }

        /// <summary>
        /// Subscribe to reset event.
        /// </summary>
        /// <param name="action">Callback to invoke.</param>
        /// <returns>The moroutines group.</returns>
        public MoroutinesGroup OnReseted(Action<MoroutinesGroup> action) => OnSubscribe(ref Reseted, action);

        /// <summary>
        /// Subscribe to run event.
        /// </summary>
        /// <param name="action"><inheritdoc cref="OnReseted(Action{Moroutine})"/></param>
        /// <returns>The moroutines group.</returns>
        public MoroutinesGroup OnRunning(Action<MoroutinesGroup> action) => OnSubscribe(ref Running, action);

        /// <summary>
        /// Subscribe to stop event.
        /// </summary>
        /// <param name="action"><inheritdoc cref="OnReseted(Action{Moroutine})"/></param>
        /// <returns>The moroutines group.</returns>
        public MoroutinesGroup OnStopped(Action<MoroutinesGroup> action) => OnSubscribe(ref Stopped, action);
        #endregion

        #region Yielders
        /// <summary>
        /// Create an awaiter object, wich knows how to wait until all moroutines is complete. <br/>
        /// Changing the list of moroutines after calling this method will have no effect on the awaiting.
        /// </summary>
        /// <returns>Awaiter object.</returns>
        public WaitForAll WaitForComplete() => new(Moroutines.Select(m => m.WaitForComplete()).ToList());

        /// <summary>
        /// Create an awaiter object, wich knows how to wait until all moroutines is stopped.
        /// Changing the list of moroutines after calling this method will have no effect on the awaiting.
        /// </summary>
        /// <returns><inheritdoc cref="WaitForComplete"/></returns>
        public WaitForAll WaitForStop() => new(Moroutines.Select(m => m.WaitForStop()).ToList());

        /// <summary>
        /// Create an awaiter object, wich knows how to wait until all moroutines is runned.
        /// Changing the list of moroutines after calling this method will have no effect on the awaiting.
        /// </summary>
        /// <returns><inheritdoc cref="WaitForComplete"/></returns>
        public WaitForAll WaitForRun() => new(Moroutines.Select(m => m.WaitForRun()).ToList());

        /// <summary>
        /// Create an awaiter object, wich knows how to wait until all moroutines is reseted.
        /// Changing the list of moroutines after calling this method will have no effect on the awaiting.
        /// </summary>
        /// <returns><inheritdoc cref="WaitForComplete"/></returns>
        public WaitForAll WaitForReset() => new(Moroutines.Select(m => m.WaitForReset()).ToList());

        /// <summary>
        /// Create an awaiter object, wich knows how to wait until all moroutines is destroyed.
        /// Changing the list of moroutines after calling this method will have no effect on the awaiting.
        /// </summary>
        /// <returns><inheritdoc cref="WaitForComplete"/></returns>
        public WaitForAll WaitForDestroy() => new(Moroutines.Select(m => m.WaitForDestroy()).ToList());
        #endregion

        #region Destroying
        /// <summary>
        /// Stop and destroy moroutines immediatly. You can't run moroutines after it destroying.
        /// </summary>
        public MoroutinesGroup Destroy()
        {
            Moroutines.ForEach(m => m.Destroy());
            Destroyed?.Invoke(this);

            return this;
        }

        /// <summary>
        /// Sets moroutines auto destroying.
        /// </summary>
        /// <param name="autoDestroy"><see langword="true"/> if you need destroy moroutines after completion.</param>
        /// <returns>The moroutine.</returns>
        public MoroutinesGroup SetAutoDestroy(bool autoDestroy)
        {
            AutoDestroy = autoDestroy;
            return this;
        }
        #endregion
    }
}
